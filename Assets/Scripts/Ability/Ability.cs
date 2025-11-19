using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public string abilityDescription;
    public float abilityCooldown;
    public float abilityDuration;
    public enum AbilityType { Passive, Active }
    public AbilityState abilityState;

    
    public virtual bool CanStart(AbilityContext2D ctx) => true;
    
    public virtual void OnStart(AbilityContext2D ctx) { }
    
    public virtual void OnUpdate(AbilityContext2D ctx, float dt) { }

    public virtual void OnEnd(AbilityContext2D ctx, bool interrupted) { }
}

public class AbilityContext2D
{
    public GameObject caster;
    public Rigidbody2D body;  
    public Vector2 inputDir;
    public Health playerHealth;
    public Vector2 facingDir;
    public Vector2 aimDirection; // Dirección hacia donde apunta el mouse (normalizada)
}