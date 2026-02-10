namespace TowerDefense.Core;
using Raylib_cs;
using System.Numerics;

public class PopupManager
{
    private bool _isVisible;
    private string _title;
    private string _message;
    private List<PopupButton> _buttons;
    private Rectangle _popupRect;
    private Color _backgroundColor;
    private Color _titleColor;
    private bool _hasInputField;
    private string _inputText;
    private Rectangle _inputRect;
    private bool _inputActive;
    private int _maxInputLength;
    public bool IsVisible => _isVisible;
    public string InputText => _inputText;
    // init popup
    public PopupManager()
    {
        _isVisible = false;
        _title = "";
        _message = "";
        _buttons = new List<PopupButton>();
        _backgroundColor = new Color(30, 30, 30, 240);
        _titleColor = Color.Yellow;
        _hasInputField = false;
        _inputText = "";
        _inputActive = false;
        _maxInputLength = 30;
    }
    public void Show(string title, string message, params PopupButton[] buttons) // base popup
    {
        Show(title, message, false, "", buttons);
    }
    public void Show(string title, string message, bool hasInputField, string defaultInputText, params PopupButton[] buttons) //dynamic popup
    {
        _isVisible = true;
        _title = title;
        _message = message;
        _buttons = new List<PopupButton>(buttons);
        _hasInputField = hasInputField;
        _inputText = defaultInputText;
        _inputActive = hasInputField;
        int popupWidth = 400;
        int popupHeight = 200 + (_buttons.Count * 60);
        if (_hasInputField)
        {
            popupHeight += 60;
        }
        int popupX = (800 - popupWidth) / 2;
        int popupY = (600 - popupHeight) / 2;
        _popupRect = new Rectangle(popupX, popupY, popupWidth, popupHeight);
        if (_hasInputField)
        {
            _inputRect = new Rectangle(
                popupX + 50,
                popupY + 120,
                popupWidth - 100,
                40
            );
        }
        float buttonY = popupY + (_hasInputField ? 180 : 120);
        foreach (var button in _buttons)
        {
            button.SetPosition(popupX + 50, buttonY);
            buttonY += 60;
        }
    }
    public void ShowSaveInput(string defaultName, Action<string> onSave, Action onCancel) // save popup
    {
        Show(
            "Save Map",
            "Enter a name for your map:",
            true,
            defaultName,
            new PopupButton("Save", Color.DarkGreen, () => onSave(_inputText)),
            new PopupButton("Cancel", Color.Maroon, onCancel)
        );
    }
    public void ShowMapInput(string defaultName, Action<string> onLoad, Action onCancel) // map selection popup
    {
        Show(
            "Choose a Map",
            "Enter the map name:",
            true,
            defaultName,
            new PopupButton("Go",Color.DarkGreen, () => onLoad(_inputText)),
            new PopupButton("Cancel", Color.Maroon, onCancel)
        );
    }
    public void ShowTowerSelection(Action<string> onTowerSelected) // tower selection popup
    {
        
    }
    public void ShowUpgradeTowerSelection(Action<string> onTowerUpraded) // tower upgrade popup
    {
        
    }
    public void ShowError(string errorMessage) // error popup (will be upgraded when loggin system is in place)
    {
        Show(
            "Error",
            errorMessage,
            new PopupButton("OK", Color.Maroon, () => Hide())
        );
    }
    public void ShowConfirmation(string title, string message, Action onYes, Action onNo) // confirmation popup
    {
        Show(
            title,
            message,
            new PopupButton("Yes", Color.DarkGreen, onYes),
            new PopupButton("No", Color.Maroon, onNo)
        );
    }
    public void Hide() // hide popup
    {
        _isVisible = false;
        _hasInputField = false;
        _inputText = "";
        _inputActive = false;
    }
    public void Update()
    {
        if (!_isVisible) return;

        if (_hasInputField && _inputActive)
        {
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if ((key >= 32) && (key <= 125) && _inputText.Length < _maxInputLength)
                {
                    _inputText += (char)key;
                }
                key = Raylib.GetCharPressed();
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _inputText.Length > 0)
            {
                _inputText = _inputText.Substring(0, _inputText.Length - 1);
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && _buttons.Count > 0)
            {
                _buttons[0].OnClick?.Invoke();
                Hide();
                return;
            }
        }
        foreach (var button in _buttons)
        {
            if (button.IsClicked())
            {
                button.OnClick?.Invoke();
                Hide();
                break;
            }
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            if (_hasInputField && _buttons.Count > 1)
            {
                // Trigger cancel button (usually second button)
                _buttons[1].OnClick?.Invoke();
            }
            Hide();
        }
    }
    public void Draw() // popup display
    {
        if (!_isVisible) return;
        // Darken background
        Raylib.DrawRectangle(0, 0, 800, 600, new Color(0, 0, 0, 180));
        // popup background
        Raylib.DrawRectangleRec(_popupRect, _backgroundColor);
        Raylib.DrawRectangleLinesEx(_popupRect, 3, Color.White);
        int titleFontSize = 30;
        int titleWidth = Raylib.MeasureText(_title, titleFontSize);
        int titleX = (int)(_popupRect.X + (_popupRect.Width - titleWidth) / 2);
        Raylib.DrawText(_title, titleX, (int)_popupRect.Y + 20, titleFontSize, _titleColor);
        // message
        int messageFontSize = 20;
        DrawWrappedText(_message, (int)_popupRect.X + 20, (int)_popupRect.Y + 70, 
                       (int)_popupRect.Width - 40, messageFontSize, Color.White);
        if (_hasInputField)
        {
            Raylib.DrawRectangleRec(_inputRect, Color.White);
            Raylib.DrawRectangleLinesEx(_inputRect, 2, _inputActive ? Color.SkyBlue : Color.Gray);
            string displayText = _inputText;
            if (_inputActive && ((int)(Raylib.GetTime() * 2) % 2 == 0))
            {
                displayText += "_";
            }
            Raylib.DrawText(displayText, (int)_inputRect.X + 10, (int)_inputRect.Y + 10, 20, Color.Black);
        }
        foreach (var button in _buttons)
        {
            button.Draw();
        }
    }
    private void DrawWrappedText(string text, int x, int y, int maxWidth, int fontSize, Color color) // text return line if too long
    {
        string[] words = text.Split(' ');
        string currentLine = "";
        int currentY = y;

        foreach (string word in words)
        {
            string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
            int lineWidth = Raylib.MeasureText(testLine, fontSize);
            if (lineWidth > maxWidth && currentLine.Length > 0)
            {
                Raylib.DrawText(currentLine, x, currentY, fontSize, color);
                currentLine = word;
                currentY += fontSize + 5;
            }
            else
            {
                currentLine = testLine;
            }
        }
        if (currentLine.Length > 0)
        {
            Raylib.DrawText(currentLine, x, currentY, fontSize, color);
        }
    }
}

public class PopupButton
{
    private Rectangle _bounds;
    private readonly string _text;
    private readonly Color _baseColor;
    private Color _hoverColor;
    private bool _isHovered;
    
    public Action? OnClick { get; set; }

    public PopupButton(string text, Color baseColor, Action? onClick = null) //init popup buttons
    {
        _text = text;
        _baseColor = baseColor;
        _hoverColor = Raylib.ColorBrightness(baseColor, 0.2f);
        _bounds = new Rectangle(0, 0, 300, 50);
        OnClick = onClick;
    }
    public void SetPosition(float x, float y) //button position 
    {
        _bounds.X = x;
        _bounds.Y = y;
    }
    public bool IsClicked() // button action
    {
        Vector2 mousePoint = Raylib.GetMousePosition();
        _isHovered = Raylib.CheckCollisionPointRec(mousePoint, _bounds);
        return _isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }
    public void Draw() // display popup button
    {
        Raylib.DrawRectangleRec(_bounds, _isHovered ? _hoverColor : _baseColor);
        Raylib.DrawRectangleLinesEx(_bounds, 2, Color.Black);
        
        int fontSize = 20;
        int textWidth = Raylib.MeasureText(_text, fontSize);
        float textX = _bounds.X + (_bounds.Width - textWidth) / 2;
        float textY = _bounds.Y + (_bounds.Height - fontSize) / 2;
        
        Raylib.DrawText(_text, (int)textX, (int)textY, fontSize, Color.White);
    }
}
