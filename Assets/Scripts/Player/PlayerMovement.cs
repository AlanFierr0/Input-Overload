using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D player;
    public float moveSpeed;
    public bool lockMovement = false;
    public Vector2 facingDir;
    
    public Sprite playerSprite;
    public Sprite playerLeftSprite;
    public Sprite playerRightSprite;
    public Sprite playerBackSprite;
    
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    
    void Start()
    {
        if (player == null) player = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
            if (playerSprite == null) playerSprite = defaultSprite;
        }
    }
    
    void Update()
    {
        if (lockMovement) return;
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        player.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
        
        if (moveX != 0 || moveY != 0)
        {
            facingDir = new Vector2(moveX, moveY).normalized;
            UpdatePlayerSprite(moveX, moveY);
        }
    }
    
    void UpdatePlayerSprite(float moveX, float moveY)
    {
        if (spriteRenderer == null) return;
        
        float absX = Mathf.Abs(moveX);
        float absY = Mathf.Abs(moveY);
        
        spriteRenderer.flipX = false;
        
        if (absX > absY)
        {
            if (moveX > 0 && playerRightSprite != null) spriteRenderer.sprite = playerRightSprite;
            else if (moveX < 0 && playerLeftSprite != null) spriteRenderer.sprite = playerLeftSprite;
        }
        else
        {
            if (moveY > 0 && playerBackSprite != null) spriteRenderer.sprite = playerBackSprite;
            else if (moveY < 0 && playerSprite != null) spriteRenderer.sprite = playerSprite;
        }
    }
}
