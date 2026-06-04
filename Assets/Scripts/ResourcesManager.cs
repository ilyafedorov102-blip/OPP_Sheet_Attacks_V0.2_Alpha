using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResourcesManager : MonoBehaviour
{
    internal ResourcesData pl1 = new ResourcesData();
    internal ResourcesData pl2 = new ResourcesData();
    internal int resFlow = 0;
    [Header("Timer & Resurces fields")]
    [SerializeField] private TMP_Text textResources_Pl1;
    [SerializeField] private TMP_Text textResources_Pl2;
    [SerializeField] private Timer timer;
    [Header("Resources prefabs")]
    [SerializeField] private GameObject Sp_gold;
    [SerializeField] private GameObject Sp_iron;
    [SerializeField] private GameObject Sp_oil;
    [SerializeField] private GameObject Sp_Titanium;
    [SerializeField] private GameObject Sp_materials;
    [Header("Objects' cell & borders")]
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2 minResBorder;
    [SerializeField] private Vector2 maxResBorder;

    List<Vector2> usedPositions = new List<Vector2>();

    private bool isColMainB = false;
    void Start()
    {
        Spawn_res(Sp_iron, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_gold, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_oil, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_Titanium, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_materials, Random.Range(4, 6), minResBorder, maxResBorder);
    }

    void Update()
    {
        textResources_Pl1.text = $"\t{pl1.iron}(+{pl1.ironMiner})   |   {pl1.gold}(+{pl1.goldMiner})   |   {pl1.titanium}(+{pl1.titaniumMiner})   |   {pl1.oil}(+{pl1.oilMiner})\n\t{pl1.buildMaterials}(+{pl1.buildMaterialsMiner}    |      {pl1.electricity}      |   {pl1.money}";
        textResources_Pl2.text = $"\t{pl2.iron}(+{pl2.ironMiner})   |   {pl2.gold}(+{pl2.goldMiner})   |   {pl2.titanium}(+{pl2.titaniumMiner})   |   {pl2.oil}(+{pl2.oilMiner})\n\t{pl2.buildMaterials}(+{pl2.buildMaterialsMiner})   |      {pl2.electricity}      |   {pl2.money}";

        if (timer.minute % 1 == 0 && timer.minute != resFlow)
        {
            pl1.iron += pl1.ironMiner;
            pl1.gold += pl1.goldMiner;
            pl1.titanium += pl1.titaniumMiner;
            pl1.oil += pl1.oilMiner;
            pl1.buildMaterials += pl1.buildMaterialsMiner;

            pl2.iron += pl2.ironMiner;
            pl2.gold += pl2.goldMiner;
            pl2.titanium += pl1.titaniumMiner;
            pl2.oil += pl1.oilMiner;
            pl2.buildMaterials += pl1.buildMaterialsMiner;

            resFlow = timer.minute;
        }
    }
    private void Spawn_res(GameObject Obj, int amount, Vector2 Min_crd, Vector2 Max_crd)
    {
        Vector2 Pos;
        GameObject temp; // Список занятых позиций

        while (amount > 0)
        {
            bool positionFound = false;

            while (!positionFound)
            {
                Pos.x = amount % 2 == 0 ? Random.Range(0, Max_crd.x) : Random.Range(Min_crd.x, 0);
                Pos.y = Random.Range(Min_crd.y, Max_crd.y);
                Pos.x = Mathf.Round(Pos.x / 2 / cellSize) * cellSize * 2;
                Pos.y = Mathf.Round(Pos.y / 2 / cellSize) * cellSize * 2;

                // Проверяем, не занята ли позиция и не взаимодействует 
                if (!usedPositions.Contains(Pos) && !isColMainB)
                {
                    usedPositions.Add(Pos);
                    temp = Instantiate(Obj, Pos, Quaternion.identity);
                    // Убедимся, что у объекта есть компонент ResourceNode и коллайдер для обнаружения
                    var rn = temp.GetComponent<ResourceNode>();
                    if (rn == null)
                        rn = temp.AddComponent<ResourceNode>();

                    // Попытаемся определить тип ресурса по ссылке на префаб
                    if (Obj == Sp_iron) rn.resourceType = ResourceType.Iron;
                    else if (Obj == Sp_gold) rn.resourceType = ResourceType.Gold;
                    else if (Obj == Sp_Titanium) rn.resourceType = ResourceType.Titanium;
                    else if (Obj == Sp_oil) rn.resourceType = ResourceType.Oil;
                    else if (Obj == Sp_materials) rn.resourceType = ResourceType.Materials;

                    // Добавим коллайдер для обнаружения при размещении
                    var col = temp.GetComponent<Collider2D>();
                    if (col == null)
                    {
                        var box = temp.AddComponent<BoxCollider2D>();
                        box.isTrigger = true;
                        // Размер клетки — cellSize
                        box.size = new Vector2(cellSize, cellSize);
                    }
                    amount--;
                    positionFound = true;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MainBuild1" || collision.gameObject.tag == "MainBuild2")
        {
            isColMainB = true;
        }
        else isColMainB = false;
    }
}
internal class ResourcesData
{
    internal List<GameObject> ResMiners;
    internal int iron;
    internal int gold;
    internal int titanium;
    internal int oil;
    internal int buildMaterials;
    internal int electricity;
    internal int money;

    internal int ironMiner = 1;
    internal int goldMiner = 1;
    internal int titaniumMiner = 1;
    internal int oilMiner = 1;
    internal int buildMaterialsMiner = 1;

    internal ResourcesData()
    {
        ResMiners = new List<GameObject>();
        iron = 0;
        gold = 0;
        titanium = 0;
        oil = 0;
        buildMaterials = 0;
        electricity = 0;
        money = 0;

        ironMiner = 1;
        goldMiner = 1;
        titaniumMiner = 1;
        oilMiner = 1;
        buildMaterialsMiner = 1;
    }
}