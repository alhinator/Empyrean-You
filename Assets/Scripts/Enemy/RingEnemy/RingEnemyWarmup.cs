using System.Collections;
using UnityEngine;

public class RingEnemyWarmup : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent>
{
    // 1. figure out where to store angle for anim. quaternion
    // 2. make constants for physics steps taken until aiming phase
    // 3. figure out how to pass on (1) to aiming phase
    // 4. exit back to idle if lost sight of player - DONE

    /// <summary>
    /// How long the ring enemy takes to warm up before firing.
    /// </summary>
    private float warmupDuration = 1f;
    public bool IsDone = false;

    public RingEnemyWarmup(RingEnemy enemy) : base(enemy) { }

    public override void OnEnter()
    {
        base.OnEnter();

        //Reset warmup duration & start timer. Needs to be called by parent Enemy as this class does not derive monobehaviour
        Enemy.StartCoroutine(WarmupTimer());

        // Grab localRotation of all rings

        // TODO this is wrong and needs to be revised
        // We want to do the following:
        // 1. grab a Vec3 from the sphere to the player
        // 2. set our initial quaternion as AxisAngle(thatVec3, 0)
        // 3. set our rotation angle as 0

        // The target rotation for our rings is as follows:
        // 1. rotate from AxisAngle(near, rotationAngle) to -targetQuaternion by 45deg
        // 2. rotate by targetQuaternion
        // 3. rotate by AxisAngle(near, ringOffset)
        
        // TODO this can cause mega issues if the player is directly above or below
        // This rotates the vector +Z across the sphere to point at the player
        // this.Enemy.aimingDirection =
            // Quaternion.LookRotation(this.Enemy.lastSeenPosition - this.Enemy.transform.position);
    }
    public override void OnExit()
    {
        IsDone = false;
        base.OnExit();
    }

    public override void OnLogic() {
        Vector3 currentLookDirection = this.Enemy.aimingDirection * Vector3.forward;
        Vector3 targetDirection = this.Enemy.lastSeenPosition - this.Enemy.transform.position;
        Quaternion adjustment = Quaternion.FromToRotation(currentLookDirection, targetDirection);

        this.Enemy.aimingDirection = adjustment * this.Enemy.aimingDirection;
        
        // The outermost ring should be rotated from +Z to point at the player,
        // Then from its new +Z it should be rotated -90deg around +X
        this.Enemy.outerTransform.localRotation =
            this.Enemy.aimingDirection *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        // The middle ring should be rotated from +Z to point at the player,
        // Then from that +Z it should be rotated -120deg around +Z
        // Then from that +Z it should be rotated -90deg around +X
        this.Enemy.middleTransform.localRotation =
            this.Enemy.aimingDirection *
            Quaternion.Euler(0.0f, 0.0f, -120.0f) *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        // The inner ring is the same but with +120deg
        this.Enemy.innerTransform.localRotation =
            this.Enemy.aimingDirection *
            Quaternion.Euler(0.0f, 0.0f, 120.0f) *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    }

    private IEnumerator WarmupTimer()
    {
        IsDone = false;
        yield return new WaitForSeconds(warmupDuration * Random.Range(0.9f, 2.2f));
        IsDone = true;

    }
}
