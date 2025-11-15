using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager global que contiene el pool de habilidades disponibles en el juego.
/// Debe estar en un GameObject global de la escena (no en el player).
/// </summary>
public class AbilityPoolManager : MonoBehaviour
{
    private static AbilityPoolManager _instance;
    public static AbilityPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AbilityPoolManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AbilityPoolManager");
                    _instance = go.AddComponent<AbilityPoolManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Available Abilities Pool")]
    [Tooltip("Pool de todas las habilidades disponibles en el juego")]
    public List<Ability> availableAbilities = new();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Obtiene una copia de las habilidades disponibles
    /// </summary>
    public List<Ability> GetAvailableAbilities()
    {
        return new List<Ability>(availableAbilities);
    }

    /// <summary>
    /// Remueve una habilidad del pool (cuando el jugador la obtiene)
    /// </summary>
    public void RemoveAbility(Ability ability)
    {
        if (availableAbilities.Contains(ability))
        {
            availableAbilities.Remove(ability);
        }
    }

    /// <summary>
    /// Verifica si hay habilidades disponibles
    /// </summary>
    public bool HasAvailableAbilities()
    {
        return availableAbilities.Count > 0;
    }

    /// <summary>
    /// Obtiene el número de habilidades disponibles
    /// </summary>
    public int GetAvailableCount()
    {
        return availableAbilities.Count;
    }
}

