namespace TowerDefense.Levels;
using Raylib_cs;
using System.Text.Json;
using System.IO;
using System.Numerics;
using TowerDefense.Enums;
using TowerDefense.Models;

public class MapManager
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public int TileSize { get; private set; }
    private TileType[,] grid;
    public PathManager PathManager { get; private set; }
    
    public MapManager(int rows, int cols, int tileSize)
    {
        Rows = rows;
        Cols = cols;
        TileSize = tileSize;
        grid = new TileType[cols, rows];
        PathManager = new PathManager();
        ClearMap(TileType.Grass);
    }
    public void ClearMap(TileType type)
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Cols; x++)
            {
                grid[x, y] = type;
            }
        }
        PathManager.Clear();
    }
    public void DrawGrid()
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Cols; x++)
            {
                Raylib_cs.Color color = grid[x, y] switch
                {
                    TileType.Path => Raylib_cs.Color.Gray,
                    TileType.Wall => Raylib_cs.Color.DarkGray,
                    TileType.Grass => Raylib_cs.Color.DarkGreen,
                    TileType.Water => Raylib_cs.Color.DarkBlue,
                    TileType.Lava => Raylib_cs.Color.Red,
                    TileType.Empty => Raylib_cs.Color.White,
                    TileType.Cover => Raylib_cs.Color.Black,
                    _ => Raylib_cs.Color.White
                };
                Raylib.DrawRectangle(x * TileSize, y * TileSize, TileSize, TileSize, color);
                Raylib.DrawRectangleLines(x * TileSize, y * TileSize, TileSize, TileSize, Raylib_cs.Color.Black);
            }
        }
        DrawPathTiles();
    }
     private void DrawPathTiles()
    {
        foreach (var pathTile in PathManager.GetAllPathTiles())
        {
            int px = pathTile.X * TileSize;
            int py = pathTile.Y * TileSize;

            // Draw type indicator
            Raylib_cs.Color typeColor = pathTile.Type switch
            {
                PathType.Start => Raylib_cs.Color.Green,
                PathType.End => Raylib_cs.Color.Red,
                PathType.Path => Raylib_cs.Color.Yellow,
                _ => Raylib_cs.Color.White
            };

            // Draw a circle in the center for the type
            int centerX = px + TileSize / 2;
            int centerY = py + TileSize / 2;
            Raylib.DrawCircle(centerX, centerY, TileSize / 6, typeColor);

            // Draw directional arrows
            DrawDirectionalArrows(pathTile, px, py);
        }
    }

    private void DrawDirectionalArrows(PathTile tile, int px, int py)
    {
        int centerX = px + TileSize / 2;
        int centerY = py + TileSize / 2;
        int arrowLength = TileSize / 3;

        if ((tile.Direction & PathDirection.Left) != 0)
        {
            Raylib.DrawLineEx(
                new Vector2(centerX, centerY),
                new Vector2(px + 5, centerY),
                2,
                Raylib_cs.Color.White
            );
        }

        if ((tile.Direction & PathDirection.Right) != 0)
        {
            Raylib.DrawLineEx(
                new Vector2(centerX, centerY),
                new Vector2(px + TileSize - 5, centerY),
                2,
                Raylib_cs.Color.White
            );
        }

        if ((tile.Direction & PathDirection.Up) != 0)
        {
            Raylib.DrawLineEx(
                new Vector2(centerX, centerY),
                new Vector2(centerX, py + 5),
                2,
                Raylib_cs.Color.White
            );
        }

        if ((tile.Direction & PathDirection.Down) != 0)
        {
            Raylib.DrawLineEx(
                new Vector2(centerX, centerY),
                new Vector2(centerX, py + TileSize - 5),
                2,
                Raylib_cs.Color.White
            );
        }
    }
    public void UpdateTile(Vector2 mousePos, TileType newType) 
    {
        int x = (int)mousePos.X / TileSize;
        int y = (int)mousePos.Y / TileSize;
        if (x >= 0 && x < Cols && y >= 0 && y < Rows)
        {
            grid[x, y] = newType;
        }
    }
    public Vector2 GetTileAtMouse() //a fusioner
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        int x = (int)mousePos.X / TileSize;
        int y = (int)mousePos.Y / TileSize;
        return new Vector2(x, y);
    }

    public void SetPathTile(Vector2 mousePos, PathType pathType)
    {
        int x = (int)mousePos.X / TileSize;
        int y = (int)mousePos.Y / TileSize;
        
        if (x >= 0 && x < Cols && y >= 0 && y < Rows)
        {
            // Set the base tile to Path
            grid[x, y] = TileType.Path;
            
            // Add/update path tile
            PathManager.SetPathTile(x, y, pathType, PathDirection.None);
            
            // Auto-detect direction based on neighbors
            PathManager.AutoSetDirection(x, y);
            
            // Update neighboring tiles' directions
            PathManager.AutoSetDirection(x - 1, y);
            PathManager.AutoSetDirection(x + 1, y);
            PathManager.AutoSetDirection(x, y - 1);
            PathManager.AutoSetDirection(x, y + 1);
        }
    }
    
    public void RemovePathTile(Vector2 mousePos)
    {
        int x = (int)mousePos.X / TileSize;
        int y = (int)mousePos.Y / TileSize;
        
        if (x >= 0 && x < Cols && y >= 0 && y < Rows)
        {
            PathManager.RemovePathTile(x, y);
            
            // Update neighboring tiles' directions
            PathManager.AutoSetDirection(x - 1, y);
            PathManager.AutoSetDirection(x + 1, y);
            PathManager.AutoSetDirection(x, y - 1);
            PathManager.AutoSetDirection(x, y + 1);
        }
    }
    private string GetSavePath()
    {
        string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string saveDir = Path.Combine(baseDir, "TowerDefenseZero", "Maps");
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        return saveDir;
    }

    public void SaveLevel(string fileName)
    {
        string fullPath = Path.Combine(GetSavePath(), fileName + ".json");
        var data = new LevelData
        {
            LevelName = fileName,
            Width = Cols,
            Height = Rows,
            Tiles = new List<TileType>(),
            PathTiles = new List<PathTileData>()
        };
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Cols; x++)
                data.Tiles.Add(grid[x, y]);

        foreach (var pathTile in PathManager.GetAllPathTiles())
        {
            data.PathTiles.Add(new PathTileData
            {
                X = pathTile.X,
                Y = pathTile.Y,
                Type = pathTile.Type,
                Direction = pathTile.Direction
            });
        }
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
        Console.WriteLine($"Map saved to: {fullPath}");
    }

    public void LoadLevel(string fileName)
    {
        string fullPath = Path.Combine(GetSavePath(), fileName + ".json");
        if (!File.Exists(fullPath)) 
        {
            Console.WriteLine("Save file not found!");
            return;
        }
        string json = File.ReadAllText(fullPath);
        var data = JsonSerializer.Deserialize<LevelData>(json);
        if (data != null)
        {
            int index = 0;
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    grid[x, y] = data.Tiles[index++];
                }
            }
            foreach (var pathData in data.PathTiles)
            {
                PathManager.SetPathTile(pathData.X, pathData.Y, pathData.Type, pathData.Direction);
            }
        }
    }
    public void InitializeDefaultMap()
    {
        string fullPath = Path.Combine(GetSavePath(), "Default.json");
        if (File.Exists(fullPath)) 
        {
            return;
        }
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Cols; x++)
                    grid[x, y] = TileType.Grass;
        int middleRow = Rows / 2;
        for (int x = 0; x < Cols; x++)
        {
            grid[x, middleRow] = TileType.Path;
        }

        PathManager.SetPathTile(0, middleRow, PathType.Start, PathDirection.Right);
        PathManager.SetPathTile(Cols - 1, middleRow, PathType.End, PathDirection.Left);
        for (int x = 1; x < Cols - 1; x++)
        {
            PathManager.SetPathTile(x, middleRow, PathType.Path, PathDirection.Left | PathDirection.Right);
        }
        SaveLevel("Default");
    }
}