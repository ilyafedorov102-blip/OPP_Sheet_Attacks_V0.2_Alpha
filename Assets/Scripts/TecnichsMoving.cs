using System;
using Unity.VisualScripting;
using UnityEngine;

public class TecnichsMoving : MonoBehaviour
{
    [SerializeField] private Technic Technic_obj;
    [SerializeField] private GameObject Tech_Panel;
    [SerializeField] private GameObject ArrowsPanel;
    private bool isMenuActive, isArrowsActive;
    private Vector2 Unit_Vector = new Vector2(1, 1);
    void Start()
    {
        isMenuActive = false; isArrowsActive = false;
        Tech_Panel.SetActive(isMenuActive);
        ArrowsPanel.SetActive(isArrowsActive);
    }
    private Vector2 Rotate_to_MovingVector(float Angle_deg)
    {
        if (Math.Round(Angle_deg) == 90)
        {
            return new Vector2(0, 1);
        }
        if (Math.Round(Angle_deg) == 180)
        {
            return new Vector2(-1, 0);
        }
        if (Math.Round(Angle_deg) == 270)
        {
            return new Vector2(0, -1);
        }
        else return new Vector2(1, 0);

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void ForwardMove()
    {
        Debug.Log($"{Technic_obj.transform.eulerAngles.z}");
        Technic_obj.Move(Rotate_to_MovingVector(Technic_obj.transform.eulerAngles.z));
        isArrowsActive = false;
        ArrowsPanel.SetActive(isArrowsActive);
    }
    public void BackMove()
    {
        Debug.Log($"{Technic_obj.transform.eulerAngles.z}");
        Technic_obj.Move(Rotate_to_MovingVector(Technic_obj.transform.eulerAngles.z) * -1);
        isArrowsActive = false;
        ArrowsPanel.SetActive(isArrowsActive);
    }
    public void LeftMoving()
    {
        Vector3 rotate = Technic_obj.transform.eulerAngles;
        rotate.z += 90;
        Technic_obj.transform.rotation = Quaternion.Euler(rotate);
        Tech_Panel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    public void RightMoving()
    {
        Vector3 rotate = Technic_obj.transform.eulerAngles;
        rotate.z -= 90;
        Technic_obj.transform.rotation = Quaternion.Euler(rotate);
        rotate.z += 90;
        Tech_Panel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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