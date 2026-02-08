#include <SDL.h>

int main(int argc, char* argv[])
{
    SDL_Init(SDL_INIT_VIDEO);

    SDL_Window* win = SDL_CreateWindow(
        "Test",
        100, 100,
        640, 480,
        SDL_WINDOW_SHOWN
    );

    SDL_Delay(2000);

    SDL_DestroyWindow(win);
    SDL_Quit();
    return 0;
}
