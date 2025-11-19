using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;
    public bool vulnerable = true;
    
    [Header("Death Settings")]
    [Tooltip("Si es true, destruye el GameObject al morir (para enemigos). Si es false, solo dispara el evento OnDeath (para jugador)")]
    public bool destroyOnDeath = true;
    
    public event Action OnDeath;
    
    void Awake()
    {
        currentHealth = maxHealth;
        
        // Si es el jugador, no destruir al morir (para evitar perder la cámara)
        if (gameObject.CompareTag("Player"))
        {
            destroyOnDeath = false;
        }
    }
    
    public void TakeDamage(int amount)
    {
        if (!vulnerable) return; // No tomar daño si no es vulnerable
        
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                // Si no se destruye (jugador), desactivar visualmente
                DisableVisuals();
            }
        }
    }
    
    /// <summary>
    /// Desactiva los componentes visuales y de control sin destruir el GameObject
    /// </summary>
    void DisableVisuals()
    {
        // Desactivar el SpriteRenderer si existe
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // Desactivar colisiones para que no interfiera
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        // Desactivar movimiento si tiene Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        // Desactivar scripts de control del jugador (nombres comunes)
        string[] scriptsToDisable = { "PlayerMovement", "PlayerAbilityController", "PlayerController", "PlayerInput" };
        foreach (string scriptName in scriptsToDisable)
        {
            MonoBehaviour script = GetComponent(scriptName) as MonoBehaviour;
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
    

    public void ChangeVulnerability(bool state)
    {
        vulnerable = state;
    }
}
