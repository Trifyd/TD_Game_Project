namespace TowerDefense.Enums;

[Flags]
public enum PathDirection
{
    None  = 0,
    Left  = 1 << 0,  // 1
    Up    = 1 << 1,  // 2
    Down  = 1 << 2,  // 4
    Right = 1 << 3   // 8
}

public static class PathDirectionExtensions
{
    public static int GetConnectionCount(this PathDirection direction)
    {
        int count = 0;
        if ((direction & PathDirection.Left) != 0) count++;
        if ((direction & PathDirection.Up) != 0) count++;
        if ((direction & PathDirection.Down) != 0) count++;
        if ((direction & PathDirection.Right) != 0) count++;
        return count;
    }

    public static bool IsValidConnectionCount(this PathDirection direction, int min = 1, int max = 3)
    {
        int count = direction.GetConnectionCount();
        return count >= min && count <= max;
    }
    public static PathDirection GetOpposite(this PathDirection direction)
    {
        return direction switch
        {
            PathDirection.Left => PathDirection.Right,
            PathDirection.Right => PathDirection.Left,
            PathDirection.Up => PathDirection.Down,
            PathDirection.Down => PathDirection.Up,
            _ => PathDirection.None
        };
    }
}