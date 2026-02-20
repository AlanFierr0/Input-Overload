using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerAbilityController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerMovement move;
    [HideInInspector] public Health health;
    
    private Camera mainCamera;
    float activeTimer;
    float nextReadyTime;
    // Fired when the player attempts to use an ability. Handlers can steal or react.
    public static event Action<Ability, AbilitySlot> OnAbilityAttempt;
    
    // Events for HUD updates
    public event Action<AbilitySlot> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;

    [System.Serializable] 
    public class AbilitySlot
    {
        public Ability ability;        
        public KeyCode key;

        public AbilityState state = AbilityState.Ready;
        [HideInInspector] public float activeUntil = 0f;
        [HideInInspector] public float nextReadyTime = 0f;
        // If the ability was stolen by a boss, this marks the slot as stolen
        public bool isStolen = false;
        // Damage to apply to the player when they try to use a stolen ability
        public int stolenDamage = 1;
    }

    public List<AbilitySlot> slots = new();

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (move == null) move = GetComponent<PlayerMovement>();
        if (health == null) health = GetComponent<Health>();
        
        // Obtener la cámara principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
    }
    
    /// <summary>
    /// Calcula la dirección de apuntado desde el jugador hacia el mouse
    /// </summary>
    Vector2 CalculateAimDirection()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return move.facingDir; // Fallback a la dirección de movimiento
            }
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane + 1f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 mouseWorldPos = new Vector2(worldPos.x, worldPos.y);
        
        Vector2 playerPos = rb.position;
        Vector2 direction = (mouseWorldPos - playerPos).normalized;
        
        // Si la dirección es cero (mouse muy cerca del jugador), usar la dirección de movimiento
        if (direction == Vector2.zero)
        {
            direction = move.facingDir;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right; // Fallback a derecha
            }
        }
        
        return direction;
    }

    void Update()
    {
        Vector2 aimDir = CalculateAimDirection();
        
        var ctx = new AbilityContext2D
        {
            caster   = gameObject,
            body     = rb,
            inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            playerHealth   = health,
            facingDir = move.facingDir,
            aimDirection = aimDir
        };

        foreach (var slot in slots)
        {
            // Validar que la tecla no sea WASD
            if (slot.key == KeyCode.W || slot.key == KeyCode.A || slot.key == KeyCode.S || slot.key == KeyCode.D)
            {
                Debug.LogWarning("PlayerAbilityController: La habilidad " + (slot.ability != null ? slot.ability.name : "null") + " está asignada a una tecla WASD. Esta habilidad no se activará.");
                continue;
            }

            switch (slot.state)
        {
            case AbilityState.Ready:
                if (Input.GetKeyDown(slot.key) && Time.time >= nextReadyTime)
                {
                    // Notify listeners that the player attempted to use this ability (ability may be null if already stolen)
                    OnAbilityAttempt?.Invoke(slot.ability, slot);

                    // If the slot was marked as stolen, apply damage and skip execution
                    if (slot.isStolen)
                    {
                        Debug.Log("PAC Ability " + (slot.ability != null ? slot.ability.name : "(stolen)") + " is stolen — applying damage to player.");
                        if (health != null)
                        {
                            health.TakeDamage(slot.stolenDamage);
                        }
                        break;
                    }

                    // If ability exists and can start, execute it
                    if (slot.ability != null && slot.ability.CanStart(ctx))
                    {
                        Debug.Log("PAC Ability " + slot.ability.name + " started.");
                        slot.ability.OnStart(ctx);

                        if (slot.ability.abilityDuration > 0f)
                        {
                            activeTimer = slot.ability.abilityDuration;
                            if (move) move.lockMovement = true;
                            slot.state = AbilityState.Active;
                        }
                        else
                        {
                            nextReadyTime = Time.time + slot.ability.abilityCooldown;
                            slot.state = AbilityState.Cooldown;
                        }
                    }
                }
                break;

            case AbilityState.Active:
                slot.ability.OnUpdate(ctx, Time.deltaTime);

                activeTimer -= Time.deltaTime;
                if (activeTimer <= 0f)
                {
                    if (move) move.lockMovement = false; 
                    slot.ability.OnEnd(ctx, interrupted: false);

                    nextReadyTime = Time.time + slot.ability.abilityCooldown;
                    slot.state = AbilityState.Cooldown;
                }
                break;

            case AbilityState.Cooldown:
                if (Time.time >= nextReadyTime)
                {
                    slot.state = AbilityState.Ready;
                }
                break;
            }
        }
    }
    
    /// <summary>
    /// Removes all abilities from the controller and updates HUD
    /// </summary>
    public void ClearAbilities()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            Ability removedAbility = slots[i].ability;
            slots.RemoveAt(i);
            if (removedAbility != null)
            {
                OnAbilityRemoved?.Invoke(removedAbility);
            }
        }
    }

    /// <summary>
    /// Adds an ability to the controller and fires OnAbilityAdded event for HUD updates
    /// </summary>
    public void AddAbility(AbilitySlot abilitySlot)
    {
        slots.Add(abilitySlot);
        OnAbilityAdded?.Invoke(abilitySlot);
    }
    
    /// <summary>
    /// Removes an ability from the controller and fires OnAbilityRemoved event for HUD updates
    /// </summary>
    public void RemoveAbility(Ability ability)
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].ability == ability)
            {
                slots.RemoveAt(i);
                OnAbilityRemoved?.Invoke(ability);
                return;
            }
        }
    }
}
