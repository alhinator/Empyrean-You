using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
    protected float maxHP;
    protected float currHP;

    public abstract void HitDetected(PlayerController player, Gun incoming);
    protected virtual void OnHit(PlayerController player, Gun incoming)
    {
        player.TriggerOnHitEffects(this);
        incoming.TriggerOnHitEffects(this);
    }
    protected virtual void OnKill(PlayerController player, Gun incoming)
    {
        player.TriggerOnKillEffects(this);
        incoming.TriggerOnKillEffects(this);
    }
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
}
