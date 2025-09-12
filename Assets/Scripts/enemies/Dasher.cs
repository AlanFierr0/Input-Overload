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
    public float dashDelay = 0.25f;     // tiempo de “wind-up”
    public float dashRange = 3f;        // distancia para habilitar dash

    private float lastDashTime = -999f;
    private bool isDashingOrWindup = false; // *** evita mezclar movimiento con dash/windup
    private Vector2 lockedDashDir;          // *** dirección bloqueada para el dashs

    void Start()
    {
        if (dasher == null) dasher = GetComponent<Rigidbody2D>();
    }

    public override void Move()
    {
        Vector2 direction = (dasher.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(dasher.position, player.position);

        if (isDashingOrWindup) return;

        if (distance < dashRange && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(DashAfterDelay(direction));
        }
        else if (distance < dashRange)
        {
            dasher.MovePosition(dasher.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            dasher.MovePosition(dasher.position - direction * speed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator DashAfterDelay(Vector2 dir)
    {
        dasher.linearVelocity = Vector2.zero;
        isDashingOrWindup = true;
        lockedDashDir = dir.normalized;
        yield return new WaitForSeconds(dashDelay);
        dasher.AddForce(-lockedDashDir * dashForce, ForceMode2D.Impulse);
        lastDashTime = Time.time;
        yield return new WaitForSeconds(0.2f); // tiempo de dash
        isDashingOrWindup = false;
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
