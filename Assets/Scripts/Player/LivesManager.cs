using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class LivesManager : MonoBehaviour
{
    [Header("Lives")]
    public int totalLives = 3;
    public int currentLives;

    public event Action<int,int> OnLivesChanged; // (current, total)
    public event Action OnOutOfLives;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        currentLives = totalLives;
    }

    void Start()
    {
        if (health != null)
            health.OnDeath += HandleHealthDeath;
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleHealthDeath;
    }

    private void HandleHealthDeath()
    {
        currentLives = Mathf.Max(0, currentLives - 1);
        OnLivesChanged?.Invoke(currentLives, totalLives);

        if (currentLives <= 0)
        {
            OnOutOfLives?.Invoke();
            // Dejar que otros sistemas manejen el Game Over
        }
        else
        {
            // Resetear vida para la siguiente vida
            health.currentHealth = health.maxHealth;
        }
    }

    public void AddLife(int amount = 1)
    {
        currentLives = Mathf.Min(totalLives, currentLives + amount);
        OnLivesChanged?.Invoke(currentLives, totalLives);
    }
}
