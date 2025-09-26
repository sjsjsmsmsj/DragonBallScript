using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerMove : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float lifeTimeClickEffect;
    [SerializeField] private float speedCamera;
    private int cellsize = 2;
    private WarriorManager warriorManager;
    private LineupManager lineupManager;
    private Vector3 newPos;
    private int currentIndex;
    private float timeDelayclick = 0.1f;
    private float timeNextClick;
    public Camera cam { get; private set; }
    public bool isMove {get; set;}
    public static event Action PlayerMove;

    private void Start()
    {
        isMove = false;
        warriorManager = GetComponent<WarriorManager>();
        lineupManager = GetComponent<LineupManager>();
        lineupManager.ProtectedFormation(warriorManager.GetWarriorList(), transform.position);

        cam = gameManager.cam;

    }
    private void Update()
    {
        if (isMove)
        {
            GoToNewPos();
        }
    }
    public void HandleMove()
    {
        if (IsMouseOutSideGame()) return;
        if (Time.time < timeNextClick) return;
        timeNextClick = Time.time + timeDelayclick;
        PlayerMove?.Invoke();
        isMove = true;
        newPos = cam.ScreenToWorldPoint(Input.mousePosition);
        gameManager.CreateEffectAtPos(EffectManager.CLICK, newPos, lifeTimeClickEffect);
        lineupManager.FormationMoveToNewPos(warriorManager.GetWarriorList(), newPos);
    }
    private void GoToNewPos()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * speedCamera);
    }
    private bool IsMouseOutSideGame()
    {
        Vector3 viewportPosMouse = cam.ScreenToViewportPoint(Input.mousePosition);
        return viewportPosMouse.x < 0 || viewportPosMouse.x > 1 ||
               viewportPosMouse.y < 0 || viewportPosMouse.y > 1;
    }
    public Vector3 GetNewPos()
    {
        return newPos;
    }
}

public class FindPathManager
{
    public int cell_x;
    public int cell_y;
    private float heightCam;
    private float widthCam;
    private int gridCols;
    private int gridRows;
    private Vector3 topLeft;

    private Camera cam;
    private int layerMask;
    public FindPathManager()
    {
        layerMask = LayerMask.GetMask("Building");
    }
    public FindPathManager(int cellSizeX, int cellSizeY, Camera cam, string layerName)
    {
        cell_x = cellSizeX;
        cell_y = cellSizeY;
        this.cam = cam;
        heightCam = cam.orthographicSize * 2f;
        widthCam = heightCam * cam.aspect;
        gridCols = Mathf.CeilToInt(widthCam / cell_x);
        gridRows = Mathf.CeilToInt(heightCam / cell_y);
        layerMask = LayerMask.GetMask(layerName);
    }
    public FindPathManager(float cellSizeX, float cellSizeY, Camera cam, string layerName)
    {
        cell_x = Mathf.CeilToInt(cellSizeX);
        cell_y = Mathf.CeilToInt(cellSizeY);
        this.cam = cam;
        heightCam = cam.orthographicSize * 2f;
        widthCam = heightCam * cam.aspect;
        gridCols = Mathf.CeilToInt(widthCam / cell_x);
        gridRows = Mathf.CeilToInt(heightCam / cell_y);
        layerMask = LayerMask.GetMask(layerName);
    }
    public List<Vector3> GetPath(Vector3 begin, Vector3 end, int radiusTeam)
    {
        Vector3 camPos = cam.transform.position;
        topLeft = new Vector3(camPos.x - widthCam / 2, camPos.y + heightCam / 2);
        float distance = Vector3.Distance(begin, end);
        RaycastHit2D[] hits = Physics2D.RaycastAll(begin, (end - begin).normalized, distance, layerMask);
        List<Vector3> path;
        if (hits.Length == 0)
        {
            path = new List<Vector3>();
            Vector3 goall = new Vector3(end.x, end.y);
            path.Add(begin);
            path.Add(goall);
            return path;
        }
        int[,] grid = CreateGrid(hits);
        Vector2Int start = Pos2GridIndex(begin);
        Vector2Int goal = Pos2GridIndex(end);

        // Check start / goal bounds using gridCols (x) and gridRows (y)
        if (start.x < 0 || start.x >= gridCols || start.y < 0 || start.y >= gridRows)
        {
            Debug.Log("Start out of bounds");
            return new List<Vector3>();
        }
        if (goal.x < 0 || goal.x >= gridCols || goal.y < 0 || goal.y >= gridRows)
        {
            Debug.Log("Goal out of bounds");
            return new List<Vector3>();
        }
        path = FindPath(grid, start, goal);
        if (path.Count == 0)
        {
            Debug.Log("path == 0");
            return new List<Vector3>();
        }
        return path;
    }

    public int[,] CreateGrid(RaycastHit2D[] hits)
    {
        int[,] grid = new int[gridCols, gridRows]; // [x:cols, y:rows]
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            BoxCollider2D boxCollider2D = hit.collider.GetComponent<BoxCollider2D>();
            if (boxCollider2D == null) continue;

            Vector2 colliderSize = boxCollider2D.bounds.size;
            Vector2 colliderPos = boxCollider2D.bounds.center;
            Vector3 colliderTopLeft = new Vector3(colliderPos.x - colliderSize.x / 2, colliderPos.y + colliderSize.y / 2);
            Vector3 colliderBotRight = new Vector3(colliderPos.x + colliderSize.x / 2, colliderPos.y - colliderSize.y / 2);
            Vector2Int indexTopLeft = Pos2GridIndex(colliderTopLeft);
            Vector2Int indexBotRight = Pos2GridIndex(colliderBotRight);

            // Ensure top-left is really top-left in grid indices
            int startX = Mathf.Min(indexTopLeft.x, indexBotRight.x);
            int endX = Mathf.Max(indexTopLeft.x, indexBotRight.x);
            int startY = Mathf.Min(indexBotRight.y, indexTopLeft.y); // note: topLeft.y > bottomRight.y in world space
            int endY = Mathf.Max(indexBotRight.y, indexTopLeft.y);

            // Inclusive loops (so +1)
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (x >= 0 && x < gridCols && y >= 0 && y < gridRows)
                    {
                        grid[x, y] = 1;
                    }
                }
            }
        }

        return grid;
    }
    public Vector2Int Pos2GridIndex(Vector2 pos)
    {
        int x = Mathf.FloorToInt((pos.x - topLeft.x) / cell_x);
        int y = Mathf.FloorToInt((topLeft.y - pos.y) / cell_y);
        return new Vector2Int(x, y);
    }
    public Vector3 GridIndex2Pos(Vector2Int index)
    {
        float x = index.x * cell_x + topLeft.x + cell_x / 2f;
        float y = topLeft.y - index.y * cell_y - cell_y / 2f;
        return new Vector3(x, y, 0);
    }
    public List<Vector3> FindPath(int[,] grid, Vector2Int start, Vector2Int goal)
    {

        int Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        gScore[start] = 0;
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();
        fScore[start] = Heuristic(start, goal);
        List<Vector2Int> openSet = new List<Vector2Int> { start };

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            foreach (var node in openSet)
            {
                if (!fScore.ContainsKey(node)) continue;
                if (fScore[current] > fScore[node])
                    current = node;
            }

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // IMPORTANT: use gridCols for x bounds and gridRows for y bounds
                if (neighbor.x < 0 || neighbor.x >= gridCols || neighbor.y < 0 || neighbor.y >= gridRows)
                    continue;

                if (grid[neighbor.x, neighbor.y] == 1)
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        Debug.Log("Not exist path to goal");
        return new List<Vector3>();
    }

    private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> path = new List<Vector3>() { GridIndex2Pos(current) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(GridIndex2Pos(current));
        }
        path.Reverse();
        return OptimizePath(path);
    }
    public static List<Vector3> OptimizePath(List<Vector3> path)
    {
        if (path == null || path.Count < 2) return path;

        Vector3 direc = (path[1] - path[0]).normalized;
        int i = 2;
        while (i < path.Count)
        {
            Vector3 currentDirec = (path[i] - path[i - 1]).normalized;
            if (IsSameDirection(currentDirec, direc))
            {
                path.RemoveAt(i - 1);
                // không tăng i, vì phần tử mới ở i-1 cần được check tiếp
            }
            else
            {
                direc = currentDirec;
                i++;
            }
        }
        return path;
    }

    public static bool IsSameDirection(Vector3 a, Vector3 b)
    {
        if (a == Vector3.zero || b == Vector3.zero)
            return false;

        a.Normalize();
        b.Normalize();
        return Vector3.Dot(a, b) > 0.999f;
    }
}

