namespace TowerDefense;
using Raylib_cs;
using System.Numerics;
using TowerDefense.Core;
using TowerDefense.World;
using TowerDefense.Enums;
class Program
{
    static GameState currentState = GameState.Menu;
    static void Main()
    {
        WindowManager window = new WindowManager(800, 600, "Tower Defense - From Zero");
        //will be managable later
        int rows = 15;
        int cols = 20;
        int tileSize = 40;
        MapManager map = new MapManager(rows, cols, tileSize);

        while (!Raylib.WindowShouldClose())
        {
            if (currentState == GameState.Menu)
            {
                DrawMenu();
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    currentState = GameState.Playing;
                }
            }
            else if (currentState == GameState.Playing)
            {
                // Game logic
                Vector2 mouseTile = map.GetTileAtMouse();
                window.PrepareFrame();
            
                map.DrawGrid();

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
            window.EndFrame();
        }

        window.Close();
    }
    static void DrawMenu()
    {
        Raylib.ClearBackground(Color.DarkBlue);
        
        int titleWidth = Raylib.MeasureText("TOWER DEFENSE: ZERO", 40);
        Raylib.DrawText("TOWER DEFENSE: ZERO", (800 - titleWidth) / 2, 200, 40, Color.Yellow);
        
        int subWidth = Raylib.MeasureText("Press [ENTER] to Start", 20);
        Raylib.DrawText("Press [ENTER] to Start", (800 - subWidth) / 2, 300, 20, Color.White);
    }
}