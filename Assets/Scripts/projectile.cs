using UnityEngine;

public class projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody2D rb;
    public string teamOwner;
    public Vector2 direction;

    void Start()
    {
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag(teamOwner)) return;
        Health health = hitInfo.GetComponent<Health>();
        if (health != null)
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
