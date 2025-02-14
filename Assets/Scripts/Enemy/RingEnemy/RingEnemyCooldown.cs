using UnityEngine;

public class RingEnemyCooldown : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent>
{
    public RingEnemyCooldown(RingEnemy enemy) : base(enemy) { }
    private const float CooldownTimer = 2f;
    private float TimeToReEngage = 0f;
    private float recoilForce = 10;


    public override void OnEnter()
    {
        base.OnEnter();
        TimeToReEngage = 0;

        Vector3 recoilDir = (Enemy.actualAttackPos - Enemy.transform.position).normalized * -1;
        Enemy.rb.AddForce(recoilDir * recoilForce, ForceMode.Impulse);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        TimeToReEngage += Time.deltaTime * Random.Range(0.9f, 2.2f);

        //Move the rings back to LocalPosition center and rotation. This will undo any changes in position from other states.
        Enemy.innerTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.innerTransform.localPosition, Vector3.zero, 2 * Time.deltaTime), Quaternion.identity);
        Enemy.middleTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.middleTransform.localPosition, Vector3.zero, 2 * Time.deltaTime), Quaternion.identity);
        Enemy.outerTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.outerTransform.localPosition, Vector3.zero, 2 * Time.deltaTime), Quaternion.identity);


    }

    public bool IsDone
    {
        get
        {
            return TimeToReEngage > CooldownTimer;
        }
    }
}
