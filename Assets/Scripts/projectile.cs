using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody2D rb;
    public string teamOwner;
    public Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        rb.linearVelocity = direction * speed;
    }

    public virtual void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Bullet")) return;
            Health health = hitInfo.GetComponent<Health>();
        if (health != null && !hitInfo.CompareTag(teamOwner))
        {
            health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    void Update()
    {
        if (Mathf.Abs(rb.position.x) > 20 || Mathf.Abs(rb.position.y) > 20)
        {
            Destroy(gameObject);
        }
    }
}
