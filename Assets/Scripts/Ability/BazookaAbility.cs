using UnityEngine;
[CreateAssetMenu(fileName = "New Bazooka Ability", menuName = "Abilities/Bazooka")]
public class BazookaAbility : Ability
{
    public GameObject rocketPrefab;
    public float explosionRadius;
    public float rocketSpeed;
    public int rocketDamage = 5; // Daño del cohete (configurable en Inspector)

    public override bool CanStart(AbilityContext2D ctx)
    {
        return ctx.playerHealth != null && ctx.playerHealth.currentHealth > 0;
    }

    public override void OnStart(AbilityContext2D ctx)
    {
        // Usar SOLO la dirección de apuntado (mouse) - NO usar inputDir (WASD)
        Vector2 shootDir = ctx.aimDirection;
        if (shootDir == Vector2.zero)
        {
            // Solo usar facingDir como fallback, nunca inputDir
            shootDir = ctx.facingDir;
            if (shootDir == Vector2.zero) shootDir = Vector2.right; // Fallback final
        }
        
        Vector2 spawnPos = (Vector2)ctx.body.transform.position + shootDir * 1f;
        GameObject rocket = GameObject.Instantiate(rocketPrefab, spawnPos, Quaternion.identity);
        var rb = rocket.GetComponent<Rigidbody2D>();
        var projectile = rocket.GetComponent<Projectile>();
        projectile.teamOwner = ctx.caster.tag;
        projectile.direction = shootDir;
        projectile.speed = rocketSpeed;
        projectile.damage = rocketDamage; // Asignar el daño configurable
    }
}
