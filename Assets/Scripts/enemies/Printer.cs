using UnityEngine;

public class Printer : Enemy
{
    [HideInInspector] public Rigidbody2D printer;
    public GameObject printerChildPrefab;
    public float shootInterval;
    private float shootTimer;
    public float playerDistanceThreshold;

    void Start()
    {
        if (printer == null) printer = GetComponent<Rigidbody2D>();
        shootTimer = shootInterval;
    }

    public override void Attack()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        //spawn child in direction of player
        {
            Vector2 spawnDirection = ((Vector2)player.position - printer.position).normalized;
            Instantiate(printerChildPrefab, printer.position + spawnDirection, Quaternion.identity);
            shootTimer = 0f;
        }
    }

    public override void Move()
    {
        Vector2 direction = (printer.position - (Vector2)player.position).normalized;
        float distance = Vector2.Distance(printer.position, player.position);

        if (distance < playerDistanceThreshold)
        {
            printer.MovePosition(printer.position - direction * speed * Time.fixedDeltaTime);
        }
    }

    void FixedUpdate()
    {
        Move();
        Attack();
    }
    
}
