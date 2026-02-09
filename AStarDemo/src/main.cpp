#include <SDL.h>

constexpr int WINDOW_WIDTH = 640;
constexpr int WINDOW_HEIGHT = 480;
constexpr int CELL_SIZE = 40;

//グリッド表示
void drawGrid(SDL_Renderer* renderer)
{
    SDL_SetRenderDrawColor(renderer, 60, 60, 60, 255);

    // 縦線
    for (int x = 0; x <= WINDOW_WIDTH; x += CELL_SIZE)
    {
        SDL_RenderDrawLine(renderer, x, 0, x, WINDOW_HEIGHT);
    }

    // 横線
    for (int y = 0; y <= WINDOW_HEIGHT; y += CELL_SIZE)
    {
        SDL_RenderDrawLine(renderer, 0, y, WINDOW_WIDTH, y);
    }
}

//セル塗
void drawCell(SDL_Renderer* renderer, int gx, int gy, SDL_Color color)
{
    SDL_Rect rect;

    rect.x = gx * CELL_SIZE;
    rect.y = gy * CELL_SIZE;
    rect.w = CELL_SIZE - 1; // 線が見えるように-1
    rect.h = CELL_SIZE - 1;

    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
    SDL_RenderFillRect(renderer, &rect);
}


int main(int argc, char* argv[])
{
    SDL_Init(SDL_INIT_VIDEO);

    SDL_Window* win = SDL_CreateWindow(
        "AStar Demo",
        100, 100,
        WINDOW_WIDTH, WINDOW_HEIGHT,
        SDL_WINDOW_SHOWN
    );

    // Renderer作成
    SDL_Renderer* renderer = SDL_CreateRenderer(win, -1, SDL_RENDERER_ACCELERATED);

    bool running = true;
    SDL_Event e;

    // ゲームループ
    while (running)
    {
        while (SDL_PollEvent(&e))
        {
            if (e.type == SDL_QUIT)
                running = false;
        }

        // 背景
        SDL_SetRenderDrawColor(renderer, 30, 30, 30, 255);
        SDL_RenderClear(renderer);

        // グリッド描画
        drawGrid(renderer);
        //テストで3,4を赤で塗る。
        drawCell(renderer, 3, 4, { 255, 100, 100, 255 });

        SDL_RenderPresent(renderer);
    }

    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(win);
    SDL_Quit();
    return 0;
}
