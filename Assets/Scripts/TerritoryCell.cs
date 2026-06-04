using UnityEngine;

// Компонент ячейки
public class TerritoryCell : MonoBehaviour
{
    public int x, y;
    public TerritoryType type;

    public void Init(int _x, int _y, TerritoryType _type)
    {
        x = _x;
        y = _y;
        type = _type;
    }
}
