using UnityEditor.Rendering;
using UnityEngine;
using UnityHFSM;

public class RingEnemyLockon : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent>
{
    public RingEnemyLockon(RingEnemy enemy) : base(enemy) { }
    private const float AimDelay = 0.1f;
    private float TimeTillShoot;
    private bool isDone;

    public override void OnEnter()
    {
        base.OnEnter();
        TimeTillShoot = 0;
        isDone = false;
        Enemy.aimParticles.enabled = true;
        Enemy.aimParticles.startColor = new Color(1f, 0, 0, 0.8f);
        Enemy.aimParticles.endColor = new Color(1f, 0, 0, 0.8f);


    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        TimeTillShoot += Time.deltaTime;
        if (TimeTillShoot > AimDelay)
        {
            TimeTillShoot = -10000;
            DoAttack();
        }
    }
    private void DoAttack()
    {
        Enemy.aimParticles.enabled = false;
        Enemy.myAttack.Shoot();
        isDone = true;
    }
    public bool IsDone
    {
        get
        {
            return isDone;
        }
    }
}
