using UnityEngine;

// Компонент превью
public class TerritoryPreview : MonoBehaviour
{
    private int cellX, cellY;
    private TerritoryManager manager;

    public void Init(int x, int y, TerritoryManager tm)
    {
        cellX = x;
        cellY = y;
        manager = tm;
    }

    void OnMouseDown()
    {
        if (manager != null)
            manager.CaptureTerritory(cellX, cellY);
    }
}
