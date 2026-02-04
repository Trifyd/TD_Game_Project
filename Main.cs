namespace TowerDefense;
using Raylib_cs;
using TowerDefense.Core;
using TowerDefense.Levels;
class Program
{
    static void Main()
    {
        WindowManager window = new WindowManager(800, 600, "Tower Defense - From Zero");
        
        int rows = 15;
        int cols = 20;
        int tileSize = 40;
        
        MapManager gameMap = new MapManager(rows, cols, tileSize);
        CursorControl cursor = new CursorControl(tileSize);
        
        Button btnStart = new Button(300, 250, 200, 50, "START GAME", Color.DarkGreen);
        Button btnEditor = new Button(300, 320, 200, 50, "MAP EDITOR", Color.DarkBlue);
        Button btnExit = new Button(300, 390, 200, 50, "EXIT", Color.Maroon);
        
        GameManager gameManager = new GameManager(gameMap, cursor, btnStart, btnEditor, btnExit);

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            // Update
            bool continueRunning = gameManager.Update();
            if (!continueRunning) break;
            // Draw
            window.PrepareFrame();
            gameManager.Draw();
            window.EndFrame();
        }
        window.Close();
    }
}






