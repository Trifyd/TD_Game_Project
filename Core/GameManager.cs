namespace TowerDefense.Core;
using Raylib_cs;
using System.Numerics;
using TowerDefense.Enums;
using TowerDefense.Levels;

public class GameManager
{
    public GameState CurrentState { get; private set; } = GameState.Menu;
    public TileType CurrentBrush { get; private set; } = TileType.Grass;
    private readonly MapManager _mapManager;
    private readonly CursorControl _cursor;
    private readonly Button _btnStart;
    private readonly Button _btnEditor;
    private readonly Button _btnExit;
    private readonly PopupManager _popupManager;
    public GameManager(MapManager mapManager, CursorControl cursor, Button btnStart, Button btnEditor, Button btnExit)  // init gamemanger
    {
        _mapManager = mapManager;
        _mapManager.InitializeDefaultMap();
        _cursor = cursor;
        _btnStart = btnStart;
        _btnEditor = btnEditor;
        _btnExit = btnExit;
        _popupManager = new PopupManager(); // init Popup manager
        Raylib.SetExitKey(KeyboardKey.Equal); // change exit key
    }
    public bool Update()
    {
        if (_popupManager.IsVisible)
        {
            _popupManager.Update();
            return true;
        }
        _cursor.Update();
        switch (CurrentState)
        {
            case GameState.Menu:
                return HandleMenuInput();
            case GameState.Editor:
                HandleEditorInput();
                break;
            case GameState.Playing:
                HandlePlayingInput();
                break;
        }
        return true;
    }
    public void HandlePlayingInput() // playing state input
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mouseTile = _cursor.GetTilePosition();
            if (mouseTile.X >= 0 && mouseTile.X < _mapManager.Cols && 
                mouseTile.Y >= 0 && mouseTile.Y < _mapManager.Rows)
            {
                // This were the tower placement logic and pop-up will happend
            }
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            _popupManager.ShowConfirmation(
                "Return to Menu",
                "Are you sure you want to exit the game?",
                () => CurrentState = GameState.Menu,
                () => { }
            );
        }
    }
    private bool HandleMenuInput() // start menu input
    {
        if (_btnStart.IsClicked())
        {
            _popupManager.ShowMapInput(
                "Default",
                (mapName)=>
                {
                    if (!string.IsNullOrWhiteSpace(mapName)) //dynamic map choice
                    {
                        _mapManager.LoadLevel(mapName);
                        _popupManager.Show(
                            "Success",
                            $"Map '{mapName}' loaded successfully!",
                            new PopupButton("OK", Color.DarkGreen, () => { })
                        );
                    }
                },
                () => { } 
            );
            CurrentState = GameState.Playing;
        }
        else if (_btnEditor.IsClicked())
        {
            _popupManager.ShowMapInput(
                "Default",
                (mapName)=>
                {
                    if (!string.IsNullOrWhiteSpace(mapName)) //dynamic map choice
                    {
                        _mapManager.LoadLevel(mapName);
                        _popupManager.Show(
                            "Success",
                            $"Map '{mapName}' loaded successfully!",
                            new PopupButton("OK", Color.DarkGreen, () => { })
                        );
                    }
                },
                () => { } 
            );
            CurrentState = GameState.Editor;
        }
        else if (_btnExit.IsClicked()) // exit
        {
            return false;
        }
        return true;
    }
    private void HandleEditorInput() // editor state input
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left)) // chosen tile in brush
        {
            _mapManager.UpdateTile(_cursor.ScreenPos, CurrentBrush);
        }
        else if (Raylib.IsMouseButtonDown(MouseButton.Right)) // default tile
        {
            _mapManager.UpdateTile(_cursor.ScreenPos, TileType.Grass);
        }
        float wheel = Raylib.GetMouseWheelMove(); // brush tile change
        if (wheel != 0)
        {
            CycleBrush(wheel);
        }
        if (Raylib.IsKeyPressed(KeyboardKey.S)) // save popup
        {
            _popupManager.ShowSaveInput(
                "Default",
                (mapName) => 
                {
                    if (!string.IsNullOrWhiteSpace(mapName)) // dynamic name saving
                    {
                        _mapManager.SaveLevel(mapName);
                        _popupManager.Show(
                            "Success",
                            $"Map '{mapName}' saved successfully!",
                            new PopupButton("OK", Color.DarkGreen, () => { })
                        );
                    }
                },
                () => { } 
            );
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Escape)) // pause popup
        {
            _popupManager.ShowConfirmation(
                "Return to Menu",
                "Are you sure? Unsaved changes will be lost.",
                () => CurrentState = GameState.Menu,
                () => { }
            );
        }
    }
    private void CycleBrush(float wheelDirection) // brush choice function
    {
        var tileTypes = Enum.GetValues<TileType>();
        int currentIndex = Array.IndexOf(tileTypes, CurrentBrush);
        if (wheelDirection > 0)
        {
            currentIndex = (currentIndex + 1) % tileTypes.Length;
        }
        else if (wheelDirection < 0)
        {
            currentIndex = (currentIndex - 1 + tileTypes.Length) % tileTypes.Length;
        }
        CurrentBrush = tileTypes[currentIndex];
    }
    public void Draw() // display by state
    {
        switch (CurrentState)
        {
            case GameState.Menu:
                DrawMenu();
                break;
            case GameState.Playing:
                DrawPlayingState();
                break;
            case GameState.Editor:
                DrawEditorState();
                break;
        }
        _popupManager.Draw();
    }
    private void DrawMenu() // menu state display
    {
        Raylib.ClearBackground(Color.DarkBlue);
        _cursor.DrawHoverPreview(CurrentState, CurrentBrush);
        string title = "TOWER DEFENSE: ZERO";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        int titleX = (800 - titleWidth) / 2;
        
        Raylib.DrawText(title, titleX, 100, titleFontSize, Color.Yellow);
        
        _btnStart.Draw();
        _btnEditor.Draw();
        _btnExit.Draw();
    }
    private void DrawPlayingState() // playing state display
    {
        _mapManager.DrawGrid();
        _cursor.DrawHoverPreview(CurrentState, CurrentBrush);
        Vector2 mouseTile = _cursor.GetTilePosition();
        int tileSize = _mapManager.TileSize;
        
        if (mouseTile.X >= 0 && mouseTile.X < _mapManager.Cols && 
            mouseTile.Y >= 0 && mouseTile.Y < _mapManager.Rows)
        {
            Raylib.DrawRectangle(
                (int)mouseTile.X * tileSize,
                (int)mouseTile.Y * tileSize,
                tileSize,
                tileSize,
                Raylib.Fade(Color.SkyBlue, 0.5f)
            );
        }
    }
    private void DrawEditorState() // editor state display
    {
        _mapManager.DrawGrid();
        _cursor.DrawHoverPreview(CurrentState, CurrentBrush);
        DrawEditorUI();
    }
    private void DrawEditorUI() // editor state instruction and brush
    {
        Raylib.DrawRectangle(5, 5, 200, 30, Raylib.Fade(Color.Black, 0.7f));
        string brushText = $"Brush: {CurrentBrush}";
        Raylib.DrawText(brushText, 10, 10, 20, Color.White);
        Raylib.DrawRectangle(0, 540, 800, 60, Raylib.Fade(Color.Black, 0.7f));
        Raylib.DrawText("Left Click - Paint | Right Click - Erase | Scroll - Change Brush", 10, 545, 16, Color.White);
        Raylib.DrawText("S - Save | ESC - Menu", 10, 565, 16, Color.White);
    }
}