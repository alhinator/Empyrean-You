using UnityEngine;

public class LotusBossEnemyP2Idle : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemyP2Idle(LotusBossEnemy enemy) : base(enemy) { }
    
    private float IdleRotationSpeed = 40;
    private bool reachedIdentity = false;
    public override void OnEnter()
    {
        base.OnEnter();
        reachedIdentity = false;
    }

    public override void OnLogic() {
        base.OnLogic();
        
        // TODO decide when & from where to spawn projectiles
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