using UnityEngine;

public class SentryGunBehaviour : MonoBehaviour
{
    public float range;
    public int damage;
    public float fireRate;
    public float bulletSpeed;
    public string playerTag;
    public GameObject bulletPrefab;
    private Transform target;
    private float fireRateTimer = 0f;
    private Health targetHealth;
    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float rangeSqr = range * range;
            float distSqr = direction.sqrMagnitude;
            if (distSqr > rangeSqr || targetHealth == null || targetHealth.currentHealth <= 0)
            {
                target = null;
                targetHealth = null;
                return;
            }
            else
            {
                fireRateTimer += Time.deltaTime;
                if (fireRateTimer >= 1f / fireRate)
                {
                    fireRateTimer = 0f;
                    Fire(angle, direction.normalized);
                }
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
        }
        else
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
            float closestDistSqr = Mathf.Infinity;
            Transform closestEnemy = null;
            Health closestHealth = null;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    var h = hit.GetComponent<Health>();
                    if (h != null && h.currentHealth > 0)
                    {
                        float dSqr = (hit.transform.position - transform.position).sqrMagnitude;
                        if (dSqr < closestDistSqr)
                        {
                            closestDistSqr = dSqr;
                            closestEnemy = hit.transform;
                            closestHealth = h;
                        }
                    }
                }
            }
            target = closestEnemy;
            targetHealth = closestHealth;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Fire(float angle, Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + (Vector3)(direction * 1.3f), Quaternion.identity);
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.teamOwner = playerTag;
        projectile.direction = direction;
        projectile.speed = bulletSpeed;
        projectile.damage = damage;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
}
