using UnityEngine;

public class Attackable : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Ищем ближайший объект с AttackMode в активном режиме
        AttackMode[] attackers = FindObjectsByType<AttackMode>(FindObjectsSortMode.None);

        foreach (var attacker in attackers)
        {
            attacker.TryAttack(gameObject);
        }
    }
}