using UnityEngine;

public class Tomahawk : Projectile
{
    public override void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (!hitInfo.collider.CompareTag("Bullet") && !hitInfo.collider.CompareTag("Structure"))
        {
            Health health = hitInfo.collider.GetComponent<Health>();
            health.TakeDamage(damage);
        }
    }
}
