using UnityEngine;

public class Tank : Enemy
{
    [HideInInspector] public Rigidbody2D tank;
    public float attackCooldown;
    private float attackTimer;

    void Start()
    {
        if (tank == null) tank = GetComponent<Rigidbody2D>();
    }
    
    public override void Move(){
        Vector2 direction = (tank.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(tank.position, player.position);
        tank.MovePosition(tank.position - direction * speed * Time.fixedDeltaTime);
    }

 public override void Attack()
    {
        
        attackTimer -= Time.deltaTime;

        if (Vector2.Distance(tank.position, player.position) < 1.5f && attackTimer <= 0f)
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
