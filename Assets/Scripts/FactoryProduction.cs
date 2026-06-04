using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryProduction : MonoBehaviour
{
    [Header("Префабы")]
    [SerializeField] private GameObject placePrefab;
    [SerializeField] private GameObject previewPrefab;

    [Header("UI (ВНУТРИ ПРЕФАБА ЗАВОДА)")]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button closeButton;

    [Header("Настройки")]
    [SerializeField] private float offset = 1f;

    private List<GameObject> activePreviews = new List<GameObject>();
    private Transform target;

    private void Awake()
    {
        target = transform;

        // UI выключаем при старте
        if (controlPanel != null)
            controlPanel.SetActive(false);

        // подписка на кнопки
        if (buildButton != null)
            buildButton.onClick.AddListener(StartPlacement);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    private void OnMouseDown()
    {
        Debug.Log("CLICK!!");
        OpenPanel();
    }

    // ===================== UI =====================

    private void OpenPanel()
    {
        if (controlPanel != null)
            controlPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (controlPanel != null)
            controlPanel.SetActive(false);
    }

    // ===================== BUILD MODE =====================

    public void StartPlacement()
    {
        ClosePanel();
        ClearPreviews();

        Vector3 center = target.position;

        SpawnPreview(center + Vector3.up * offset);
        SpawnPreview(center + Vector3.down * offset);
        SpawnPreview(center + Vector3.left * offset);
        SpawnPreview(center + Vector3.right * offset);
    }

    private void SpawnPreview(Vector3 pos)
    {
        bool available = true;

        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        Debug.Log($"=== SpawnPreview at {pos}, hits: {hits.Length} ===");

        foreach (var h in hits)
        {
            Debug.Log($"  Hit: {h.gameObject.name}, isTrigger: {h.isTrigger}");
            if (h == null) continue;

            // игнорируем всё что относится к системе размещения
            if (h.GetComponent<ResourceNode>() != null) continue;
            if (h.GetComponent<PreviewNode>() != null) continue;
            if (h.GetComponent<TerritoryCell>() != null) continue;  
            if (h.GetComponent<FactoryProduction>() != null) continue;

            // ВАЖНО: игнорируем триггеры (UI и т.п.)
            if (h.isTrigger) continue;

            available = false;
            break;
        }

        GameObject preview = Instantiate(previewPrefab, pos, Quaternion.identity);
        activePreviews.Add(preview);

        var node = preview.GetComponent<PreviewNode>();
        if (node != null)
            node.Init(this, pos, available);
    }

    public void SelectPosition(Vector3 pos)
    {
        bool available = true;

        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (var h in hits)
        {
            Debug.Log($"SelectPosition Hit: {h.gameObject.name}, isTrigger: {h.isTrigger}");

            if (h == null) continue;
            if (h.isTrigger) continue;
            if (h.transform.IsChildOf(transform) || h.transform == transform) continue;
            if (h.GetComponent<PreviewNode>() != null) continue;
            if (h.GetComponent<TerritoryCell>() != null) continue;

            Debug.Log($">>> BLOCKED BY: {h.gameObject.name}");
            available = false;
            break;
        }

        Debug.Log($"SelectPosition available: {available}");

        if (available)
            Instantiate(placePrefab, pos, Quaternion.identity);

        ClearPreviews();
    }

    public void ClearPreviews()
    {
        foreach (var p in activePreviews)
        {
            if (p != null)
                Destroy(p);
        }

        activePreviews.Clear();
    }
}