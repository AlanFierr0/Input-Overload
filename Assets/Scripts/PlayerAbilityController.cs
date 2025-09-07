using UnityEngine;
using System.Collections.Generic;

public class PlayerAbilityController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PlayerMovement move;
    [HideInInspector] public Health health;
    
    float activeTimer;
    float nextReadyTime;

    [System.Serializable] 
    public class AbilitySlot
    {
        public Ability ability;        
        public KeyCode key;

        public AbilityState state = AbilityState.Ready;
        [HideInInspector] public float activeUntil = 0f;
        [HideInInspector] public float nextReadyTime = 0f;
    }

    public List<AbilitySlot> slots = new();

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (move == null) move = GetComponent<PlayerMovement>();
        if (health == null) health = GetComponent<Health>();
    }

    void Update()
    {
        var ctx = new AbilityContext2D
        {
            caster   = gameObject,
            body     = rb,
            inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            playerHealth   = health,
            facingDir = move.facingDir
        };

        foreach (var slot in slots)
        {
            switch (slot.state)
        {
            case AbilityState.Ready:
                if (slot.ability != null && Input.GetKeyDown(slot.key) && Time.time >= nextReadyTime && slot.ability.CanStart(ctx))
                {
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
}
