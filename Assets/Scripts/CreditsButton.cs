using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Вешается на кнопку "Авторы".
/// В инспекторе перетащи GameObject панели авторов в поле creditsPanel.
/// </summary>
[RequireComponent(typeof(Button))]
public class CreditsButton : MonoBehaviour
{
    [Header("Панель авторов")]
    [Tooltip("GameObject с панелью авторов")]
    [SerializeField] private GameObject creditsPanel;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenCredits);

        // Панель скрыта при старте
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        if (creditsPanel == null)
        {
            Debug.LogWarning("CreditsButton: creditsPanel не назначен!");
            return;
        }

        creditsPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OpenCredits);
    }
}
