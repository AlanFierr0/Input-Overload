using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
                _instance = FindFirstObjectByType<AbilityPoolManager>();
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
    [Tooltip("Lista completa de habilidades para auto-llenar el pool")]
    public List<Ability> allAbilities = new();
    private List<Ability> initialAvailableAbilities = new();

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
            return;
        }

        EnsureAllAbilitiesList();

        if (availableAbilities == null)
        {
            availableAbilities = new List<Ability>();
        }

        if (availableAbilities.Count < 3 && allAbilities != null && allAbilities.Count > 0)
        {
            availableAbilities = new List<Ability>(allAbilities);
        }

        if (initialAvailableAbilities == null || initialAvailableAbilities.Count == 0)
        {
            initialAvailableAbilities = new List<Ability>(availableAbilities);
        }
    }

    private void EnsureAllAbilitiesList()
    {
#if UNITY_EDITOR
        if (allAbilities == null || allAbilities.Count == 0)
        {
            allAbilities = new List<Ability>();
            string[] guids = AssetDatabase.FindAssets("t:Ability");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Ability ability = AssetDatabase.LoadAssetAtPath<Ability>(path);
                if (ability != null && !allAbilities.Contains(ability))
                {
                    allAbilities.Add(ability);
                }
            }
        }
#endif

        if ((allAbilities == null || allAbilities.Count == 0) && availableAbilities != null && availableAbilities.Count > 0)
        {
            allAbilities = new List<Ability>(availableAbilities);
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

    /// <summary>
    /// Restaura el pool inicial de habilidades (por ejemplo al perder la partida)
    /// </summary>
    public void ResetPool()
    {
        availableAbilities = new List<Ability>(initialAvailableAbilities);
    }
}

