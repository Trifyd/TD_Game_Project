namespace TowerDefense.Models;
using TowerDefense.Enums;

public class LevelData
{
    public string LevelName { get; set; } = "New Level";
    public int Width { get; set; }
    public int Height { get; set; }
    // A flat list of all tiles
    public List<TileType> Tiles { get; set; } = new();
}