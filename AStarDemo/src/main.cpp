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

//A＊経路探索
std::vector<Node*> FindPath(Node* start, Node* goal)
{
    std::vector<Node*> open;
    std::vector<Node*> closed;

    open.push_back(start);

    while (!open.empty())
    {
        // f最小ノード取得
        auto current = *std::min_element(open.begin(), open.end(),
            [](Node* a, Node* b) { return a->f < b->f; });

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

        open.erase(std::find(open.begin(), open.end(), current));
        closed.push_back(current);

        const int dx[4] = { 1,-1,0,0 };
        const int dy[4] = { 0,0,1,-1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = current->x + dx[i];
            int ny = current->y + dy[i];

            if (nx < 0 || ny < 0 || nx >= COLS || ny >= ROWS)
                continue;

            Node* neighbor = &grid[ny][nx];

            if (!neighbor->walkable)
                continue;

            if (std::find(closed.begin(), closed.end(), neighbor) != closed.end())
                continue;

            int newG = current->g + 1;

            if (std::find(open.begin(), open.end(), neighbor) == open.end())
                open.push_back(neighbor);
            else if (newG >= neighbor->g)
                continue;

            neighbor->parent = current;
            neighbor->g = newG;
            neighbor->h = heuristic(neighbor, goal);
            neighbor->f = neighbor->g + neighbor->h;
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

    //ゲームループ
    while (running)
    {
        while (SDL_PollEvent(&e))
        {
            if (e.type == SDL_QUIT)
            {
                running = false;
            }  

            ////マウスクリックで壁を作る。
            //if (e.type == SDL_MOUSEBUTTONDOWN)
            //{
            //    int mx, my;
            //    SDL_GetMouseState(&mx, &my);

            //    int gx = mx / CELL_SIZE;
            //    int gy = my / CELL_SIZE;

            //    grid[gy][gx].walkable = !grid[gy][gx].walkable;
            //}

            //右クリックで壁をクリア
            if (e.type == SDL_MOUSEBUTTONDOWN &&
                e.button.button == SDL_BUTTON_RIGHT)
            {
                ResetGrid();
            }
        }

        int mx, my;
        Uint32 buttons = SDL_GetMouseState(&mx, &my);

        //左クリック中のドラッグで壁を作る。
        if (buttons & SDL_BUTTON(SDL_BUTTON_LEFT))
        {
            int gx = mx / CELL_SIZE;
            int gy = my / CELL_SIZE;

            if (gx >= 0 && gx < COLS && gy >= 0 && gy < ROWS)
            {
                grid[gy][gx].walkable = false;
            }
        }

        //背景
        SDL_SetRenderDrawColor(renderer, 30, 30, 30, 255);
        SDL_RenderClear(renderer);

        //テスト描画設定。
        auto path = FindPath(&grid[1][1], &grid[10][10]);

        for (auto n : path)
        {
            DrawCell(renderer, n->x, n->y, { 0,255,0,255 });
        }
        //スタートとゴールが分かりやすいように塗る
        DrawCell(renderer, 1, 1, { 255, 100, 100, 255 });
        DrawCell(renderer, 10, 10, { 100,200, 255, 255 });

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
