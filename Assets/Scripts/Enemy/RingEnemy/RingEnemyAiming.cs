using UnityEngine;

public class RingEnemyAiming : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent>
{
    public RingEnemyAiming(RingEnemy enemy) : base(enemy) { }

    private float TimeAiming = 0;
    private float TimeToLock = 2f;
    private float last = 0;
    public override void OnEnter()
    {
        base.OnEnter();
        TimeAiming = 0;
        last = 0;
    }
    public override void OnExit()
    {
        Enemy.aimParticles.enabled = false;
        base.OnExit();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        MoveRingsToFiringPos();
        TimeAiming += Time.deltaTime;



    }
    public override void OnLogic()
    {
        base.OnLogic();
        DoBeamLogic();
    }

    private void MoveRingsToFiringPos()
    {

        Quaternion directionToLook = Quaternion.LookRotation(Enemy.transform.position - Enemy.lastSeenPosition);

        Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, directionToLook, 4 * Time.deltaTime);

        Enemy.innerTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.innerTransform.localPosition, new Vector3(0, 0, 2), 4 * Time.deltaTime),
            Quaternion.Slerp(Enemy.innerTransform.localRotation, Quaternion.identity, 2 * Time.deltaTime));

        Enemy.middleTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.middleTransform.localPosition, new Vector3(-3, 2, 0), 4 * Time.deltaTime),
            Quaternion.Slerp(Enemy.middleTransform.localRotation, new Quaternion(45, 115, 90, 0), 2 * Time.deltaTime));

        Enemy.outerTransform.SetLocalPositionAndRotation(Vector3.Lerp(Enemy.outerTransform.localPosition, new Vector3(3, 2, 0), 4 * Time.deltaTime),
            Quaternion.Slerp(Enemy.outerTransform.localRotation, new Quaternion(-45, 115, 90, 0), 2 * Time.deltaTime));
    }

    private void DoBeamLogic()
    {
        if (!Enemy.isInLoS) { TimeAiming = 0; last = 0; return; }//Don't do beam logic + reset timer if we can't see player
        Enemy.actualAttackPos = Enemy.lastSeenPosition;

        Enemy.aimParticles.SetPosition(0, Enemy.transform.position);
        Enemy.aimParticles.SetPosition(1, Enemy.lastSeenPosition);
        Enemy.aimParticles.startColor = new Color(0.7f, 0, 0, 0.25f);
        Enemy.aimParticles.endColor = new Color(0.7f, 0, 0, 0.25f);

        //start flashing the beam now. Increases speed at half charge and 3/4 charge

        float duration = TimeAiming < TimeToLock / 2 ? 0.5f : TimeAiming < TimeToLock * 3 / 4 ? 0.25f : 0.1f;
        if (TimeAiming > last + duration)
        {
            Enemy.aimParticles.enabled = !Enemy.aimParticles.enabled;
            last = TimeAiming;
        }

    }

    public bool IsDone
    {
        get
        {
            return TimeAiming > TimeToLock;
        }
    }
}
