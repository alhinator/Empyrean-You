using System.ComponentModel;
using UnityEngine;
using UnityEngine.Android;

public abstract class Weapon : MonoBehaviour
{
    [Header("Base Weapon Properties")]
    /// <summary>
    /// The display name of this source of damage
    /// </summary>
    public string displayName;
    /// <summary>
    /// Damage dealt on hit
    /// </summary>
    public float Damage;
    /// <summary>
    /// The Combat Entity this Weapon is tied to.
    /// </summary>
    protected CombatEntity Owner;

    /// <summary>
    /// Called when whatever controls this Weapon's firing system decides it's time to shoot.
    /// </summary>
    protected abstract void Shoot();
    //If... my raycast or projectile hits...
    //new DamageInstance(this.Owner, this, this.damage, hit.point.getComponent<CombatEntity>);

    /// <summary>
    /// Called by Owner when it detects this weapon has successfully hit a target.
    /// Param d will already have the correct damage amount for this gun.
    /// </summary>
    public virtual void OnHit(DamageInstance d)
    {

    }
    /// <summary>
    /// Called by Owner when it detects this weapon has successfully killed a target.
    /// </summary>
    public virtual void OnKill(DamageInstance d)
    {

    }

    /// <summary>
    /// Called by Owner when Owner has been hit.
    /// </summary>
    public virtual void OnDamage(DamageInstance d)
    {

    }

    /// <summary>
    /// Called by Owner when Owner has been killed.
    /// </summary>
    public virtual void OnDeath(DamageInstance d)
    {

    }



    public string Name
    {
        get
        {
            return displayName;
        }
    }
    public CombatEntity GetOwner
    {
        get
        {
            return Owner;
        }
    }

}