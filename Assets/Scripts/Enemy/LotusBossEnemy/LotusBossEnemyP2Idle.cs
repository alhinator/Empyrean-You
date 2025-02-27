using UnityEngine;

public class LotusBossEnemyP2Idle : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemyP2Idle(LotusBossEnemy enemy) : base(enemy) { }
    
    const float TimeBetweenProjectiles = 4f;
    
    private float IdleRotationSpeed = 40;
    private bool reachedIdentity = false;
    private int projectileRotation = 0;
    private float timeSinceProjectile = 0f;

    private LotusBullet waitingOn = null;
    
    public override void OnEnter()
    {
        base.OnEnter();
        reachedIdentity = false;
        projectileRotation = 0;
        this.waitingOn = null;
    }

    public override void OnLogic() {
        base.OnLogic();

        if (waitingOn is { } queued) {
            if (!queued.IsInFlight()) {
                queued.Activate();
                this.waitingOn = null;
            }
        }
        else {
            this.timeSinceProjectile += Time.fixedDeltaTime;
            if (this.timeSinceProjectile >= TimeBetweenProjectiles) {
                this.timeSinceProjectile -= TimeBetweenProjectiles;

                LotusBullet toActivate = this.Enemy.PetalBullets[this.projectileRotation];
                this.projectileRotation += 1;
                this.projectileRotation %= this.Enemy.PetalBullets.Length;

                if (toActivate.IsInFlight()) this.waitingOn = toActivate;
                else toActivate.Activate();
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), IdleRotationSpeed/3 * Time.deltaTime);
            petal.transform.localPosition = idealPosition;
            if (!reachedIdentity)
            {
                Quaternion idealRotation = Quaternion.RotateTowards(petal.transform.localRotation, Quaternion.identity, IdleRotationSpeed * Time.deltaTime);
                petal.transform.localRotation = idealRotation;
                if (petal.transform.localRotation != Quaternion.identity) { allReached = false; }
            }
            else
            {
                petal.transform.Rotate(0.01f * Mathf.Sin(Time.time), IdleRotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(Time.time));
            }
        }
        reachedIdentity = allReached;
    }
}