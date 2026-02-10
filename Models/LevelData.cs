namespace TowerDefense.Models;
using TowerDefense.Enums;

public class LevelData
{
    public string LevelName { get; set; } = "New Level";
    public int Width { get; set; }
    public int Height { get; set; }
    public List<TileType> Tiles { get; set; } = new(); // A flat list of all tiles
    public List<PathTileData> PathTiles { get; set; } = new();
}
public class PathTileData
{
    public int X { get; set; }
    public int Y { get; set; }
    public PathType Type { get; set; }
    public PathDirection Direction { get; set; }
}