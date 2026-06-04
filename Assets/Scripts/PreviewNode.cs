using UnityEngine;

public class PreviewNode : MonoBehaviour
{
    private FactoryProduction owner;
    private Vector3 pos;
    private bool available = true;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color allowedColor = new Color(0f, 1f, 0f, 0.6f);
    [SerializeField] private Color blockedColor = new Color(1f, 0f, 0f, 0.6f);

    public void Init(FactoryProduction owner, Vector3 pos, bool available)
    {
        this.owner = owner;
        this.pos = pos;
        this.available = available;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.color = available ? allowedColor : blockedColor;
    }

    private void OnMouseDown()
    {
        if (owner == null) return;
        if (!available) return;

        owner.SelectPosition(pos);
    }
}