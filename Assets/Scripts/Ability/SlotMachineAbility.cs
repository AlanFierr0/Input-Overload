using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Slots { clover, heart, diamond, spade, star, seven, rocket }

[CreateAssetMenu(fileName = "New Slot Machine Ability", menuName = "Abilities/Slot Machine")]

[System.Serializable]
public class SlotConfig
{
    public Slots slot;
    public GameObject projectilePrefab;
    public int jackpotProjectiles;
    public int projectileSpeed;
    public int projectileDamage = 1; // Daño del projectile (configurable)

public class SlotMachineAbility : Ability
{
    public List<SlotConfig> projectileList;
    private Dictionary<Slots, GameObject> slotProjectiles;
    public float jackpotProbability = 0.4f;  // Aumentado a 40% para más probabilidades de jackpot
    public int slotDuration = 1;  // Reducido a 1 segundo para disparar más rápido
    public int numberOfProjectile = 3;  // Aumentado a 3 projectiles por defecto

    private Dictionary<Slots, float> slotProbabilities = new Dictionary<Slots, float>
    {
        { Slots.clover, 0.50f },   // 50%
        { Slots.star,   0.35f },   // 35%
        { Slots.rocket, 0.15f }    // 15% - aumentado rocket para más variedad
    };
    public override bool CanStart(AbilityContext2D ctx)
    {
        return ctx.playerHealth != null && ctx.playerHealth.currentHealth > 0;
    }

    private void OnEnable()
    {
        slotProjectiles = new Dictionary<Slots, GameObject>();
        foreach (var entry in projectileList)
        {
            if (entry != null && entry.projectilePrefab != null)
                slotProjectiles[entry.slot] = entry.projectilePrefab;
        }
    }   

    public override void OnStart(AbilityContext2D ctx)
    {
        var runner = ctx.caster.GetComponent<MonoBehaviour>();
        if (runner == null) return;
        runner.StartCoroutine(SlotMachineCoroutine(ctx));
    }

    private IEnumerator SlotMachineCoroutine(AbilityContext2D ctx)
    {
        //animation or effect can be played here
        Debug.Log("Spinning the slot machine...");
        yield return new WaitForSeconds(slotDuration);
        bool isJackpot = Random.value < jackpotProbability;
        Slots result = GetRandomSlot();
        Debug.Log($"Slot result: {result}");
        int projectilesToShoot = isJackpot ? GetJackpotCount(result) : numberOfProjectile;
        GameObject prefab = slotProjectiles.ContainsKey(result) ? slotProjectiles[result] : null;
        if (projectilesToShoot <= 1)
        {
            Vector2 dir = ctx.facingDir.normalized;
            var go = Object.Instantiate(prefab, (Vector2)ctx.caster.transform.position + dir * 1f, Quaternion.identity);
            var proj = go.GetComponent<Projectile>();
            proj.direction = dir;
            proj.teamOwner = "Player";
            var slotConfig = projectileList.Find(p => p.slot == result);
            proj.speed = slotConfig.projectileSpeed;
            proj.damage = slotConfig.projectileDamage; // Asignar damage
            yield break;
        } else
        {
            ShootInCircle(ctx.caster.transform.position, prefab, projectilesToShoot,result);
        }
    }

    private int GetJackpotCount(Slots slot)
    {
        for (int i = 0; i < projectileList.Count; i++)
        {
            var projectiles = projectileList[i];
            if (projectiles != null && projectiles.slot == slot)
                return projectiles.jackpotProjectiles;
        }
        return numberOfProjectile;
    }

    private Slots GetRandomSlot()
    {
        Slots[] order = { Slots.clover, Slots.star, Slots.rocket };
        float rand = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < order.Length; i++)
        {
            if (!slotProbabilities.TryGetValue(order[i], out float p)) continue;
            cumulative += p;
            if (rand < cumulative) return order[i];
        }
        return order[order.Length - 1];
    }
    private void ShootInCircle(Vector2 origin, GameObject prefab, int count, Slots slot)
    {    
        float angleStep = 360f / count;
        float angleDeg = 0f; 
        var slotConfig = projectileList.Find(p => p.slot == slot);

        for (int i = 0; i < count; i++)
        {
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)).normalized;
            var go = Object.Instantiate(prefab, origin + dir * 1f, Quaternion.identity);
            var proj = go.GetComponent<Projectile>();
            proj.direction = dir;
            proj.teamOwner = "Player";
            proj.speed = slotConfig.projectileSpeed;
            proj.damage = slotConfig.projectileDamage; // Asignar damage
            angleDeg += angleStep;
        }
    }
}
}
