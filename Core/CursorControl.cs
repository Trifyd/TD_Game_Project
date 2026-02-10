namespace TowerDefense.Core;
using System.Numerics;
using TowerDefense.Enums;
using Raylib_cs;


public class CursorControl
{
    public Vector2 ScreenPos { get; private set; }
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    private int _tileSize;
    public CursorControl(int tileSize) // init cursor control
    {
        _tileSize = tileSize;
        Raylib.HideCursor();
    }
    public void Update() // update cursor position
    {
        ScreenPos = Raylib.GetMousePosition();
        GridX = (int)ScreenPos.X / _tileSize;
        GridY = (int)ScreenPos.Y / _tileSize;
    }
    public Vector2 GetTilePosition() // mouse the tile is hovering
    {
        return new Vector2(GridX, GridY);
    }
    public bool IsInBounds(int maxCols, int maxRows) // is the cursor in the game window
    {
        return GridX >= 0 && GridX < maxCols && GridY >= 0 && GridY < maxRows;
    }
    public void DrawHoverPreview(GameState state, TileType currentBrush) // cursor brush tile hover preview
    {
        if (state == GameState.Editor)
        {
            Color previewColor = GetColorForTile(currentBrush);
            Raylib.DrawRectangle(GridX * _tileSize, GridY * _tileSize, _tileSize, _tileSize, Raylib.Fade(previewColor, 0.5f));
        }
        else if (state == GameState.Playing)
        {
            Raylib.DrawRectangleLines(GridX * _tileSize, GridY * _tileSize, _tileSize, _tileSize, Color.SkyBlue);
        }
        Raylib.DrawCircleV(ScreenPos, 4, Color.White);
    }
    private static Color GetColorForTile(TileType type) // tile type colors
    {
        return type switch
        {
            TileType.Grass => Color.DarkGreen,
            TileType.Path  => Color.Gray,
            TileType.Wall  => Color.Brown,
            TileType.Water => Color.Blue,
            TileType.Lava  => Color.Red,
            _              => Color.White
        };
    }
}