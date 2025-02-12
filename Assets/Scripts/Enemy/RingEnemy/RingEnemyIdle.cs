
using UnityEngine;
using UnityHFSM;

public class RingEnemyIdle : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent>
{
    const float MaxWanderDistance = 20f;
    const float WanderSpeed = 5f;
    private Vector3 targetPosition = Vector3.negativeInfinity;

    public RingEnemyIdle(RingEnemy enemy) : base(enemy)
    {

    }

    private void DetermineNewTargetPos()
    {
        Vector3 normalizedTarget = Random.insideUnitSphere;
        float wanderDist = Random.Range(MaxWanderDistance / 2, MaxWanderDistance);
        if (Physics.SphereCast(new Ray(this.Enemy.transform.position, normalizedTarget), this.Enemy.hitbox.radius * 10f, out RaycastHit hitInfo, wanderDist, Enemy.rb.includeLayers))
        { //Hit, only move halfway to hit point
            Vector3 halfway = (hitInfo.point - this.Enemy.transform.position) / 2;
            this.targetPosition = this.Enemy.transform.position + halfway;
        }
        else
        { //No hit, we can move all the way to our ideal target.
            this.targetPosition = this.Enemy.transform.position + (normalizedTarget * wanderDist);
        }
        Debug.Log("RingEnemy: My ideal position is:" + this.targetPosition);


    }

    private void WalkTowardsTargetPos()
    {
        if (!targetPosition.Equals(Vector3.negativeInfinity))
        {
            Vector3 moveDir = targetPosition - this.Enemy.transform.position;
            this.Enemy.rb.AddForce(moveDir.normalized * WanderSpeed, ForceMode.Force);
            Debug.DrawLine(Enemy.transform.position, targetPosition, Color.red, 0.01f);
        }
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (targetPosition.Equals(Vector3.negativeInfinity) || Vector3.Distance(this.Enemy.transform.position, targetPosition) < 2f)
        {
            Debug.Log("Want to determine new position.");
            DetermineNewTargetPos();
            //Enemy.rb.velocity = Vector3.zero;
        }
        else
        {
            WalkTowardsTargetPos();
        }
    }



    public override void OnUpdate()
    {
        base.OnUpdate();

        Enemy.OuterOrientation.Advance(Time.deltaTime);
        Enemy.MiddleOrientation.Advance(Time.deltaTime);
        Enemy.InnerOrientation.Advance(Time.deltaTime);

        Enemy.outerTransform.localRotation = Enemy.OuterOrientation.Sample();
        Enemy.middleTransform.localRotation = Enemy.outerTransform.localRotation * Enemy.MiddleOrientation.Sample();
        Enemy.innerTransform.localRotation = Enemy.middleTransform.localRotation * Enemy.InnerOrientation.Sample();
    }
}
