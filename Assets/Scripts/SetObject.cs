using System;
using UnityEngine;
using UnityEngine.UI;
public class SetObject : MonoBehaviour
{
    [SerializeField] private GameObject Pref;
    [SerializeField] private GameObject SetPanel;
    [SerializeField] private Button SetButton;

    [SerializeField] private GameManager GM;
    [SerializeField] private ResourcesManager RM;
    [SerializeField] private Moves PlMoves;

    private int tag_num;
    private void Start()
    {
        SetButton.onClick.AddListener(SetSpawnParameters);
    }
    private void SetSpawnParameters()
    {
        GM.objectPrefab = Pref;
        GM.btnSetObj = SetButton;
        GM.Panel = SetPanel;
        GM.CreateObject();
        GM.isSet = true;
        UpdateResMiner(PlMoves.isMoveFirst ? RM.pl1 : RM.pl2);
        Debug.Log($"Ресурсы 1 игрока: {RM.pl1.ResMiners.Count}\nРесурсы 2 игрока:{RM.pl2.ResMiners.Count}");
    }
    private void UpdateResMiner(ResourcesData ResPlData)
    {
        tag_num = 0;
        if (Pref.tag == "iron_miner") tag_num = 1;
        if (Pref.tag == "gold_miner") tag_num = 2;
        if (Pref.tag == "materials_miner") tag_num = 3;
        if (Pref.tag == "titanium_miner") tag_num = 4;
        if (Pref.tag == "oil_miner") tag_num = 5;

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
}
