using UnityEngine;

// Компонент для префабов добытчиков. При размещении добытчик сам определяет тип ресурса под ним
public class Miner : MonoBehaviour
{
    public ResourceNode claimedResource;
    public ResourceType resourceType;

    // Инициализация добытчика: привязать к ресурсной ноде
    public void Claim(ResourceNode node)
    {
        if (node == null) return;
        claimedResource = node;
        // Попытаемся определить тип ресурса сначала по тегу объекта, затем по полю в node
        string tg = node.gameObject.tag != null ? node.gameObject.tag.ToLower() : string.Empty;
        if (tg.Contains("iron")) resourceType = ResourceType.Iron;
        else if (tg.Contains("gold")) resourceType = ResourceType.Gold;
        else if (tg.Contains("titanium")) resourceType = ResourceType.Titanium;
        else if (tg.Contains("oil")) resourceType = ResourceType.Oil;
        else if (tg.Contains("material")) resourceType = ResourceType.Materials;
        else resourceType = node.resourceType;

        node.resourceType = resourceType;
        node.occupied = true;
    }
}
