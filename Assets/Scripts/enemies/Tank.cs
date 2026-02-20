using UnityEngine;

public class Tank : Enemy
{
    [HideInInspector] public Rigidbody2D tank;
    public float attackCooldown;
    private float attackTimer;
    public float knockbackForce = 5f;
    private float knockbackDuration = 0.3f; // Duración del knockback antes de reanudar movimiento
    private float knockbackTimer = 0f;

    void Start()
    {
        if (tank == null) 
        {
            tank = GetComponent<Rigidbody2D>();
        }
        attackTimer = 0f;
    }
    
    public override void Move(){
        // No moverse mientras recibe knockback
        if (knockbackTimer > 0f) return;
        
        Vector2 direction = (tank.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(tank.position, player.position);
        tank.MovePosition(tank.position - direction * speed * Time.fixedDeltaTime);
    }

    public override void Attack()
    {
        attackTimer -= Time.deltaTime;
        knockbackTimer -= Time.deltaTime; // Reducir el timer de knockback cada frame
        
        float distanceToPlayer = Vector2.Distance(tank.position, player.position);
        
        // Distancia de ataque aumentada a 2.5f para más probabilidades de atacar
        if (distanceToPlayer < 2.5f && attackTimer <= 0f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            // Calcular dirección del knockback
            Vector2 knockbackDirection = ((Vector2)player.position - tank.position).normalized;
            
            // Knockback al player hacia atrás (ajustado por masa para fuerza igual)
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(knockbackDirection * knockbackForce * playerRb.mass, ForceMode2D.Impulse);
            }
            
            // Knockback al tanque hacia atrás (dirección opuesta, ajustado por masa)
            tank.AddForce(-knockbackDirection * knockbackForce * tank.mass, ForceMode2D.Impulse);
            
            // Activar el timer de knockback para pausar el movimiento
            knockbackTimer = knockbackDuration;
            
            attackTimer = attackCooldown;
        }
    }


    void FixedUpdate()
    {
        if (player == null) return; // Seguridad
        
        Move();
        Attack();
    }
    
}
