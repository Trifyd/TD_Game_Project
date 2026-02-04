namespace TowerDefense.Levels;
using Raylib_cs;
using System.Text.Json;
using System.IO;
using System.Numerics;
using TowerDefense.Enums;
using TowerDefense.Models;
using System.Drawing;

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
    }
    public void UpdateTile(Vector2 mousePos, TileType newType) //a fusioner
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
            Tiles = new List<TileType>()
        };
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Cols; x++)
                data.Tiles.Add(grid[x, y]);
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
        SaveLevel("Default");
    }
}