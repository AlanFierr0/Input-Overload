using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D player;
    public float moveSpeed;
    public bool lockMovement = false;
    public Vector2 facingDir;
    
    [Header("Player Sprites")]
    [Tooltip("Sprite cuando el personaje mira hacia adelante (por defecto)")]
    public Sprite playerSprite;
    
    [Tooltip("Sprite cuando el personaje mira hacia la izquierda")]
    public Sprite playerLeftSprite;
    
    [Tooltip("Sprite cuando el personaje mira hacia la derecha")]
    public Sprite playerRightSprite;
    
    [Tooltip("Sprite cuando el personaje está de espaldas (mirando hacia arriba)")]
    public Sprite playerBackSprite;
    
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    
    void Start()
    {
        if (player == null) player = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Guardar el sprite inicial como sprite por defecto
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
            // Si no se asignó playerSprite, usar el sprite actual
            if (playerSprite == null)
            {
                playerSprite = defaultSprite;
            }
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
        
        // Determinar la dirección predominante
        float absX = Mathf.Abs(moveX);
        float absY = Mathf.Abs(moveY);
        
        // Si el movimiento es principalmente horizontal
        if (absX > absY)
        {
            if (moveX > 0)
            {
                // Moviendo a la derecha
                if (playerRightSprite != null)
                {
                    spriteRenderer.sprite = playerRightSprite;
                    spriteRenderer.flipX = false;
                }
            }
            else if (moveX < 0)
            {
                // Moviendo a la izquierda
                if (playerLeftSprite != null)
                {
                    spriteRenderer.sprite = playerLeftSprite;
                    spriteRenderer.flipX = false;
                }
            }
        }
        // Si el movimiento es principalmente vertical
        else if (absY > absX)
        {
            if (moveY > 0)
            {
                // Moviendo hacia arriba - mostrar de espaldas
                if (playerBackSprite != null)
                {
                    spriteRenderer.sprite = playerBackSprite;
                    spriteRenderer.flipX = false;
                }
            }
            else if (moveY < 0)
            {
                // Moviendo hacia abajo - usar sprite por defecto (frente)
                if (playerSprite != null)
                {
                    spriteRenderer.sprite = playerSprite;
                    spriteRenderer.flipX = false;
                }
            }
        }
        // Si hay movimiento diagonal, priorizar la dirección más fuerte
        else if (absX > 0.1f || absY > 0.1f)
        {
            // En caso de movimiento diagonal, usar la dirección más fuerte
            if (absX >= absY)
            {
                if (moveX > 0 && playerRightSprite != null)
                {
                    spriteRenderer.sprite = playerRightSprite;
                    spriteRenderer.flipX = false;
                }
                else if (moveX < 0 && playerLeftSprite != null)
                {
                    spriteRenderer.sprite = playerLeftSprite;
                    spriteRenderer.flipX = false;
                }
            }
            else
            {
                if (moveY > 0 && playerBackSprite != null)
                {
                    spriteRenderer.sprite = playerBackSprite;
                    spriteRenderer.flipX = false;
                }
                else if (moveY < 0 && playerSprite != null)
                {
                    spriteRenderer.sprite = playerSprite;
                    spriteRenderer.flipX = false;
                }
            }
        }
    }
}
