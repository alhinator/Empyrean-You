using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : CombatEntity
{
    public override bool OnDeath(DamageInstance d)
    {
        Destroy(this.gameObject);
        return base.OnDeath(d);
    }
}
