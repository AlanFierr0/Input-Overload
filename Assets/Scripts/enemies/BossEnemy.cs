using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Steal Settings")]
    public float stealRange = 8f;
    public int defaultStolenDamage = 1;
    public string bossTag = "Enemy";

    [Header("Sprite Animation")]
    public Sprite spriteIdle;           // Mirando al frente (idle)
    public Sprite spriteRight;          // Mirando a la derecha
    public Sprite spriteLeft;           // Mirando a la izquierda
    public Sprite spriteAttack;         // Atacando
    
    private SpriteRenderer spriteRenderer;
    private Vector2 currentDirection = Vector2.right;
    private bool isAttacking = false;
    private float attackAnimationTimer = 0f;
    private float attackAnimationDuration = 0.3f; // Duración de la animación de ataque

    // Abilities the boss has stolen
    public List<Ability> stolenAbilities = new List<Ability>();
    // Map stolen ability to the player's slot so we can return it later
    private Dictionary<Ability, PlayerAbilityController.AbilitySlot> stolenMap = new Dictionary<Ability, PlayerAbilityController.AbilitySlot>();

    Rigidbody2D rb;
    public float attackCooldown;
    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (string.IsNullOrEmpty(tag)) tag = bossTag; // ensure tag present
        attackTimer = 0f; // Inicializar para que pueda atacar
    }

    void OnEnable()
    {
        PlayerAbilityController.OnAbilityAttempt += HandleAbilityAttempt;
        if (health != null)
            health.OnDeath += OnBossDeath;
    }

    void OnDisable()
    {
        PlayerAbilityController.OnAbilityAttempt -= HandleAbilityAttempt;
        if (health != null)
            health.OnDeath -= OnBossDeath;
    }

    void HandleAbilityAttempt(Ability ability, PlayerAbilityController.AbilitySlot slot)
    {
        if (ability == null || slot == null) return;
        if (slot.isStolen) return; // already stolen
        // If boss already has a stolen ability, do not steal more
        if (stolenAbilities != null && stolenAbilities.Count >= 1) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > stealRange) return; // out of range

        StealAbility(ability, slot);
    }

    public void StealAbility(Ability ability, PlayerAbilityController.AbilitySlot slot)
    {
        if (ability == null || slot == null) return;
        // Only allow stealing one ability
        if (stolenAbilities != null && stolenAbilities.Count >= 1) return;
        if (stolenMap.ContainsKey(ability)) return;

        // remember original slot so we can return later
        stolenMap[ability] = slot;
        stolenAbilities.Add(ability);

        // mark the slot as stolen and remove the ability reference so the player can't use it
        slot.isStolen = true;
        slot.ability = null;

        // set damage on slot if not configured
        if (slot.stolenDamage <= 0) slot.stolenDamage = defaultStolenDamage;

        Debug.Log($"BossEnemy: Stole ability {ability.name} from player slot.");
    }

    public void UseStolenAbility(Ability ability)
    {
        if (ability == null) return;
        if (!stolenAbilities.Contains(ability)) return;

        var playerHealth = player != null ? player.GetComponent<Health>() : null;

        var ctx = new AbilityContext2D
        {
            caster = gameObject,
            body = rb,
            inputDir = Vector2.zero,
            playerHealth = playerHealth,
            facingDir = (player != null) ? (player.position - transform.position).normalized : Vector2.right,
            aimDirection = (player != null) ? (player.position - transform.position).normalized : Vector2.right
        };

        if (!ability.CanStart(ctx)) return;

        Debug.Log($"BossEnemy: Using stolen ability {ability.name}.");
        ability.OnStart(ctx);
    }

    void OnBossDeath()
    {
        ReturnStolenAbilities();
    }

    void ReturnStolenAbilities()
    {
        if (player == null) return;

        var playerController = player.GetComponent<PlayerAbilityController>();
        if (playerController == null) return;

        foreach (var kvp in stolenMap)
        {
            var ability = kvp.Key;
            var slot = kvp.Value;
            if (slot != null)
            {
                slot.ability = ability;
                slot.isStolen = false;
                Debug.Log($"BossEnemy: Returned ability {ability.name} to player slot.");
            }
        }

        stolenMap.Clear();
        stolenAbilities.Clear();
    }

    public override void Attack()
    {
        attackTimer -= Time.deltaTime;

        if (rb == null || player == null) return;

        if (Vector2.Distance(rb.position, player.position) < 1.5f && attackTimer <= 0f)
        {
            player.GetComponent<Health>().TakeDamage(damage);
            attackTimer = attackCooldown;
            
            // Activar animación de ataque
            isAttacking = true;
            attackAnimationTimer = attackAnimationDuration;
        }
    }

    public override void Move()
    {
        if (rb == null || player == null) return;

        // Calcular dirección hacia el jugador
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        currentDirection = directionToPlayer;
        
        // Moverse hacia el jugador
        rb.MovePosition(rb.position + directionToPlayer * speed * Time.fixedDeltaTime);
    }
    
    /// <summary>
    /// Actualiza el sprite basado en la dirección y estado del boss
    /// </summary>
    private void UpdateBossSprite()
    {
        if (spriteRenderer == null) return;
        
        // Si está atacando, mostrar sprite de ataque
        if (isAttacking)
        {
            if (spriteAttack != null)
                spriteRenderer.sprite = spriteAttack;
            
            attackAnimationTimer -= Time.deltaTime;
            if (attackAnimationTimer <= 0f)
            {
                isAttacking = false;
            }
            return;
        }
        
        // Determinar sprite basado en dirección
        float directionAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        
        // Normalizar ángulo a 0-360
        if (directionAngle < 0)
            directionAngle += 360;
        
        // Determinar sprite basado en el ángulo
        // 0° = derecha, 90° = arriba, 180° = izquierda, 270° = abajo
        Sprite targetSprite = spriteIdle; // Por defecto idle
        
        if (directionAngle >= 315 || directionAngle < 45)
        {
            // Mirando a la derecha
            targetSprite = spriteRight != null ? spriteRight : spriteIdle;
        }
        else if (directionAngle >= 45 && directionAngle < 135)
        {
            // Mirando hacia arriba
            targetSprite = spriteIdle != null ? spriteIdle : spriteIdle;
        }
        else if (directionAngle >= 135 && directionAngle < 225)
        {
            // Mirando a la izquierda
            targetSprite = spriteLeft != null ? spriteLeft : spriteIdle;
        }
        else
        {
            // Mirando hacia abajo
            targetSprite = spriteIdle != null ? spriteIdle : spriteIdle;
        }
        
        spriteRenderer.sprite = targetSprite;
    }

    void FixedUpdate()
    {
        Move();
        Attack();
        UpdateBossSprite();
    }
}