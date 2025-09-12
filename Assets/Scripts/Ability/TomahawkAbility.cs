using UnityEngine;
[CreateAssetMenu(fileName = "New Tomahawk Ability", menuName = "Abilities/Tomahawk")]
public class TomahawkAbility : Ability
{
    public Rigidbody2D shooter;
    public GameObject tomahawkPrefab;
    public float throwForce = 10f;
    public int maxTomahawks;

    public override bool CanStart(AbilityContext2D ctx)
    {
        if (abilityCooldown > 0) return false;
        return GameObject.FindGameObjectsWithTag("Tomahawk").Length < maxTomahawks;
    }

    public override void OnStart(AbilityContext2D ctx)
    {
        Vector2 throwDir = ctx.inputDir.normalized;
        if (throwDir == Vector2.zero) throwDir = ctx.facingDir * 1f;
        Vector2 spawnPos = (Vector2)ctx.body.transform.position + throwDir * 1f;
        GameObject tomahawk = GameObject.Instantiate(tomahawkPrefab, spawnPos, Quaternion.identity);
        var rb  = tomahawk.GetComponent<Rigidbody2D>();
        var projectile = tomahawk.GetComponent<Projectile>();
        projectile.teamOwner = ctx.caster.tag;
        projectile.direction = throwDir;
        projectile.speed = throwForce;
    }
}
