using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public string enemyName;
    [HideInInspector] public Health health;
    public float speed;
    public int damage;
    public int expReward;
    [HideInInspector] public Transform player;
    public abstract void Attack();
    public abstract void Move();

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<Health>();
    }
    void OnEnable()
    {
        if (health != null)
            health.OnDeath += HandleDeath;
    }

        void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }
    void HandleDeath()
    {
        Destroy(gameObject);
    }
}
