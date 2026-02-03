namespace TowerDefense.Core;
using Raylib_cs;

public class WindowManager
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public WindowManager(int width, int height, string title)
    {
        Width = width;
        Height = height;
        Raylib.InitWindow(Width, Height, title);
        Raylib.SetTargetFPS(60);
    }

    public void PrepareFrame()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
    }

    public void EndFrame()
    {
        Raylib.EndDrawing();
    }

    public void Close()
    {
        Raylib.CloseWindow();
    }
}