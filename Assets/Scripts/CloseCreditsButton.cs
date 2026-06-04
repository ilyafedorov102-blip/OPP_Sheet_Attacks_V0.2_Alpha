using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Вешается на кнопку-крестик внутри панели авторов.
/// В инспекторе перетащи GameObject панели авторов в поле creditsPanel,
/// либо оставь пустым — тогда скрипт сам найдёт ближайший родительский Panel.
/// </summary>
[RequireComponent(typeof(Button))]
public class CloseCreditsButton : MonoBehaviour
{
    [Header("Панель авторов (необязательно)")]
    [Tooltip("Если не назначено — будет использован первый родительский GameObject типа Panel")]
    [SerializeField] private GameObject creditsPanel;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(CloseCredits);

        // Автопоиск панели по иерархии, если не назначена вручную
        if (creditsPanel == null)
            creditsPanel = FindParentPanel();
    }

    private void CloseCredits()
    {
        if (creditsPanel == null)
        {
            Debug.LogWarning("CloseCreditsButton: creditsPanel не найден!");
            return;
        }

        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Поднимается вверх по иерархии и возвращает первый объект с компонентом Image
    /// (стандартный признак UI-панели), пропуская сам объект кнопки.
    /// </summary>
    private GameObject FindParentPanel()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            // Image есть почти у любой UI-панели
            if (current.GetComponent<Image>() != null)
                return current.gameObject;

            current = current.parent;
        }

        Debug.LogWarning("CloseCreditsButton: родительская панель не найдена автоматически. " +
                         "Назначь creditsPanel вручную в инспекторе.");
        return null;
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(CloseCredits);
    }
}
