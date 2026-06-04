using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cellSize;
    [SerializeField] private Color RedCol = Color.red;
    [SerializeField] private Vector2 minBorder;
    [SerializeField] private Vector2 maxBorder;

    SpriteRenderer SR;

    private Color DefaultCol;
    private GameObject currentObject;

    [SerializeField] internal GameObject objectPrefab;
    [SerializeField] internal Button btnSetObj;
    [SerializeField] internal GameObject Panel;

    [SerializeField] private ResourcesManager RM;
    [SerializeField] private Moves PlMoves;

    private int tag_num;
    private bool isDragging = false;
    internal bool isSet = false;
    private void Start()
    {
    }

    private void Update()
    {
        if (isSet)
        {
            setObject();
        }
    }
    private void setObject()
    {
        // Перетаскивание объекта при создании

        if (isDragging && currentObject != null) // В режиме размещения координату устанавливаемого префаба размещаем на логической сетке
        {
            if (Input.GetMouseButtonDown(0) && Obj_in_Borders(currentObject)) // Фиксируем объект на месте при повторном нажатии ЛКМ
            {
                isDragging = false;
                if (currentObject != null)
                {
                    SR = currentObject.GetComponent<SpriteRenderer>();
                    if (SR != null)
                        SR.color = DefaultCol;
                }
                UpdateResMiner(PlMoves.isMoveFirst ? RM.pl1 : RM.pl2);
                currentObject = null;
                isSet = false;
            }
            else
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // При размещении к-ты равны округленным к-там мышки
                mousePos.x = Mathf.Round(mousePos.x / cellSize) * cellSize;
                mousePos.y = Mathf.Round(mousePos.y / cellSize) * cellSize;
                currentObject.transform.position = mousePos;

                // Проверяем границы и меняем цвет
                UpdateObjectColor();
            }
        }
    }
    private void UpdateResMiner(ResourcesData ResPlData)
    {
        tag_num = 0;
        if (objectPrefab.tag == "iron_miner") tag_num = 1;
        if (objectPrefab.tag == "gold_miner") tag_num = 2;
        if (objectPrefab.tag == "materials_miner") tag_num = 3;
        if (objectPrefab.tag == "titanium_miner") tag_num = 4;
        if (objectPrefab.tag == "oil_miner") tag_num = 5;

        switch (tag_num)
        {
            case 1:
                ResPlData.ironMiner++;
                break;
            case 2:
                ResPlData.goldMiner++;
                break;
            case 3:
                ResPlData.buildMaterialsMiner++;
                break;
            case 4:
                ResPlData.titaniumMiner++;
                break;
            case 5:
                ResPlData.oilMiner++;
                break;
        }
    }
    internal void CreateObject()
    {
        // Закрываем панельку и создаем соответствующий ей объект по префабу на месте курсора
        Panel.SetActive(false);

        if (!isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.Round(mousePos.x / cellSize) * cellSize;
            mousePos.y = Mathf.Round(mousePos.y / cellSize) * cellSize;
            currentObject = Instantiate(objectPrefab, mousePos, Quaternion.identity);
            // Получаем SpriteRenderer созданного объекта и сохраняем его цвет по умолчанию
            SR = currentObject.GetComponent<SpriteRenderer>();
            if (SR != null)
            {
                DefaultCol = SR.color;
            }

            isDragging = true;

            // Проверяем границы сразу после создания
            UpdateObjectColor();
        }
    }

    private void UpdateObjectColor()
    {
        if (currentObject != null && SR != null)
        {
            if ((!Obj_in_Borders(currentObject)))
            {
                SR.color = RedCol;
            }
            else
            {
                SR.color = DefaultCol;
            }
        }
    }

    private bool Obj_in_Borders(GameObject obj)
    {
        return (obj.transform.position.x > minBorder.x && obj.transform.position.x < maxBorder.x
            && obj.transform.position.y > minBorder.y && obj.transform.position.y < maxBorder.y);
    }
    
}