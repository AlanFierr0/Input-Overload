using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody2D rb;
    public string teamOwner;
    // Optional: the tag of the GameObject that spawned this projectile. If set, collisions
    // treat this tag as the friendly team (so instantiators can override hardcoded teamOwner).
    public string instigatorTag;
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
        // Determine which tag should be considered "friendly" for this projectile.
        string friendlyTag = string.IsNullOrEmpty(instigatorTag) ? teamOwner : instigatorTag;

        if (hitInfo.CompareTag("Bullet") || (!string.IsNullOrEmpty(friendlyTag) && hitInfo.CompareTag(friendlyTag)))
        {
            return;
        }

        Health health = hitInfo.GetComponent<Health>();
        if (health != null && (string.IsNullOrEmpty(friendlyTag) || !hitInfo.CompareTag(friendlyTag)))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (Mathf.Abs(rb.position.x) > 10000 || Mathf.Abs(rb.position.y) > 10000)
        {
            Destroy(gameObject);
        }
    }
}
