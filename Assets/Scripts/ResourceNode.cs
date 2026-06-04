using UnityEngine;

public enum ResourceType
{
    Iron,
    Gold,
    Titanium,
    Oil,
    Materials
}

// Компонент, который ставится на префабы ресурсов (их спавнит ResourcesManager)
public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType;
    // Флаг, занят ли ресурс заводом
    public bool occupied = false;

    public bool IsAvailableFor(ResourceType type)
    {
        return !occupied && resourceType == type;
    }
}
