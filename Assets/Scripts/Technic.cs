using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Technic : MonoBehaviour
{
    [SerializeField] internal float hp;
    [SerializeField] internal float damage;
    [SerializeField] internal int cell_speed;
    [SerializeField] internal float cell_size;
    Technic(float hp, float damage, int cell_speed, float cell_size)
    {
        this.hp = hp;
        this.damage = damage;
        this.cell_speed = cell_speed;
        this.cell_size = cell_size;
    }
    internal void Move(Vector2 direction)
    {
        Vector3 TPos = this.transform.position;
        Vector3 NewPos = new Vector3(0, 0, 0);
        NewPos.x = TPos.x + cell_speed * cell_size * direction.x;
        NewPos.y = TPos.y + cell_speed * cell_size * direction.y;
        this.transform.position = NewPos;
    }

}