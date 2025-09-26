using UnityEngine;
[CreateAssetMenu(fileName = "New Bazooka Ability", menuName = "Abilities/Bazooka")]
public class BazookaAbility : Ability
{
    public GameObject rocketPrefab;
    public float explosionRadius;
    public float rocketSpeed;

    public override bool CanStart(AbilityContext2D ctx)
    {
        return ctx.playerHealth != null && ctx.playerHealth.currentHealth > 0;
    }

    public override void OnStart(AbilityContext2D ctx)
    {
        Vector2 shootDir = ctx.inputDir.normalized;
        if (shootDir == Vector2.zero) shootDir = ctx.facingDir * 1f;
        Vector2 spawnPos = (Vector2)ctx.body.transform.position + shootDir * 1f;
        GameObject rocket = GameObject.Instantiate(rocketPrefab, spawnPos, Quaternion.identity);
        var rb = rocket.GetComponent<Rigidbody2D>();
        var projectile = rocket.GetComponent<Projectile>();
        projectile.teamOwner = ctx.caster.tag;
        projectile.direction = shootDir;
        projectile.speed = rocketSpeed;
    }
}
