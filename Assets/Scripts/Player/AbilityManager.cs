using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public PlayerAbilityController abilityController;
    public LevelUpUI levelUpUI;

    // Slots removidos - ahora solo se manejan en PlayerAbilityController

    public List<KeyCode> predefinedKeys = new()
    {
    KeyCode.B, KeyCode.C, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
    KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.T,
    KeyCode.U, KeyCode.V, KeyCode.X, KeyCode.Y, KeyCode.Z,

    KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
    KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,

    KeyCode.LeftShift, KeyCode.RightShift,
    KeyCode.LeftAlt, KeyCode.RightAlt,
    KeyCode.LeftControl, KeyCode.RightControl,
    KeyCode.Tab,

    KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
    KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12
    };
    // El pool de habilidades ahora está en AbilityPoolManager (global)

    void Start()
    {
        // Buscar abilityController automáticamente si no está asignado
        if (abilityController == null)
        {
            abilityController = GetComponent<PlayerAbilityController>();
            if (abilityController == null)
            {
                abilityController = FindFirstObjectByType<PlayerAbilityController>();
            }
        }

        if (abilityController == null)
        {
            Debug.LogError("AbilityManager: No PlayerAbilityController found! Abilities cannot be added to player.");
        }

        // Buscar LevelUpUI automáticamente si no está asignado
        if (levelUpUI == null)
        {
            levelUpUI = FindFirstObjectByType<LevelUpUI>();
        }

        if (levelUpUI != null)
        {
            levelUpUI.Initialize(this);
        }
        else
        {
            Debug.LogWarning("AbilityManager: No LevelUpUI found in scene. Level up UI will not be displayed.");
        }
    }

    public void AddAbility()
    {
        AbilityPoolManager poolManager = AbilityPoolManager.Instance;
        
        if (poolManager == null || !poolManager.HasAvailableAbilities())
        {
            Debug.LogWarning("AbilityManager: No abilities available in pool!");
            return;
        }

        if (predefinedKeys.Count == 0)
        {
            Debug.LogWarning("AbilityManager: No available keybinds!");
            return;
        }

        // Generar 3 opciones de habilidades aleatorias
        List<LevelUpUI.AbilityOption> options = GenerateAbilityOptions(3);

        // Si hay UI de level up, mostrarla
        if (levelUpUI != null)
        {
            levelUpUI.ShowLevelUpUI(options);
        }
        else
        {
            // Fallback: asignar la primera opción automáticamente
            Debug.LogWarning("AbilityManager: No LevelUpUI assigned, auto-selecting first ability");
            AddSelectedAbility(options[0].ability, options[0].keybind);
        }
    }

    List<LevelUpUI.AbilityOption> GenerateAbilityOptions(int count)
    {
        List<LevelUpUI.AbilityOption> options = new List<LevelUpUI.AbilityOption>();
        
        AbilityPoolManager poolManager = AbilityPoolManager.Instance;
        if (poolManager == null)
        {
            Debug.LogError("AbilityManager: AbilityPoolManager not found!");
            return options;
        }
        
        // Crear una copia de las habilidades disponibles para no modificar las originales
        List<Ability> abilitiesCopy = poolManager.GetAvailableAbilities();
        List<KeyCode> availableKeys = new List<KeyCode>(predefinedKeys);

        // FILTRAR EXPLÍCITAMENTE LAS TECLAS WASD ANTES DE GENERAR OPCIONES
        availableKeys.RemoveAll(key => 
            key == KeyCode.W || 
            key == KeyCode.A || 
            key == KeyCode.S || 
            key == KeyCode.D
        );

        // Verificar que quedan teclas disponibles después del filtrado
        if (availableKeys.Count == 0)
        {
            Debug.LogError("AbilityManager: No hay teclas disponibles después de filtrar WASD!");
            return options;
        }

        // Asegurarse de no exceder el número de habilidades disponibles
        int optionsCount = Mathf.Min(count, abilitiesCopy.Count, availableKeys.Count);

        for (int i = 0; i < optionsCount; i++)
        {
            if (abilitiesCopy.Count == 0 || availableKeys.Count == 0)
                break;

            LevelUpUI.AbilityOption option = new LevelUpUI.AbilityOption();
            
            // Seleccionar habilidad aleatoria de las disponibles
            int abilityIndex = Random.Range(0, abilitiesCopy.Count);
            option.ability = abilitiesCopy[abilityIndex];
            // Remover solo de la lista temporal para evitar duplicados en las opciones
            abilitiesCopy.RemoveAt(abilityIndex);

            // Seleccionar keybind aleatorio de las disponibles
            int keyIndex = Random.Range(0, availableKeys.Count);
            option.keybind = availableKeys[keyIndex];
            // Remover solo de la lista temporal para evitar duplicados en las opciones
            availableKeys.RemoveAt(keyIndex);

            options.Add(option);
        }

        return options;
    }

    public void AddSelectedAbility(Ability ability, KeyCode key)
    {
        if (ability == null)
        {
            Debug.LogError("AbilityManager: Cannot add null ability!");
            return;
        }

        // Validar que no se usen las teclas WASD (movimiento)
        if (key == KeyCode.W || key == KeyCode.A || key == KeyCode.S || key == KeyCode.D)
        {
            Debug.LogWarning("AbilityManager: Las teclas WASD no pueden usarse para habilidades. Usando una tecla alternativa.");
            // Buscar una tecla alternativa disponible
            if (predefinedKeys.Count > 0)
            {
                key = predefinedKeys[0];
            }
            else
            {
                Debug.LogError("AbilityManager: No hay teclas disponibles para asignar la habilidad!");
                return;
            }
        }

        // Verificar si la habilidad ya está en el controlador (evitar duplicados)
        bool abilityAlreadyExists = false;
        bool keyAlreadyInUse = false;
        
        if (abilityController != null)
        {
            foreach (var slot in abilityController.slots)
            {
                if (slot.ability == ability)
                {
                    abilityAlreadyExists = true;
                    Debug.LogWarning("AbilityManager: Ability " + (string.IsNullOrEmpty(ability.abilityName) ? ability.name : ability.abilityName) + " is already in PlayerAbilityController!");
                    break;
                }
                if (slot.key == key)
                {
                    keyAlreadyInUse = true;
                    Debug.LogWarning("AbilityManager: Key " + key + " is already assigned to another ability!");
                    break;
                }
            }
        }

        // Si la habilidad o la tecla ya están en uso, no agregar
        if (abilityAlreadyExists || keyAlreadyInUse)
        {
            return;
        }

        // Remover la habilidad del pool global (solo si está ahí)
        AbilityPoolManager poolManager = AbilityPoolManager.Instance;
        if (poolManager != null)
        {
            poolManager.RemoveAbility(ability);
        }

        // Remover la tecla de la lista de disponibles (solo si está ahí)
        if (predefinedKeys.Contains(key))
        {
            predefinedKeys.Remove(key);
        }

        // Agregar directamente al controlador de habilidades del jugador
        // Intentar buscar el controlador si es null
        if (abilityController == null)
        {
            abilityController = GetComponent<PlayerAbilityController>();
            if (abilityController == null)
            {
                abilityController = FindFirstObjectByType<PlayerAbilityController>();
            }
        }

        if (abilityController != null)
        {
            // Verificar si ya existe en el controlador
            bool existsInController = false;
            foreach (var controllerSlot in abilityController.slots)
            {
                if (controllerSlot.ability == ability && controllerSlot.key == key)
                {
                    existsInController = true;
                    break;
                }
            }

            if (!existsInController)
            {
                abilityController.slots.Add(new PlayerAbilityController.AbilitySlot 
                { 
                    ability = ability, 
                    key = key,
                    state = AbilityState.Ready
                });
                Debug.Log("Gained ability: " + (string.IsNullOrEmpty(ability.abilityName) ? ability.name : ability.abilityName) + " assigned to key: " + key);
            }
            else
            {
                Debug.LogWarning("AbilityManager: Ability already exists in PlayerAbilityController!");
            }
        }
        else
        {
            Debug.LogError("AbilityManager: abilityController is null! Cannot add ability to player. Make sure PlayerAbilityController is attached to the player GameObject.");
        }
    }
}