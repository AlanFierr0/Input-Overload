// ...existing code...
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : Projectile
{
    public float explosionRadius = 2f;
    public LayerMask damageableLayers;
    public GameObject explosionEffect;

    public override void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Vector2 hitPosition = hitInfo.transform.position;
        GameObject explosion = Instantiate(explosionEffect, hitPosition, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, 1f);
        Destroy(explosion, 1f);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitPosition, explosionRadius, damageableLayers.value);
        foreach (var hitCollider in hitColliders)
        {
            Health health = hitCollider.GetComponent<Health>();
            if (health != null && !hitCollider.gameObject.CompareTag(teamOwner))
            {
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}