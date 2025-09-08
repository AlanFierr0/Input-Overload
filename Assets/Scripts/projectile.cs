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

    public virtual void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.collider.CompareTag("Bullet") || hitInfo.collider.CompareTag(teamOwner)) return;
        Health health = hitInfo.collider.GetComponent<Health>();
        if (health != null && !hitInfo.collider.CompareTag(teamOwner))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
        Debug.Log($"Projectile hit {hitInfo.collider.name}");
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
