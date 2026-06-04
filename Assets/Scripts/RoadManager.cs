using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject roadSegmentPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject buildPanel;

    [Header("Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);

    internal bool IsRoadsConnected { get; private set; } = false;

    private enum BuildState { Idle, SelectingStart, SelectingEnd }
    private BuildState currentState = BuildState.Idle;

    private Vector2Int? startCell = null;
    private Vector2Int? endCell = null;

    private Dictionary<Vector2Int, GameObject> roadSegments = new Dictionary<Vector2Int, GameObject>();
    private List<GameObject> tempRoadSegments = new List<GameObject>();

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (roadSegmentPrefab == null)
            Debug.LogError("Road Segment Prefab not assigned!");

        IsRoadsConnected = false;
    }

    void Update()
    {
        if (currentState != BuildState.Idle)
        {
            HandleRoadBuilding();
        }
    }

    public void StartRoadCreation()
    {
        if (buildPanel != null)
            buildPanel.SetActive(false);

        currentState = BuildState.SelectingStart;
        Debug.Log("Режим создания дороги: выберите начальную клетку");
    }

    private void HandleRoadBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPoint = hit.point;
            Vector2Int currentCell = WorldToGridCell(hitPoint);

            // Обновляем временную дорогу для обоих состояний
            if (currentState == BuildState.SelectingStart && startCell.HasValue)
            {
                UpdateTempRoad(startCell.Value, currentCell);
            }
            else if (currentState == BuildState.SelectingEnd && startCell.HasValue)
            {
                UpdateTempRoad(startCell.Value, currentCell);
            }

            // Обработка клика
            if (Input.GetMouseButtonDown(0))
            {
                if (currentState == BuildState.SelectingStart)
                {
                    if (!IsCellOccupied(currentCell))
                    {
                        startCell = currentCell;
                        currentState = BuildState.SelectingEnd;
                        Debug.Log($"Начало дороги выбрано: {currentCell}. Теперь выберите конец дороги.");
                    }
                    else
                    {
                        Debug.Log("Клетка уже занята! Выберите другую.");
                    }
                }
                else if (currentState == BuildState.SelectingEnd && startCell.HasValue)
                {
                    endCell = currentCell;

                    if (startCell.Value == endCell.Value)
                    {
                        Debug.Log("Начальная и конечная клетки совпадают. Выберите другую.");
                        return;
                    }

                    BuildRoad(startCell.Value, endCell.Value);

                    ClearTempRoad();
                    startCell = null;
                    endCell = null;
                    currentState = BuildState.Idle;

                    if (buildPanel != null)
                        buildPanel.SetActive(true);

                    CheckConnectionBetweenObjects();
                }
            }
        }
    }

    private void BuildRoad(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = GetStraightPath(start, end);
        if (path == null || path.Count == 0)
        {
            Debug.LogError("Не удалось построить путь!");
            return;
        }

        // Проверяем, не занята ли какая-либо клетка на пути
        foreach (Vector2Int cell in path)
        {
            if (IsCellOccupied(cell))
            {
                Debug.LogWarning($"Клетка {cell} уже занята! Строительство прервано.");
                return;
            }
        }

        // Строим сегменты дороги
        foreach (Vector2Int cell in path)
        {
            if (!roadSegments.ContainsKey(cell))
            {
                GameObject newSegment = Instantiate(roadSegmentPrefab, GridCellToWorld(cell), Quaternion.identity, gridParent);
                roadSegments.Add(cell, newSegment);
            }
        }

        MergeRoads();
        Debug.Log($"Дорога построена! Количество сегментов: {path.Count}");
    }

    private List<Vector2Int> GetStraightPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        if (start.x != end.x && start.y != end.y)
        {
            // L-образный путь: сначала горизонтальный, потом вертикальный
            Vector2Int intermediate = new Vector2Int(end.x, start.y);
            path.AddRange(GetHorizontalPath(start, intermediate));
            path.AddRange(GetVerticalPath(intermediate, end));
        }
        else if (start.x == end.x)
        {
            path.AddRange(GetVerticalPath(start, end));
        }
        else if (start.y == end.y)
        {
            path.AddRange(GetHorizontalPath(start, end));
        }

        return path;
    }

    private List<Vector2Int> GetHorizontalPath(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        int step = (to.x > from.x) ? 1 : -1;
        for (int x = from.x; x != to.x + step; x += step)
        {
            path.Add(new Vector2Int(x, from.y));
        }
        return path;
    }

    private List<Vector2Int> GetVerticalPath(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        int step = (to.y > from.y) ? 1 : -1;
        for (int y = from.y + step; y != to.y + step; y += step)
        {
            path.Add(new Vector2Int(from.x, y));
        }
        return path;
    }

    private void UpdateTempRoad(Vector2Int start, Vector2Int currentMouseCell)
    {
        ClearTempRoad();

        if (start == currentMouseCell) return;

        List<Vector2Int> tempPath = GetStraightPath(start, currentMouseCell);

        foreach (Vector2Int cell in tempPath)
        {
            // Показываем только те клетки, которые не заняты постоянной дорогой
            if (!roadSegments.ContainsKey(cell))
            {
                GameObject tempSegment = Instantiate(roadSegmentPrefab, GridCellToWorld(cell), Quaternion.identity);

                // Делаем временную дорогу полупрозрачной (опционально)
                MeshRenderer renderer = tempSegment.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = 0.5f;
                    renderer.material.color = color;
                }

                tempRoadSegments.Add(tempSegment);
            }
        }
    }

    private void ClearTempRoad()
    {
        foreach (GameObject segment in tempRoadSegments)
        {
            if (segment != null)
                Destroy(segment);
        }
        tempRoadSegments.Clear();
    }

    private void CheckConnectionBetweenObjects()
    {
        if (!startCell.HasValue || !endCell.HasValue) return;

        bool hasStartObject = CheckObjectAtCell(startCell.Value, "Building");
        bool hasEndObject = CheckObjectAtCell(endCell.Value, "Building");

        if (hasStartObject && hasEndObject)
        {
            IsRoadsConnected = true;
            Debug.Log("Дорога соединила два здания! IsRoadsConnected = true");
        }
        else
        {
            IsRoadsConnected = false;
        }
    }

    private bool CheckObjectAtCell(Vector2Int cell, string tag)
    {
        Vector3 worldPos = GridCellToWorld(cell);
        Collider[] colliders = Physics.OverlapSphere(worldPos, cellSize * 0.4f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(tag))
                return true;
        }
        return false;
    }

    private void MergeRoads()
    {
        Debug.Log("Дороги объединены в одну сеть");
        // Здесь можно добавить логику объединения сетей
    }

    private Vector2Int WorldToGridCell(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.z / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector3 GridCellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * cellSize, 0f, cell.y * cellSize);
    }

    private bool IsCellOccupied(Vector2Int cell)
    {
        return roadSegments.ContainsKey(cell);
    }
}