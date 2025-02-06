using UnityEngine;
public class DamageInstance
{
    public CombatEntity Source;
    public Weapon AttackingWeapon;
    public float AdjustedDamage;
    public CombatEntity Target;
    public DamageInstance(CombatEntity SourceEntity, Weapon SourceWeapon, CombatEntity AttackTarget)
    {
        Source = SourceEntity;
        AttackingWeapon = SourceWeapon;
        AdjustedDamage = AttackingWeapon.Damage;
        Target = AttackTarget;
        DoDamagePipeline();
    }
    private void DoDamagePipeline()
    {
        //Order of operations as laid out in design doc: 
        // Target.HitDetected ? Should the effects of the hit go through? -> Source.OnHit -> Target.OnDamage -> Did Target die? -> Target.OnDeath -> Source.OnKill
        //Debug.Log("In DoDmgPipeline: before modifiactions, adjustedDamge = " + AdjustedDamage);
        if (Target.HitDetected(this))
        {
            Source.OnHit(this);
            bool killed = Target.OnDamage(this);
            if (killed)
            {
                bool confirmed = Target.OnDeath(this);
                if (confirmed)
                {
                    Source.OnKill(this);
                }
            }
        }
    }
}