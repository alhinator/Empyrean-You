using UnityEngine;
public class RingEnemyAttack : Weapon
{
    public override void Shoot()
    {
        //code taken from artemis.cs, relevant attributions are in that file
        Debug.Log("In REA shoot");
        var Enemy = Owner as RingEnemy;
        Vector3 fireDirection = (Enemy.actualAttackPos - Enemy.transform.position).normalized;
        Physics.Raycast(Enemy.transform.position, fireDirection, out RaycastHit hit, Enemy.AttackRange, LayerMask.GetMask("WalkableTerrain", "CameraObstacle"));
        Vector3 endposition = hit.transform ? hit.point : Enemy.transform.position + fireDirection * Enemy.AttackRange;
        Vector3 startPosition = Enemy.transform.position;
        Vector3 particlePosition = (endposition - startPosition) / 2 + startPosition; // particle system position is delta middle + start position
        float distance = Vector3.Distance(endposition, startPosition) / 2; //distance is half the total distance since line extends both ways
        int numParticles = (int)(distance * 50);

        Enemy.fireParticles.transform.position = particlePosition; //update the system's position
        Enemy.fireParticles.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.

        ParticleSystem.ShapeModule sm = Enemy.fireParticles.shape;
        sm.radius = distance; //adjust the line size

        ParticleSystem.EmissionModule em = Enemy.fireParticles.emission;
        ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
        em.SetBurst(0, b); //set burst to be the number of desired particles.
        Enemy.fireParticles.Play();
       
       //Now RaycastAll to Player layer using endPos as our target position.
        RaycastHit[] hits = Physics.RaycastAll(new Ray(startPosition, (endposition - startPosition).normalized), distance * 2, LayerMask.GetMask("Player"));
        foreach (RaycastHit h in hits){
            h.transform.gameObject.TryGetComponent<CombatEntity>(out CombatEntity tg);
            if(tg != null){
                new DamageInstance(this.Owner, this, tg);
            }
        }
       
    }
}