using UnityEngine;
[CreateAssetMenu(fileName = "New Dash Ability", menuName = "Abilities/Dash")]
public class DashAbility : Ability
{
    public float dashForce;

    public override bool CanStart(AbilityContext2D ctx)
    {
        return ctx.inputDir != Vector2.zero;
    }
    public override void OnStart(AbilityContext2D ctx)
    {
        Vector2 dashDir = ctx.inputDir.normalized;
        ctx.body.AddForce(dashDir * dashForce, ForceMode2D.Impulse);
        ctx.playerHealth.ChangeVulnerability(false);
    }
    public override void OnEnd(AbilityContext2D ctx, bool interrupted)
    {
        ctx.playerHealth.ChangeVulnerability(true);
    }
    
}
