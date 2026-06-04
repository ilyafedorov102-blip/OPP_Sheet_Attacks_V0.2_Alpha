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
    [SerializeField] private bool isMiner;
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
        GM.isMiner = isMiner;
        Debug.Log($"Ресурсы 1 игрока: {RM.pl1.ResMiners.Count}\nРесурсы 2 игрока:{RM.pl2.ResMiners.Count}");
    }
}
