using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cellSize;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float mapWidth = 20f;   // Ширина игрового поля
    [SerializeField] private float mapHeight = 12f;  // Высота игрового поля
    [SerializeField] private Color RedCol = Color.red;
    [SerializeField] private Vector2 minBorder;
    [SerializeField] private Vector2 maxBorder;

    SpriteRenderer SR;

    private Color DefaultCol;
    private GameObject currentObject;

    [SerializeField] internal GameObject objectPrefab;
    [SerializeField] internal Button btnSetObj;
    [SerializeField] internal GameObject Panel;

    private bool isDragging = false;
    internal bool isSet = false;
    private void Start()
    {
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // Сохраняем позицию мыши в мировых координатах
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Меняем зум
            float newZoom = Camera.main.orthographicSize - scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);

            // Смещаем камеру, чтобы мышь оставалась на той же позиции
            Vector3 newMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Camera.main.transform.position += mouseWorldPos - newMouseWorldPos;
            LimitCameraPosition();
        }
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
    private void LimitCameraPosition()
    {
        Camera cam = Camera.main;
        float verticalSize = cam.orthographicSize;
        float horizontalSize = verticalSize * cam.aspect;

        Vector3 pos = cam.transform.position;

        // Ограничиваем позицию, чтобы камера не выходила за границы карты
        pos.x = Mathf.Clamp(pos.x, -mapWidth + horizontalSize, mapWidth - horizontalSize);
        pos.y = Mathf.Clamp(pos.y, -mapHeight + verticalSize, mapHeight - verticalSize);

        cam.transform.position = pos;
    }

    private bool Obj_in_Borders(GameObject obj)
    {
        return (obj.transform.position.x > minBorder.x && obj.transform.position.x < maxBorder.x
            && obj.transform.position.y > minBorder.y && obj.transform.position.y < maxBorder.y);
    }
}