using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
public class RingEnemy : CombatEntity
{
    void Start()
    {
        maxHP = 200;
        currHP = MaximumHP;
        this.Abilities = new Ability[0];
    }
    public override bool HitDetected(DamageInstance d)
    {
        return true; //We have no need to avoid being hit w/ ring enemy logic
    }
    public override bool OnDamage(DamageInstance d)
    {
        //ring enemy has no sort of damage reductions.
        //Debug.Log("took " + d.AdjustedDamage);
        currHP -= d.AdjustedDamage;
        if (CurrentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool OnDeath(DamageInstance d)
    {
        Debug.Log("my name is " + transform.name + " and i just died");
        Destroy(this.gameObject);
        return true;

    }

}