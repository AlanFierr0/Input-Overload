using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string enemyName;
    [HideInInspector] public Health health;
    public float speed;
    public int damage;
    [HideInInspector] public Transform player;
    public abstract void Attack();
    public abstract void Move();
    public void Die() {
        Object.Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<Health>();
    }
}
