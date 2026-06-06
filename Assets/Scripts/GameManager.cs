using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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
    [SerializeField] private TerritoryManager territoryManager;
    [SerializeField] private Moves PlMoves;

    private int tag_num;
    private bool isDragging;
    internal bool isSet, isMiner;

    private void Start()
    {
        isSet = false; isMiner = false; isDragging = false;
    }
    private void Update()
    {
        if (PlMoves.attakedBuild != 0)
        {
            PlayerPrefs.SetInt("PlayerNum", PlMoves.attakedBuild);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Final1");
        }
        if (isSet)
        {
            setObject();
        }
        
    }
    private void UpdateResMinerByType(ResourcesData ResPlData, ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Iron:
                ResPlData.ironMiner++;
                break;
            case ResourceType.Gold:
                ResPlData.goldMiner++;
                break;
            case ResourceType.Materials:
                ResPlData.buildMaterialsMiner++;
                break;
            case ResourceType.Titanium:
                ResPlData.titaniumMiner++;
                break;
            case ResourceType.Oil:
                ResPlData.oilMiner++;
                break;
        }
    }

    
    private void setObject()
    {
        // Перетаскивание объекта при создании
        // xuyxuyxuy
        if (isDragging && currentObject != null) // В режиме размещения координату устанавливаемого префаба размещаем на логической сетке
        {
            if (Input.GetMouseButtonDown(0) && Obj_in_Borders(currentObject)) // Фиксируем объект на месте при повторном нажатии ЛКМ
            {
                // Проверяем, находится ли выбранная клетка в собственной территории
                Vector2 placePos = currentObject.transform.position;
                TerritoryType cellType = TerritoryType.Neutral;
                if (territoryManager != null)
                    cellType = territoryManager.GetTerritoryTypeAtPosition(placePos);

                TerritoryType currentPlayerType = PlMoves.isMoveFirst ? TerritoryType.Player1 : TerritoryType.Player2;

                if (cellType != currentPlayerType)
                {
                    // Нельзя ставить не на своей территории
                    if (SR != null) SR.color = RedCol;
                }
                else
                {
                    if (isMiner)
                    {
                        // Ищем ресурс под точкой
                        Collider2D[] hits = Physics2D.OverlapPointAll(placePos);
                        ResourceNode found = null;
                        foreach (var h in hits)
                        {
                            var rn = h.GetComponent<ResourceNode>();
                            if (rn != null && !rn.occupied)
                            {
                                found = rn;
                                break;
                            }
                        }

                        if (found == null)
                        {
                            // Нет подходящего ресурса
                            if (SR != null) SR.color = RedCol;
                        }
                        else
                        {
                            // Успешное размещение — привязываем Miner к ResourceNode
                            var minerComp = currentObject.GetComponent<Miner>();
                            if (minerComp == null)
                                minerComp = currentObject.AddComponent<Miner>();

                            minerComp.Claim(found);

                            var playerData = PlMoves.isMoveFirst ? RM.pl1 : RM.pl2;
                            if (playerData != null)
                            {
                                playerData.ResMiners.Add(currentObject);
                            }

                            isDragging = false;
                            if (currentObject != null)
                            {
                                SR = currentObject.GetComponent<SpriteRenderer>();
                                if (SR != null)
                                    SR.color = DefaultCol;
                            }

                            // Увеличиваем счётчик в зависимости от типа майнера
                            if (playerData != null && minerComp != null)
                                UpdateResMinerByType(playerData, minerComp.resourceType);

                            currentObject = null;
                            isSet = false;
                            if (PlMoves.isMoveFirst) RM.pl1.iron -= 10;
                            else RM.pl2.iron -= 10;
                        }
                    }
                    else
                    {
                        isDragging = false;
                        isSet = false;  // Уже есть

                        // ДОБАВЬТЕ: сбросьте флаги для нового объекта
                        isMiner = false;
                        if (PlMoves.isMoveFirst)
                        {
                            RM.pl1.iron -= 20;
                            RM.pl1.buildMaterials -= 30;
                            RM.pl1.titanium -= 5;
                        }
                        else
                        {
                            RM.pl2.iron -= 20;
                            RM.pl2.buildMaterials -= 30;
                            RM.pl2.titanium -= 5;
                        }
                    }
                }
            }
            // xuyxuyxuy
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
    /*
     private void setObject()
    {
        // Перетаскивание объекта при создании

        if (isDragging && currentObject != null) // В режиме размещения координату устанавливаемого префаба размещаем на логической сетке
        {
            if (Input.GetMouseButtonDown(0) && Obj_in_Borders(currentObject)) // Фиксируем объект на месте при повторном нажатии ЛКМ
            {
                // Проверяем, находится ли выбранная клетка в собственной территории
                Vector2 placePos = currentObject.transform.position;
                TerritoryType cellType = TerritoryType.Neutral;
                if (territoryManager != null)
                    cellType = territoryManager.GetTerritoryTypeAtPosition(placePos);

                TerritoryType currentPlayerType = PlMoves.isMoveFirst ? TerritoryType.Player1 : TerritoryType.Player2;

                if (cellType != currentPlayerType)
                {
                    // Нельзя ставить не на своей территории
                    if (SR != null) SR.color = RedCol;
                }
                else
                {
                    // Ищем ресурс под точкой
                    Collider2D[] hits = Physics2D.OverlapPointAll(placePos);
                    ResourceNode found = null;
                    var rn
                    foreach (var h in hits)
                    {
                        rn = h.GetComponent<ResourceNode>();
                        if (rn != null && !rn.occupied)
                        {
                            found = rn;
                            break;
                        }
                    }

                    if (found == null)
                    {
                        // Нет подходящего ресурса
                        if (SR != null) SR.color = RedCol;
                    }
                    else
                    {
                        // Успешное размещение — привязываем Miner к ResourceNode
                        var minerComp = currentObject.GetComponent<Miner>();
                        if (minerComp == null)
                            minerComp = currentObject.AddComponent<Miner>();

                        minerComp.Claim(found);

                        var playerData = PlMoves.isMoveFirst ? RM.pl1 : RM.pl2;
                        if (playerData != null)
                        {
                            playerData.ResMiners.Add(currentObject);
                        }

                        isDragging = false;
                        if (currentObject != null)
                        {
                            SR = currentObject.GetComponent<SpriteRenderer>();
                            if (SR != null)
                                SR.color = DefaultCol;
                        }

                        // Увеличиваем счётчик в зависимости от типа майнера
                        if (playerData != null && minerComp != null)
                            UpdateResMinerByType(playerData, minerComp.resourceType);

                        currentObject = null;
                        isSet = false;
                    }
                }
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
     */
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