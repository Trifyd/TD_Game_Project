namespace TowerDefense.Levels;
using TowerDefense.Enums;
using TowerDefense.Models;
using System.Collections.Generic;
using System.Linq;

public class PathManager
{
    private Dictionary<(int x, int y), PathTile> pathTiles;
    private List<PathTile> startTiles;
    private List<PathTile> endTiles;
    public IReadOnlyList<PathTile> StartTiles => startTiles.AsReadOnly();
    public IReadOnlyList<PathTile> EndTiles => endTiles.AsReadOnly();
    public PathManager() // init pathmanager
    {
        pathTiles = new Dictionary<(int, int), PathTile>();
        startTiles = new List<PathTile>();
        endTiles = new List<PathTile>();
    }
    public void SetPathTile(int x, int y, PathType type, PathDirection direction) // set new path data to tile
    {
        var key = (x, y);
        if (pathTiles.TryGetValue(key, out var existingTile))
        {
            startTiles.Remove(existingTile);
            endTiles.Remove(existingTile);
        }
        if (type == PathType.None)
        {
            pathTiles.Remove(key);
            return;
        }
        var tile = new PathTile(x, y, type, direction);
        pathTiles[key] = tile;
        if (type == PathType.Start)
            startTiles.Add(tile);
        else if (type == PathType.End)
            endTiles.Add(tile);
    }
    public PathTile? GetPathTile(int x, int y) // get path data from tile
    {
        pathTiles.TryGetValue((x, y), out var tile);
        return tile;
    }
    public void RemovePathTile(int x, int y) // remove path data from tile
    {
        SetPathTile(x, y, PathType.None, PathDirection.None);
    }
    public void AutoSetDirection(int x, int y) // auto set path direction
    {
        var tile = GetPathTile(x, y);
        if (tile == null || tile.Type == PathType.None)
            return;
        PathDirection newDirection = PathDirection.None;
        var leftTile = GetPathTile(x - 1, y);
        var rightTile = GetPathTile(x + 1, y);
        var upTile = GetPathTile(x, y - 1);
        var downTile = GetPathTile(x, y + 1);
        if (leftTile != null && leftTile.Type != PathType.None)
            newDirection |= PathDirection.Left;
        if (rightTile != null && rightTile.Type != PathType.None)
            newDirection |= PathDirection.Right;
        if (upTile != null && upTile.Type != PathType.None)
            newDirection |= PathDirection.Up;
        if (downTile != null && downTile.Type != PathType.None)
            newDirection |= PathDirection.Down;
        SetPathTile(x, y, tile.Type, newDirection);
    }
    public bool ValidateAllTiles() // validate path data from all tile
    {
        foreach (var tile in pathTiles.Values)
        {
            if (!tile.IsValid())
                return false;
        }
        return true;
    }
    public List<PathTile>? FindPath(PathTile start, PathTile end) // find valid path
    {
        if (start == null || end == null)
            return null;
        var queue = new Queue<(PathTile tile, List<PathTile> path)>();
        var visited = new HashSet<(int, int)>();
        queue.Enqueue((start, new List<PathTile> { start }));
        visited.Add((start.X, start.Y));
        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();
            if (current.X == end.X && current.Y == end.Y)
                return path;
            var neighbors = new[]
            {
                (current.X - 1, current.Y, PathDirection.Left),
                (current.X + 1, current.Y, PathDirection.Right),
                (current.X, current.Y - 1, PathDirection.Up),
                (current.X, current.Y + 1, PathDirection.Down)
            };
            foreach (var (nx, ny, dir) in neighbors)
            {
                if ((current.Direction & dir) == 0)
                    continue;
                var neighbor = GetPathTile(nx, ny);
                if (neighbor == null || visited.Contains((nx, ny)))
                    continue;
                if ((neighbor.Direction & dir.GetOpposite()) == 0)
                    continue;
                visited.Add((nx, ny));
                var newPath = new List<PathTile>(path) { neighbor };
                queue.Enqueue((neighbor, newPath));
            }
        }
        return null; // No path found
    }
    public List<List<PathTile>> GetAllValidPaths() // find all valid path
    {
        var allPaths = new List<List<PathTile>>();
        foreach (var start in startTiles)
        {
            foreach (var end in endTiles)
            {
                var path = FindPath(start, end);
                if (path != null)
                    allPaths.Add(path);
            }
        }
        return allPaths;
    }
    public void Clear() // clear all tile data
    {
        pathTiles.Clear();
        startTiles.Clear();
        endTiles.Clear();
    }
    public IEnumerable<PathTile> GetAllPathTiles() // get all tile with path data
    {
        return pathTiles.Values;
    }
}