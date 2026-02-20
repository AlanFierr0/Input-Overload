using UnityEngine;

public class PrinterChild : Enemy
{
    [HideInInspector] public Rigidbody2D printerChild;
    public float attackCooldown;
    private float attackTimer;
    public float knockbackForce = 5f; // Fuerza del knockback

    void Start()
    {
        if (printerChild == null) printerChild = GetComponent<Rigidbody2D>();
        attackTimer = 0f; // Inicializar para que pueda atacar desde el inicio
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
            
            // Calcular dirección del knockback
            Vector2 knockbackDirection = ((Vector2)player.position - printerChild.position).normalized;
            
            // Knockback al player
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
            
            // Destruir la hoja después de golpear
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Move();
        Attack();
    }
}
