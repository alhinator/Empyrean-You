using System.Collections;
using UnityEngine;

public class RingEnemyWarmup : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent> {

    /// <summary>
    /// How long the ring enemy takes to warm up before firing.
    /// </summary>
    private const int BasePhysicsTicks = 60;

    private int totalPhysicsTicks;
    private int elapsedPhysicsTicks;
    public bool IsDone => elapsedPhysicsTicks >= totalPhysicsTicks;

    private Quaternion initialOuter;
    private Quaternion initialMiddle;
    private Quaternion initialInner;
    
    private Quaternion targetOuter;
    private Quaternion targetMiddle;
    private Quaternion targetInner;
    
    public RingEnemyWarmup(RingEnemy enemy) : base(enemy) { }

    public override void OnEnter()
    {
        base.OnEnter();

        totalPhysicsTicks = (int) (BasePhysicsTicks * Random.Range(0.9f, 2.2f));
        elapsedPhysicsTicks = 0;

        this.initialOuter = this.Enemy.outerTransform.localRotation;
        this.initialMiddle = this.Enemy.middleTransform.localRotation;
        this.initialInner = this.Enemy.innerTransform.localRotation;

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

    public override void OnUpdate() {
        base.OnUpdate();
        
        this.Enemy.outerTransform.localRotation =
            Quaternion.Slerp(this.initialOuter, this.targetOuter, this.elapsedPhysicsTicks / (float) totalPhysicsTicks);
        this.Enemy.middleTransform.localRotation =
            Quaternion.Slerp(this.initialMiddle, this.targetMiddle, this.elapsedPhysicsTicks / (float) totalPhysicsTicks);
        this.Enemy.innerTransform.localRotation =
            Quaternion.Slerp(this.initialInner, this.targetInner, this.elapsedPhysicsTicks / (float) totalPhysicsTicks);
    }

    public override void OnLogic() {
        Vector3 currentLookDirection = this.Enemy.aimingDirection * Vector3.forward;
        Vector3 targetDirection = this.Enemy.lastSeenPosition - this.Enemy.transform.position;
        Quaternion adjustment = Quaternion.FromToRotation(currentLookDirection, targetDirection);

        this.Enemy.aimingDirection = adjustment * this.Enemy.aimingDirection;
        
        // This isn't exactly what I had in mind but at least it looks intentional :/
        
        // The outermost ring should be rotated from +Z to point at the player,
        // Then from its new +Z it should be rotated -90deg around +X
        this.targetOuter =
            this.Enemy.aimingDirection *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        // The middle ring should be rotated from +Z to point at the player,
        // Then from that +Z it should be rotated -120deg around +Z
        // Then from that +Z it should be rotated -90deg around +X
        this.targetMiddle =
            this.Enemy.aimingDirection *
            Quaternion.Euler(0.0f, 0.0f, -120.0f) *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        // The inner ring is the same but with +120deg
        this.targetInner =
            this.Enemy.aimingDirection *
            Quaternion.Euler(0.0f, 0.0f, 120.0f) *
            Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        this.elapsedPhysicsTicks += 1;
    }
}
