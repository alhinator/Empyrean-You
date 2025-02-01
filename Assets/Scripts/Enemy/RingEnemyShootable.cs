using System.Collections;
using UnityEngine;
public class RingEnemyShootable : Shootable
{
    void Start()
    {
        maxHP = 200;
        currHP = MaximumHP;
    }
    public override void HitDetected(PlayerController player, Gun incoming)
    {
        currHP -= incoming.bulletDamage;
        OnHit(player, incoming);
        if (CurrentHP < 0)
        {
            OnKill(player, incoming);
        }
    }
    protected override void OnHit(PlayerController player, Gun incoming)
    {
        base.OnHit(player, incoming);
    }
    protected override void OnKill(PlayerController player, Gun incoming)
    {
        Debug.Log("my name is " + transform.name + " and i just died");

        base.OnKill(player, incoming);
        Destroy(this.gameObject);

    }

}