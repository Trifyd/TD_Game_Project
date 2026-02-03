namespace TowerDefense.World;
using Raylib_cs;
using System.Numerics;
using TowerDefense.Enums;

public class MapManager
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public int TileSize { get; private set; }

    private TileType[,] grid;

    public MapManager(int rows, int cols, int tileSize)
    {
        Rows = rows;
        Cols = cols;
        TileSize = tileSize;
        grid = new TileType[cols, rows];
    }

    public void DrawGrid()
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Cols; x++)
            {
                Color color = grid[x, y] switch
                {
                    TileType.Path => Color.Gray,
                    TileType.Wall => Color.DarkGray,
                    _ => Color.DarkGreen // Default to Grass
                };

                Raylib.DrawRectangle(x * TileSize, y * TileSize, TileSize, TileSize, color);
                Raylib.DrawRectangleLines(x * TileSize, y * TileSize, TileSize, TileSize, Color.Black);
            }
        }
    }

    // Returns the grid coordinates (column, row) based on mouse position
    public Vector2 GetTileAtMouse()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        int x = (int)mousePos.X / TileSize;
        int y = (int)mousePos.Y / TileSize;
        return new Vector2(x, y);
    }
}