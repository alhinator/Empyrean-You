using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    CombatEntity Owner;

    public virtual bool HitDetected(DamageInstance d)
    {
        //Return false if this hit should be nullified.
        return true;
    }

    /// <summary>
    /// Called by Owner when one of Owner's weapons hits something
    /// </summary>
    /// <param name="d"></param>
    public virtual void OnHit(DamageInstance d)
    {

    }
    /// <summary>
    /// Called by Owner when one of Owner's weapons kills something
    /// </summary>
    /// <param name="d"></param>
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
    /// Called by Owner when Owner may be killed. Return false if Owner should not actually be killed.
    /// </summary>
    public virtual bool OnDeath(DamageInstance d)
    {
        return true;
    }
}