#include <SDL.h>
#include <vector>
#include <queue>
#include <algorithm>

constexpr int WINDOW_WIDTH = 640;
constexpr int WINDOW_HEIGHT = 480;
constexpr int CELL_SIZE = 40;
constexpr int COLS = WINDOW_WIDTH / CELL_SIZE;
constexpr int ROWS = WINDOW_HEIGHT / CELL_SIZE;

//グリッド表示
void DrawGrid(SDL_Renderer* renderer)
{
    SDL_SetRenderDrawColor(renderer, 60, 60, 60, 255);

    //縦線
    for (int x = 0; x <= WINDOW_WIDTH; x += CELL_SIZE)
    {
        SDL_RenderDrawLine(renderer, x, 0, x, WINDOW_HEIGHT);
    }

    //横線
    for (int y = 0; y <= WINDOW_HEIGHT; y += CELL_SIZE)
    {
        SDL_RenderDrawLine(renderer, 0, y, WINDOW_WIDTH, y);
    }
}

//セル塗
void DrawCell(SDL_Renderer* renderer, int gx, int gy, SDL_Color color)
{
    SDL_Rect rect;

    rect.x = gx * CELL_SIZE;
    rect.y = gy * CELL_SIZE;
    rect.w = CELL_SIZE - 1; //線が見えるように-1
    rect.h = CELL_SIZE - 1;

    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
    SDL_RenderFillRect(renderer, &rect);
}

//ノード
struct Node
{
    int x;
    int y;

    bool walkable = true;  //通行可能か

    int g = 0;
    int h = 0;
    int f = 0;

    Node* parent = nullptr;
};

//マンハッタン距離
int heuristic(Node* a, Node* b)
{
    return abs(a->x - b->x) + abs(a->y - b->y);
}

Node* startNode = nullptr;
Node* goalNode = nullptr;

std::vector<Node*> currentPath;


//グリッド状のノードデータ
std::vector<std::vector<Node>> grid;

//グリッドの初期化
void InitGrid()
{
    grid.resize(ROWS, std::vector<Node>(COLS));

    for (int y = 0; y < ROWS; ++y)
    {
        for (int x = 0; x < COLS; ++x)
        {
            grid[y][x] = { x, y, true };
        }
    }
}

//濃度比較
struct CompareNode
{
    bool operator()(Node* a, Node* b) const
    {
        return a->f > b->f;
    }
};


//A＊経路探索
std::vector<Node*> FindPath(Node* start, Node* goal)
{
    //データのクリア
    for (int y = 0; y < ROWS; ++y)
    {
        for (int x = 0; x < COLS; ++x)
        {
            grid[y][x].parent = nullptr;
            grid[y][x].g = grid[y][x].h = grid[y][x].f = 0;
        }
    }

    std::priority_queue<Node*, std::vector<Node*>, CompareNode> open;

    bool closed[ROWS][COLS] = {};

    open.push(start);

    while (!open.empty())
    {
        Node* current = open.top();
        open.pop();

        if (closed[current->y][current->x])
            continue;

        closed[current->y][current->x] = true;

        if (current == goal)
        {
            std::vector<Node*> path;
            while (current)
            {
                path.push_back(current);
                current = current->parent;
            }
            std::reverse(path.begin(), path.end());
            return path;
        }

        const int dx[4] = { 1,-1,0,0 };
        const int dy[4] = { 0,0,1,-1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = current->x + dx[i];
            int ny = current->y + dy[i];

            if (nx < 0 || ny < 0 || nx >= COLS || ny >= ROWS)
                continue;

            Node* neighbor = &grid[ny][nx];

            if (!neighbor->walkable || closed[ny][nx])
                continue;

            int newG = current->g + 1;

            if (neighbor->parent == nullptr || newG < neighbor->g)
            {
                neighbor->parent = current;
                neighbor->g = newG;
                neighbor->h = heuristic(neighbor, goal);
                neighbor->f = neighbor->g + neighbor->h;

                open.push(neighbor);
            }
        }
    }

    return {};
}


//ノード描画
void DrawNodes(SDL_Renderer* renderer)
{
    for (auto& row : grid)
    {
        for (auto& node : row)
        {
            if (!node.walkable)
            {
                DrawCell(renderer, node.x, node.y, { 80, 80, 80, 255 });
            }
        }
    }
}

//グリッドの情報を初期化
void ResetGrid()
{
    for (auto& row : grid)
        for (auto& node : row)
            node.walkable = true;
}


int main(int argc, char* argv[])
{
    SDL_Init(SDL_INIT_VIDEO);

    InitGrid();

    SDL_Window* win = SDL_CreateWindow(
        "AStar Demo",
        100, 100,
        WINDOW_WIDTH, WINDOW_HEIGHT,
        SDL_WINDOW_SHOWN
    );

    //Renderer作成
    SDL_Renderer* renderer = SDL_CreateRenderer(win, -1, SDL_RENDERER_ACCELERATED);

    bool running = true;
    SDL_Event e;

    bool placeStartNext = true;
    bool placingWall = false;

    //ゲームループ
    while (running)
    {
        while (SDL_PollEvent(&e))
        {
            if (e.type == SDL_QUIT)
            {
                running = false;
            }  

            // スペースでリセット
            if (e.type == SDL_KEYDOWN && e.key.keysym.sym == SDLK_SPACE)
            {
                ResetGrid();
            }

            if (e.type == SDL_MOUSEBUTTONDOWN)
            {
                int mx, my;
                SDL_GetMouseState(&mx, &my);

                int gx = mx / CELL_SIZE;
                int gy = my / CELL_SIZE;

                if (gx < 0 || gy < 0 || gx >= COLS || gy >= ROWS)
                {
                    continue;
                }
                   
                Node* clicked = &grid[gy][gx];

                //左クリックで壁を作る。
                if (e.button.button == SDL_BUTTON_LEFT)
                {
                    placingWall = true;

                    if (clicked != startNode && clicked != goalNode)
                    {
                        clicked->walkable = false;
                    }
                }

                // 右 → start/goalトグル
                if (e.button.button == SDL_BUTTON_RIGHT)
                {
                    if (placeStartNext)
                    {
                        startNode = clicked;
                    }
                    else
                    {
                        goalNode = clicked;
                    }

                    placeStartNext = !placeStartNext;

                    if (startNode && goalNode)
                    {
                        currentPath = FindPath(startNode, goalNode);
                    }
                }
            }
            // ドラッグ中
            if (e.type == SDL_MOUSEMOTION && placingWall)
            {
                int gx = e.motion.x / CELL_SIZE;
                int gy = e.motion.y / CELL_SIZE;

                if (gx >= 0 && gx < COLS && gy >= 0 && gy < ROWS)
                {
                    Node* clicked = &grid[gy][gx];

                    if (clicked != startNode && clicked != goalNode)
                        clicked->walkable = false;
                }
            }

            // 離した
            if (e.type == SDL_MOUSEBUTTONUP)
            {
                if (e.button.button == SDL_BUTTON_LEFT)
                    placingWall = false;
            }
        }

        //背景
        SDL_SetRenderDrawColor(renderer, 30, 30, 30, 255);
        SDL_RenderClear(renderer);


        for (auto n : currentPath)
        {
            DrawCell(renderer, n->x, n->y, { 0,255,0,255 });
        }

        if (startNode)
        {
            DrawCell(renderer, startNode->x, startNode->y, { 255,80,80,255 });
        }
            
        if (goalNode)
        {
            DrawCell(renderer, goalNode->x, goalNode->y, { 80,160,255,255 });
        }
            

        //ノード描画
        DrawNodes(renderer);
    
        //グリッド描画
        DrawGrid(renderer);

        SDL_RenderPresent(renderer);
    }

    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(win);
    SDL_Quit();
    return 0;
}
