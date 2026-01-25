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
    public float deployDistance = 2f; // Distancia adelante del jugador
    
    private GameObject currentSentry;
    
    public override void OnStart(AbilityContext2D ctx)
    {
        // Destruir la torreta anterior si existe
        if (currentSentry != null)
        {
            Destroy(currentSentry);
        }
        
        // Calcular posición adelante del jugador
        Vector2 deployPosition = (Vector2)ctx.caster.transform.position + ctx.facingDir * deployDistance;
        
        GameObject sentryGun = Instantiate(sentryGunPrefab, deployPosition, Quaternion.identity);
        currentSentry = sentryGun;
        
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


