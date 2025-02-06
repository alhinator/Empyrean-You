using UnityEditor;
using UnityEngine;

/// <summary>
/// The abstract parent class for all entities that need to engage in combat.
/// </summary>
public abstract class CombatEntity : MonoBehaviour
{
    // ----- Properties -----

    /// <summary>
    /// The display name of this Combat Entity
    /// </summary>
    protected string displayName;
    /// <summary>
    /// The abilities that will be processed when this entity engages in combat
    /// </summary>
    protected Ability[] Abilities;
    /// <summary>
    /// The weapons this CombatEntity has access to.
    /// </summary>
    protected Weapon[] Weapons;

    /// <summary>
    /// The current HP of this combat entity.
    /// </summary>
    protected float maxHP;
    /// <summary>
    /// The maximum HP of this combat entity.
    /// </summary>
    protected float currHP;


    // ----- Public Setters / Getters -----
    public float CurrentHP
    {
        get
        {
            return currHP;
        }
    }
    public float MaximumHP
    {
        get
        {
            return maxHP;
        }
    }


    // ----- Public Damage-pipeline methods -----

    /// <summary>
    /// Called externally when a DamageInstance is created with this CombatEntity as its target. Searches through its abilities to verify that the hit should be processed.
    /// </summary>
    /// <returns></returns>
    public virtual bool HitDetected(DamageInstance d)
    {
        bool allowed = true;
        foreach (Ability a in Abilities)
        {
            allowed = allowed && a.HitDetected(d);
        }
        return allowed;
    }

    /// <summary>
    /// Called externally when a DamageInstance is created with this CombatEntity as its source. Searches through its weapons and abilities to apply on-hit effects to the DamageInstance.
    /// </summary>
    public virtual void OnHit(DamageInstance d)
    {
        foreach (Weapon w in Weapons)
        {
            if (w == d.AttackingWeapon)
            {
                w.OnHit(d);
            }
        }
        foreach (Ability a in Abilities)
        {
            a.OnHit(d);
        }
    }
    /// <summary>
    /// Called externally when a DamageInstance determines its target has been confirmed killed. Searches through its weapons and abilities to apply on-kill effects to the DamageInstance.
    /// </summary>
    public virtual void OnKill(DamageInstance d)
    {
        foreach (Weapon w in Weapons)
        {
            if (w == d.AttackingWeapon)
            {
                w.OnKill(d);
            }
        }
        foreach (Ability a in Abilities)
        {
            a.OnKill(d);
        }
    }

    /// <summary>
    /// Called externally by a DamageInstance after this CombatEntity determines that a hit was valid and the Source's on-hit effects have been applied.
    /// </summary>
    /// <returns> True if the damage kills this CombatEntity. Otherwise, false.</returns>
    public virtual bool OnDamage(DamageInstance d)
    {
        //First, call my weapons' and abilities' OnDamage.
        foreach (Weapon w in Weapons)
        {
            w.OnDamage(d);
        }
        foreach (Ability a in Abilities)
        {
            a.OnDamage(d);
        }

        currHP -= d.AdjustedDamage;
        if (currHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Called externally by a DamageInstance when it believes this CombatEntity should die due to damage.
    /// </summary>
    /// <returns>True by default, false if one of this CombatEntity's abilities determines it should neither die nor trigger on-kill effects.</returns>
    public virtual bool OnDeath(DamageInstance d)
    {
        bool actuallyDied = true;
        foreach (Ability a in Abilities)
        {
            actuallyDied = actuallyDied && a.OnDeath(d);
        }
        return actuallyDied;
    }


}