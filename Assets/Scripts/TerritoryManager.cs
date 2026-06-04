using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoryManager : MonoBehaviour
{
    [Header("Настройки сетки")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform gridParent;

    [Header("Префабы территорий")]
    [SerializeField] private GameObject neutralTilePrefab;
    [SerializeField] private GameObject player1TilePrefab;
    [SerializeField] private GameObject player2TilePrefab;
    [SerializeField] private GameObject greenPreviewPrefab;

    [Header("UI")]
    [SerializeField] private Button captureButton;
    [SerializeField] private GameObject capturePanel;

    [Header("Менеджер шагов")]
    [SerializeField] private Moves PlMoves;

    private TerritoryCell[,] grid;
    private List<Vector2Int> currentPreviewPositions = new List<Vector2Int>();

    void Start()
    {
        Grid_Create();
        if (captureButton != null)
            captureButton.onClick.AddListener(OnCaptureButtonClick);
    }
    void Grid_Create()
    {
        grid = new TerritoryCell[gridWidth, gridHeight];
        // Сначала создаём все нейтральные клетки
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject tile = Instantiate(neutralTilePrefab, pos + gridParent.transform.position, Quaternion.identity, gridParent);
                TerritoryCell cell = tile.GetComponent<TerritoryCell>();

                cell.Init(x, y, TerritoryType.Neutral);
                grid[x, y] = cell;
            }
        }

        // Устанавливаем начальную территорию первого игрока (слева)
        int player1StartX = 0;
        int player1StartY = Mathf.Clamp(gridHeight / 2, 0, gridHeight - 1);
        SetTerritory(player1StartX, player1StartY, TerritoryType.Player1, player1TilePrefab);

        // Устанавливаем начальную территорию второго игрока (справа)
        int player2StartX = gridWidth - 1;
        int player2StartY = Mathf.Clamp(gridHeight / 2, 0, gridHeight - 1);
        SetTerritory(player2StartX, player2StartY, TerritoryType.Player2, player2TilePrefab);

        Debug.Log($"Сетка создана: {gridWidth}x{gridHeight}");
    }

    void SetTerritory(int x, int y, TerritoryType type, GameObject prefab)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning($"Попытка установить территорию за пределами сетки: ({x},{y})");
            return;
        }

        // Уничтожаем старую клетку, если она существует
        if (grid[x, y] != null)
        {
            Destroy(grid[x, y].gameObject);
        }

        // Создаём новую
        Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0);
        GameObject newTile = Instantiate(prefab, pos+gridParent.transform.position, Quaternion.identity, gridParent);
        TerritoryCell newCell = newTile.GetComponent<TerritoryCell>();

        if (newCell == null)
        {
            Debug.LogError($"На префабе {prefab.name} отсутствует компонент TerritoryCell!");
            return;
        }

        newCell.Init(x, y, type);
        grid[x, y] = newCell;
    }

    void OnCaptureButtonClick()
    {
        if (capturePanel != null)
            capturePanel.SetActive(false);

        TerritoryType currentPlayerType = PlMoves.isMoveFirst ? TerritoryType.Player1 : TerritoryType.Player2;
        List<Vector2Int> allyPositions = new List<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null && grid[x, y].type == currentPlayerType)
                    allyPositions.Add(new Vector2Int(x, y));
            }
        }

        ClearPreviews();

        foreach (Vector2Int pos in allyPositions)
        {
            TryAddPreview(pos.x + 1, pos.y);
            TryAddPreview(pos.x - 1, pos.y);
            TryAddPreview(pos.x, pos.y + 1);
            TryAddPreview(pos.x, pos.y - 1);
        }

        if (currentPreviewPositions.Count == 0)
        {
            Debug.Log("Нет доступных территорий для захвата!");
        }
    }

    void TryAddPreview(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        return;

        if (currentPreviewPositions.Contains(new Vector2Int(x, y)))
            return;

        TerritoryType currentPlayerType = PlMoves.isMoveFirst
            ? TerritoryType.Player1
            : TerritoryType.Player2;

        // Нельзя выбрать свою территорию
        if (grid[x, y] == null  || grid[x, y].type == currentPlayerType)
            return;

        if (greenPreviewPrefab == null)
        {
            Debug.LogError("greenPreviewPrefab не назначен!");
            return;
        }

        GameObject preview = Instantiate(
            greenPreviewPrefab,
            grid[x, y].transform.position,
            Quaternion.identity
        );

        TerritoryPreview previewComp = preview.GetComponent<TerritoryPreview>();

        if (previewComp != null)
            previewComp.Init(x, y, this);
        else
            Debug.LogError($"На префабе {greenPreviewPrefab.name} отсутствует компонент TerritoryPreview!");

        currentPreviewPositions.Add(new Vector2Int(x, y));
    }

    public void CaptureTerritory(int x, int y)
    {
        if (grid[x, y] == null)
            return;

        TerritoryType currentPlayerType = PlMoves.isMoveFirst
            ? TerritoryType.Player1
            : TerritoryType.Player2;

        // Нельзя захватывать свою территорию
        if (grid[x, y].type == currentPlayerType)
            return;

        GameObject newPrefab = PlMoves.isMoveFirst
            ? player1TilePrefab
            : player2TilePrefab;

        SetTerritory(x, y, currentPlayerType, newPrefab);

        ClearPreviews();

        if (capturePanel != null)
            capturePanel.SetActive(true);
    }


    void ClearPreviews()
    {
        foreach (Vector2Int pos in currentPreviewPositions)
        {
            if (grid != null && pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight)
            {
                if (grid[pos.x, pos.y] != null)
                {
                    Collider2D[] hits = Physics2D.OverlapPointAll(grid[pos.x, pos.y].transform.position);
                    foreach (var hit in hits)
                    {
                        if (hit.GetComponent<TerritoryPreview>() != null)
                            Destroy(hit.gameObject);
                    }
                }
            }
        }
        currentPreviewPositions.Clear();
    }
}

// Типы территорий
public enum TerritoryType
{
    Neutral,
    Player1,
    Player2
}