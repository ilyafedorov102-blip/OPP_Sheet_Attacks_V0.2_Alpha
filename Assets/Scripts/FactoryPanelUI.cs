using UnityEngine;
using UnityEngine.UI;

public class FactoryPanelUI : MonoBehaviour
{
    public Button buildButton;
    public Button closeButton;

    private FactoryProduction owner;

    public void Init(FactoryProduction factory)
    {
        owner = factory;

        buildButton.onClick.AddListener(owner.StartPlacement);
        closeButton.onClick.AddListener(owner.ClosePanel);
    }
}