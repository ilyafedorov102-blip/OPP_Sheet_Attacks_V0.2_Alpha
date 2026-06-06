using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackMode : MonoBehaviour
{
    private Moves moves;
    [Header("UI")]
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private Button attackButton;

    [Header("Настройки")]
    [SerializeField] private CircleCollider2D attackRadius;
    private string attackableTag; 
    private bool isAttackMode = false;
    private List<GameObject> highlightedTargets = new List<GameObject>();
    private static readonly Color highlightColor = new Color(0f, 1f, 0f, 1f);

    private void Awake()
    {
        moves = FindAnyObjectByType<Moves>();
        if (attackButton != null)
            attackButton.onClick.AddListener(EnterAttackMode);
    }
    private void Start()
    {
        attackableTag = ((moves.isMoveFirst == false) ? "MainBuild1" : "MainBuild2");
        Debug.Log($"Атакуемый чел: {attackableTag}");
    }
    private void Update()
    {
        
    }
    // ==================== UI ====================

    private void EnterAttackMode()
    {
        if (attackPanel != null)
            attackPanel.SetActive(false);

        isAttackMode = true;
        HighlightTargets();
    }

    private void ExitAttackMode()
    {
        isAttackMode = false;
        ClearHighlights();

        if (attackPanel != null)
            attackPanel.SetActive(true);
    }

    // ==================== ПОДСВЕТКА ====================

    private void HighlightTargets()
    {
        ClearHighlights();

        float radius = attackRadius != null ? attackRadius.radius : 1f;
        Vector2 center = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (!hit.CompareTag(attackableTag)) continue;

            var sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = highlightColor;
                highlightedTargets.Add(hit.gameObject);
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var target in highlightedTargets)
        {
            if (target == null) continue;

            var sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.white;
        }

        highlightedTargets.Clear();
    }

    // ==================== АТАКА ====================

    public void TryAttack(GameObject target)
    {
        if (!isAttackMode) return;
        if (!highlightedTargets.Contains(target)) return;

        Destroy(target);

        switch(attackableTag)
        {
            case "MainBuild1":
                moves.attakedBuild = 2; break;
            case "MainBuild2":
                moves.attakedBuild = 1; break;
        }
        ExitAttackMode();
    }
}