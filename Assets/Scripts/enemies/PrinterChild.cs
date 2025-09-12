using UnityEngine;

public class PrinterChild : Enemy
{
    [HideInInspector] public Rigidbody2D printerChild;
    public float attackCooldown;
    private float attackTimer;

    void Start()
    {
        if (printerChild == null) printerChild = GetComponent<Rigidbody2D>();
        attackTimer = attackCooldown;
    }

    public override void Move()
    {
        Vector2 direction = (printerChild.position - (Vector2)player.position).normalized;
        printerChild.MovePosition(printerChild.position - direction * speed * Time.fixedDeltaTime);
    }

    public override void Attack()
    {
        
        attackTimer -= Time.deltaTime;

        if (Vector2.Distance(printerChild.position, player.position) < 1.5f && attackTimer <= 0f)
        {
            player.GetComponent<Health>().TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }

    void FixedUpdate()
    {
        Move();
        Attack();
    }
}
