using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapController : MonoBehaviour
{
    public GameObject bluePrefab, greenPrefab, blackPrefab;
    public int width = 10;
    public int height = 10;
    public int depth = 1;
    public float cellSize = 1f;
    public int numPaths = 5; // Number of black paths
    public int[,,] grid;
    private bool[,,] collapsed;
    private List<int>[,,] possibilities;
    private Dictionary<int, List<int>> rules;
    public List<Vector3> blackPositions = new List<Vector3>();
    public List<Vector3> pathStartPoints = new List<Vector3>(); // New list for path start points
    public GameObject[,,] tiles;
    public List<List<(int, int)>> paths = new List<List<(int, int)>>();

    void Start()
    {
        InitializeRules();
        InitializeGrid();
        GeneratePaths();
        GenerateGrid();
    }

    void InitializeRules()
    {
        rules = new Dictionary<int, List<int>>();
        // 0: blue, 1: green, 2: black
        rules[0] = new List<int> { 0, 1, 2 };
        rules[1] = new List<int> { 0, 1, 2 };
        rules[2] = new List<int> { 1, 2 }; // black next to green or black to form paths
    }

    void GeneratePaths()
    {
        System.Collections.Generic.HashSet<(int, int)> used = new System.Collections.Generic.HashSet<(int, int)>();
        pathStartPoints.Clear(); // Clear previous start points

        for (int p = 0; p < numPaths; p++)
        {
            int startY = Random.Range(0, height);
            int endY = Random.Range(0, height);
            List<(int, int)> path = new List<(int, int)>();
            int currentX = 0;
            int currentY = startY;

            // Store the start point of this path
            Vector3 startPoint = new Vector3(currentX * cellSize, 0, currentY * cellSize);
            pathStartPoints.Add(startPoint);
            Debug.Log($"Path {p} starts at: {startPoint}");

            path.Add((currentX, currentY));
            used.Add((currentX, currentY));
            while (currentX < width - 1)
            {
                currentX++;
                int dy = 0;
                if (Random.value > 0.6f) // 40% chance to change direction
                {
                    dy = Random.Range(-1, 2);
                }
                currentY = Mathf.Clamp(currentY + dy, 0, height - 1);
                if (!used.Contains((currentX, currentY)))
                {
                    path.Add((currentX, currentY));
                    used.Add((currentX, currentY));
                }
                else
                {
                    // If overlap, try again or just add anyway
                    path.Add((currentX, currentY));
                }
            }
            // Ensure end is connected
            if (currentY != endY)
            {
                // Simple adjust last few
            }
            paths.Add(new List<(int, int)>(path));
            // Set path to black
            foreach (var pos in path)
            {
                grid[pos.Item1, pos.Item2, 0] = 2;
                collapsed[pos.Item1, pos.Item2, 0] = true;
                possibilities[pos.Item1, pos.Item2, 0] = new List<int> { 2 };
                blackPositions.Add(new Vector3(pos.Item1 * cellSize, 0, pos.Item2 * cellSize));
            }
        }

        Debug.Log($"Generated {numPaths} paths with {pathStartPoints.Count} start points");
    }

    void InitializeGrid()
    {
        grid = new int[width, height, depth];
        tiles = new GameObject[width, height, depth];
        collapsed = new bool[width, height, depth];
        possibilities = new List<int>[width, height, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Weighted: blue 3, green 3, black 1
                    possibilities[x, y, z] = new List<int> { 0, 0, 0, 1, 1, 1, 2 };
                }
            }
        }
        // Force at least 3 random black starts on left and right edges
        System.Collections.Generic.HashSet<int> leftYs = new System.Collections.Generic.HashSet<int>();
        System.Collections.Generic.HashSet<int> rightYs = new System.Collections.Generic.HashSet<int>();
        while (leftYs.Count < 3)
        {
            leftYs.Add(Random.Range(0, height));
        }
        while (rightYs.Count < 3)
        {
            rightYs.Add(Random.Range(0, height));
        }
        foreach (int y in leftYs)
        {
            possibilities[0, y, 0] = new List<int> { 2 };
        }
        foreach (int y in rightYs)
        {
            possibilities[width - 1, y, 0] = new List<int> { 2 };
        }
    }

    void GenerateGrid()
    {
        List<(int, int, int)> toCollapse = new List<(int, int, int)>();
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    toCollapse.Add((x, y, z));

        while (toCollapse.Count > 0)
        {
            // Find cell with least possibilities
            (int, int, int)? minCell = null;
            int minCount = int.MaxValue;
            foreach (var cell in toCollapse)
            {
                if (possibilities[cell.Item1, cell.Item2, cell.Item3].Count < minCount)
                {
                    minCount = possibilities[cell.Item1, cell.Item2, cell.Item3].Count;
                    minCell = cell;
                }
            }
            if (minCell == null) break;

            // Collapse
            int chosen = possibilities[minCell.Value.Item1, minCell.Value.Item2, minCell.Value.Item3][Random.Range(0, possibilities[minCell.Value.Item1, minCell.Value.Item2, minCell.Value.Item3].Count)];
            grid[minCell.Value.Item1, minCell.Value.Item2, minCell.Value.Item3] = chosen;
            collapsed[minCell.Value.Item1, minCell.Value.Item2, minCell.Value.Item3] = true;
            toCollapse.Remove(minCell.Value);

            // Propagate
            Propagate(minCell.Value.Item1, minCell.Value.Item2, minCell.Value.Item3);
        }

        // Instantiate
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject prefab = null;
                    switch (grid[x, y, z])
                    {
                        case 0: prefab = bluePrefab; break;
                        case 1: prefab = greenPrefab; break;
                        case 2: prefab = blackPrefab; break;
                    }
                    tiles[x, y, z] = Instantiate(prefab, new Vector3(x * cellSize, z * cellSize, y * cellSize), Quaternion.identity);
                }
            }
        }
    }

    void Propagate(int x, int y, int z)
    {
        int[] dx = { -1, 0, 1, 0, 0, 0 };
        int[] dy = { 0, 1, 0, -1, 0, 0 };
        int[] dz = { 0, 0, 0, 0, 1, -1 };
        for (int d = 0; d < 6; d++)
        {
            int nx = x + dx[d];
            int ny = y + dy[d];
            int nz = z + dz[d];
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && nz >= 0 && nz < depth && !collapsed[nx, ny, nz])
            {
                var allowed = rules[grid[x, y, z]];
                possibilities[nx, ny, nz] = possibilities[nx, ny, nz].FindAll(p => allowed.Contains(p)).ToList();
                if (possibilities[nx, ny, nz].Count == 0)
                {
                    // Contradiction, but for simplicity, ignore or reset
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        foreach (var pos in blackPositions)
        {
            Gizmos.DrawCube(pos, Vector3.one * cellSize * 0.8f);
        }

        // Draw path start points in red
        Gizmos.color = Color.red;
        foreach (var startPoint in pathStartPoints)
        {
            Gizmos.DrawSphere(startPoint, cellSize * 0.3f);
        }
    }

    // Get a specific path by index for enemies to follow
    public List<Vector3> GetPathByIndex(int pathIndex)
    {
        if (pathIndex < 0 || pathIndex >= paths.Count) return new List<Vector3>();

        List<Vector3> pathPositions = new List<Vector3>();
        foreach (var gridPos in paths[pathIndex])
        {
            pathPositions.Add(new Vector3(gridPos.Item1 * cellSize, 0, gridPos.Item2 * cellSize));
        }
        return pathPositions;
    }

    // Get all path start points for spawning
    public List<Vector3> GetPathStartPoints()
    {
        return new List<Vector3>(pathStartPoints);
    }
}
