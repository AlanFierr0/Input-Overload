using UnityEngine;
[CreateAssetMenu(fileName = "New Sentry Gun", menuName = "Ability/Sentry Gun")]
public class SentryGun : Ability
{
    public float range;
    public int damage;
    public float fireRate;
    public float bulletSpeed;
    public GameObject sentryGunPrefab;
    public GameObject bulletPrefab;
    public override void OnStart(AbilityContext2D ctx)
    {
        GameObject sentryGun = Instantiate(sentryGunPrefab, ctx.caster.transform.position, Quaternion.identity);
        SentryGunBehaviour sentryGunBehaviour = sentryGun.GetComponent<SentryGunBehaviour>();
        sentryGunBehaviour.range = range;
        sentryGunBehaviour.damage = damage;
        sentryGunBehaviour.fireRate = fireRate;
        sentryGunBehaviour.bulletPrefab = bulletPrefab;
        sentryGunBehaviour.bulletSpeed = bulletSpeed;
        sentryGunBehaviour.playerTag = ctx.caster.tag;
        Rigidbody2D rb = sentryGun.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

    }
}


