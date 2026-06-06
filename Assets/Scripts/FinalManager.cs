using TMPro;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    [SerializeField] private TMP_Text FinalTitle;
    void Start()
    {
        FinalTitle.text = $"Победил {PlayerPrefs.GetInt("PlayerNum")} игрок!!!";
    }
}
