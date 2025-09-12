using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;
    public bool vulnerable = true;

    public void TakeDamage(int damage)
    {
        if (!vulnerable) return;
        else
            maxHealth -= damage;
        
    }
    public void Heal(int amount)
    {
        
            maxHealth += amount;
    }
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
    if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void ChangeVulnerability(bool state)
    {
        vulnerable = state;
    }
}
