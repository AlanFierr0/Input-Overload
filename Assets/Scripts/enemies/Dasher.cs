using System.Collections;
using UnityEngine;

public class Dasher : Enemy
{
    [HideInInspector] public Rigidbody2D dasher;

    [Header("Attack")]
    public float attackCooldown;
    private float attackTimer;

    [Header("Dash")]
    public float dashForce = 10f;
    public float dashCooldown = 2f;
    public float dashDelay = 0.25f;    
    public float dashRange = 3f;        

    private float lastDashTime = -999f;
    private bool isDashingOrWindup = false;
    private Vector2 lockedDashDir;
    private Coroutine dashCoroutine;

    void Start()
    {
        if (dasher == null) dasher = GetComponent<Rigidbody2D>();
        
        // Bloquear rotación en eje Z para evitar que el Dasher rote
        if (dasher != null)
        {
            dasher.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public override void Move()
    {
        Vector2 directionToPlayer = ((Vector2)player.position - dasher.position).normalized;
        float distance = Vector2.Distance(dasher.position, player.position);

        if (isDashingOrWindup) return;

        if (distance < dashRange && Time.time >= lastDashTime + dashCooldown)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            dashCoroutine = StartCoroutine(DashAfterDelay(directionToPlayer));
        }
        else if (distance < dashRange)
        {
            dasher.linearVelocity = Vector2.zero;
        }
        else
        {
            dasher.MovePosition(dasher.position + directionToPlayer * speed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator DashAfterDelay(Vector2 dir)
    {
        isDashingOrWindup = true;
        dasher.linearVelocity = Vector2.zero;
        dasher.constraints = RigidbodyConstraints2D.FreezeAll;
        
        lockedDashDir = dir.normalized;
        yield return new WaitForSeconds(dashDelay);
        
        dasher.constraints = RigidbodyConstraints2D.None;
        dasher.linearVelocity = Vector2.zero;
        dasher.AddForce(lockedDashDir * dashForce, ForceMode2D.Impulse);
        lastDashTime = Time.time;
        
        yield return new WaitForSeconds(0.5f);
        
        isDashingOrWindup = false;
        dasher.linearVelocity = Vector2.zero;
        dashCoroutine = null;
    }

    public override void Attack()
    {

        attackTimer -= Time.deltaTime;

        if (Vector2.Distance(dasher.position, player.position) < 1.5f && attackTimer <= 0f)
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
