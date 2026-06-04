using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject Pause;
    [SerializeField] private GameObject BuildingsRoadAndTerritory;
    [SerializeField] private GameObject BuildingsAttacks;
    [SerializeField] private GameObject BuildingsDefence;
    [SerializeField] private GameObject BuildingsOthers;

    [SerializeField] private Button btnRoadAndTerritory;
    [SerializeField] private Button btnAttack;
    [SerializeField] private Button btnDefence;
    [SerializeField] private Button btnOther;
    [SerializeField] private Button btnMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (btnRoadAndTerritory == null)
            btnRoadAndTerritory = GameObject.Find("btnBuildingsRoad")?.GetComponent<Button>();

        if (btnAttack == null)
            btnAttack = GameObject.Find("btnBuildingsAttack")?.GetComponent<Button>();

        if (btnDefence == null)
            btnDefence = GameObject.Find("btnBuildingsDefence")?.GetComponent<Button>();

        if (btnOther == null)
            btnOther = GameObject.Find("btnBuildingsOther")?.GetComponent<Button>();

        if (btnMove == null)
            btnMove = GameObject.Find("btnStep")?.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangePause();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("GameWindow"); // для открытия игрового окна
    }

    public void Back()
    {
        SceneManager.LoadScene("SampleScene"); // возвращаемся назад в главное меню
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Game has been closed");
    }

    public void ChangePause()
    {
        if (Pause.activeSelf == true)
        {
            Pause.SetActive(false);

            btnRoadAndTerritory.interactable = true;
            btnAttack.interactable = true;
            btnDefence.interactable = true;
            btnOther.interactable = true;
            btnMove.interactable = true;

            BuildingsRoadAndTerritory.SetActive(true);
            BuildingsAttacks.SetActive(true);
            BuildingsDefence.SetActive(true);
            BuildingsOthers.SetActive(true);
        }
        else
        {
            Pause.SetActive(true);

            btnRoadAndTerritory.interactable = false;
            btnAttack.interactable = false;
            btnDefence.interactable = false;
            btnOther.interactable = false;
            btnMove.interactable = false;

            BuildingsRoadAndTerritory.SetActive(false);
            BuildingsAttacks.SetActive(false);
            BuildingsDefence.SetActive(false);
            BuildingsOthers.SetActive(false);
        }
    }

    public void ChangePanelBuildingsRoadAndTerritory()
    {
        ChangePanelStatus(BuildingsPanels.RoadAndTerritory);
    }

    public void ChangePanelBuildingsAttacks()
    {
        ChangePanelStatus(BuildingsPanels.Attacks);
    }

    public void ChangePanelBuildingsDefence()
    {
        ChangePanelStatus(BuildingsPanels.Defence);
    }

    public void ChangePanelBuildingsOthers()
    {
        ChangePanelStatus(BuildingsPanels.Others);
    }

    public void CloseAllBuildingsPanel()
    {
        if (BuildingsRoadAndTerritory != null) BuildingsRoadAndTerritory.SetActive(false);
        if (BuildingsAttacks != null) BuildingsAttacks.SetActive(false);
        if (BuildingsDefence != null) BuildingsDefence.SetActive(false);
        if (BuildingsOthers != null) BuildingsOthers.SetActive(false);
    }

    public enum BuildingsPanels
    {
        RoadAndTerritory,
        Attacks,
        Defence,
        Others
    }

    public void ChangePanelStatus(BuildingsPanels panels)
    {
        CloseAllBuildingsPanel();

        switch (panels)
        {
            case BuildingsPanels.RoadAndTerritory:
                {
                    BuildingsRoadAndTerritory.SetActive(true);
                    break;
                }
            case BuildingsPanels.Attacks:
                {
                    BuildingsAttacks.SetActive(true);
                    break;
                }
            case BuildingsPanels.Defence:
                {
                    BuildingsDefence.SetActive(true);
                    break;
                }
            case BuildingsPanels.Others:
                {
                    BuildingsOthers.SetActive(true);
                    break;
                }
        }
    }
}