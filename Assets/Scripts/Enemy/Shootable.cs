using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
   public int MaximumHP;
   protected int currHP;

   public abstract void HitDetected(PlayerController player, Gun incoming);
   protected abstract void OnHit(PlayerController player, Gun incoming);
   protected abstract void OnKill(PlayerController player, Gun incoming);





   public int CurrentHP{
    get{
        return currHP;
    }
   }
}
