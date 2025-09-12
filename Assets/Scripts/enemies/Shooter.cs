using UnityEngine;

public class Shooter : Enemy
{
    [HideInInspector] public Rigidbody2D shooter;
    public GameObject projectilePrefab;
    public float shootInterval;
    private float shootTimer;
    public float projectileSpeed;

    void Start()
    {
        if (shooter == null) shooter = GetComponent<Rigidbody2D>();
        shootTimer = shootInterval;
    }

    public override void Attack()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            GameObject projectile = Instantiate(projectilePrefab, shooter.position, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody2D>();
            projectile.GetComponent<Projectile>().teamOwner = shooter.gameObject.tag;
            projectile.GetComponent<Projectile>().damage = damage;
            projectile.GetComponent<Projectile>().speed = projectileSpeed;

            Vector2 playerPos2D = (Vector2)player.position;
            projectile.GetComponent<Projectile>().direction = (playerPos2D - shooter.position).normalized;
            shootTimer = 0f;
        }
    }

    public override void Move()
    {
        Vector2 direction = (shooter.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(shooter.position, player.position);

        if (distance < 5f)
        {
            shooter.MovePosition(shooter.position + direction * speed * Time.fixedDeltaTime);
        }
        else if (distance > 10f)
        {
            shooter.MovePosition(shooter.position - direction * speed * Time.fixedDeltaTime);
        }
    }

    void FixedUpdate()
    {
        Move();
        Attack();
    }
}
