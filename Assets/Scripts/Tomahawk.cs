using UnityEngine;

public class Tomahawk : Projectile
{
    public void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (!hitInfo.collider.CompareTag("Bullet") && !hitInfo.collider.CompareTag("Structure"))
        {
            Health health = hitInfo.collider.GetComponent<Health>();
            Debug.Log("Hit " + hitInfo.collider.name);
            health.TakeDamage(damage);
            Debug.Log("Dealt " + damage + " damage to " + hitInfo.collider.name);
        }
    }
}
