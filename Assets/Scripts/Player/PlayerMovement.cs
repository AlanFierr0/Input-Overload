using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D player;
    public float moveSpeed;
    public bool lockMovement = false;
    public Vector2 facingDir;
    void Start()
    {
        if (player == null) player = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (lockMovement) return;
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        player.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
        if (moveX != 0 || moveY != 0) facingDir = new Vector2(moveX, moveY).normalized;
    }
}
