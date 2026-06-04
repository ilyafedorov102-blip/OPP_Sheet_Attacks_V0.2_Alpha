using System;
using Unity.VisualScripting;
using UnityEngine;

public class TecnichsMoving : MonoBehaviour
{
    [SerializeField] private Technic Technic_obj;
    [SerializeField] private GameObject Tech_Panel;
    [SerializeField] private GameObject ArrowsPanel;
    private bool isMenuActive, isArrowsActive;
    void Start()
    {
        isMenuActive = false; isArrowsActive = false;
        Tech_Panel.SetActive(isMenuActive);
        ArrowsPanel.SetActive(isArrowsActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Vector3 Obj_To_ScreenCrd(GameObject obj)
    {
        Vector3 obj_crd = obj.transform.position;
        return new Vector3(obj_crd.x*100, obj_crd.y*100, 0);
    }
    public void UpMoving()
    {
        Technic_obj.Move(new Vector2(0,1));
    }
    public void DownMoving()
    {
        Technic_obj.Move(new Vector2(0, -1));
    }
    public void LeftMoving()
    {
        Technic_obj.Move(new Vector2(-1, 0));
    }
    public void RightMoving()
    {
        Technic_obj.Move(new Vector2(1, 0));
    }
    public void SetMenuVisibility()
    {
        Tech_Panel.SetActive(isMenuActive);
        isMenuActive = !isMenuActive;
    }
    public void SetArrowsVisibility()
    {
        ArrowsPanel.SetActive(isArrowsActive);
        isArrowsActive = !isArrowsActive;
    }
    public void Tech_Attack()
    {
        SetMenuVisibility(); SetArrowsVisibility();
    }
}
