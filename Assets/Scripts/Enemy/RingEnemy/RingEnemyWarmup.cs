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
    }
    public override void OnExit()
    {
        IsDone = false;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        Vector3 targetDirection = this.Enemy.lastSeenPosition - this.Enemy.transform.position;
        // TODO
    }

    private IEnumerator WarmupTimer()
    {
        IsDone = false;
        yield return new WaitForSeconds(warmupDuration * Random.Range(0.9f, 2.2f));
        IsDone = true;

    }
}
