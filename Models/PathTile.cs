namespace TowerDefense.Models;
using TowerDefense.Enums;

public class PathTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public PathType Type { get; set; }
    public PathDirection Direction { get; set; }

    public PathTile(int x, int y, PathType type = PathType.None, PathDirection direction = PathDirection.None)
    {
        X = x;
        Y = y;
        Type = type;
        Direction = direction;
    }

    public bool IsValid()
    {
        if (Type == PathType.None)
            return true;

        int connectionCount = Direction.GetConnectionCount();

        return Type switch
        {
            PathType.Start => connectionCount >= 1 && connectionCount <= 3,
            PathType.End => connectionCount >= 1 && connectionCount <= 3,
            PathType.Path => connectionCount >= 1 && connectionCount <= 3,
            _ => false
        };
    }

    public bool ConnectsTo(PathTile other)
    {
        if (other == null) return false;

        int dx = other.X - X;
        int dy = other.Y - Y;

        if (Math.Abs(dx) + Math.Abs(dy) != 1)
            return false;

        PathDirection directionToOther = PathDirection.None;
        if (dx == 1) directionToOther = PathDirection.Right;
        else if (dx == -1) directionToOther = PathDirection.Left;
        else if (dy == 1) directionToOther = PathDirection.Down;
        else if (dy == -1) directionToOther = PathDirection.Up;

        bool thisHasOutput = (Direction & directionToOther) != 0;
        bool otherHasInput = (other.Direction & directionToOther.GetOpposite()) != 0;

        return thisHasOutput && otherHasInput;
    }

    public override string ToString()
    {
        return $"PathTile({X},{Y}) Type:{Type} Dir:{Direction}";
    }
}