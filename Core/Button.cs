namespace TowerDefense.Core;
using Raylib_cs;
using System.Numerics;

public class Button(float x, float y, float width, float height, string text, Color baseColor)
{
    private Rectangle bounds = new Rectangle(x, y, width, height);
    private readonly string text = text;
    private Color color = baseColor;
    private Color hoverColor = Raylib.ColorBrightness(baseColor, 0.2f);
    private bool isHovered;
    public bool IsClicked() // click action
    {
        Vector2 mousePoint = Raylib.GetMousePosition();
        isHovered = Raylib.CheckCollisionPointRec(mousePoint, bounds);
        return isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }
    public void Draw() // drawn button
    {
        Raylib.DrawRectangleRec(bounds, isHovered ? hoverColor : color);
        Raylib.DrawRectangleLinesEx(bounds, 2, Color.Black);
        int fontSize = 20;
        int textWidth = Raylib.MeasureText(text, fontSize);
        float textX = bounds.X + (bounds.Width - textWidth) / 2;
        float textY = bounds.Y + (bounds.Height - fontSize) / 2;
        Raylib.DrawText(text, (int)textX, (int)textY, fontSize, Color.White);
    }
}