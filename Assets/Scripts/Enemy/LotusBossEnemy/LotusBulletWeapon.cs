using UnityEngine;
public class LotusBulletWeapon : Weapon {
    public float explosionRadius;
    public override void Shoot(){
        Collider[] hits = Physics.OverlapSphere(transform.position,  explosionRadius, LayerMask.GetMask("Player"));
        foreach( var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                new DamageInstance(this.Owner, this, hit.GetComponent<CombatEntity>());
            }
        }
    }
}