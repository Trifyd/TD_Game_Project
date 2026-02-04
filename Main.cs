namespace TowerDefense;
using Raylib_cs;
using System.Numerics;
using TowerDefense.Core;
using TowerDefense.Levels;
using TowerDefense.Enums;
class Program
{
    static GameState currentState = GameState.Menu;
    static Button btnStart = default!;
    static Button btnEditor = default!;
    static Button btnExit = default!;
    static void Main()
    {
        WindowManager window = new WindowManager(800, 600, "Tower Defense - From Zero");
        
        int rows = 15;
        int cols = 20;
        int tileSize = 40;
        MapManager gameMap = new MapManager(rows, cols, tileSize);

        btnStart = new Button(300, 250, 200, 50, "START GAME", Color.DarkGreen);
        btnEditor = new Button(300, 320, 200, 50, "MAP EDITOR", Color.DarkBlue);
        btnExit = new Button(300, 390, 200, 50, "EXIT", Color.Maroon);

        while (!Raylib.WindowShouldClose())
        {
            if (currentState == GameState.Menu)
            {
                if (currentState == GameState.Menu)
                {
                    if (btnStart.IsClicked())
                    {
                        gameMap.InitializeDefaultMap();
                        currentState = GameState.Playing;
                    }
                    if (btnEditor.IsClicked())
                    {
                        gameMap.InitializeDefaultMap();
                        gameMap.LoadLevel("Default"); 
                        currentState = GameState.Editor;
                    }
                    if (btnExit.IsClicked()) break;
                }
            }
            window.PrepareFrame();

            switch (currentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Playing:
                    gameMap.DrawGrid();
                    break;
                case GameState.Editor:
                    gameMap.DrawGrid();
                    break;
            }

            if (currentState == GameState.Playing)
            {
                Vector2 mouseTile = gameMap.GetTileAtMouse();
                window.PrepareFrame();
                gameMap.DrawGrid();

                if (mouseTile.X >= 0 && mouseTile.X < cols && mouseTile.Y >= 0 && mouseTile.Y < rows)
                {
                    Raylib.DrawRectangle(
                    (int)mouseTile.X * tileSize, 
                    (int)mouseTile.Y * tileSize, 
                    tileSize, tileSize, 
                    Raylib.Fade(Color.SkyBlue, 0.5f)
                    );
                }
            }
            if (currentState == GameState.Editor)
            {
                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    gameMap.UpdateTile(Raylib.GetMousePosition(), TileType.Path);
                }
                if (Raylib.IsMouseButtonDown(MouseButton.Right))
                {
                    gameMap.UpdateTile(Raylib.GetMousePosition(), TileType.Grass);
                }
            }
            window.EndFrame();
        }
        window.Close();
    }
    static void DrawMenu()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        string title = "TOWER DEFENSE: ZERO";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        int titleX = (800 - titleWidth) / 2;
        Raylib.DrawText(title, titleX, 100, titleFontSize, Color.Yellow);
        btnStart.Draw();
        btnEditor.Draw();
        btnExit.Draw();
    }
}