using UnityEngine;

public class LotusBossEnemyP2Attack : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemyP2Attack(LotusBossEnemy enemy) : base(enemy) { }
    
    private float rotationSpeed = 10;
    private bool reachedIdentity = false;

    public override void OnLogic() {
        base.OnLogic();
        
        // TODO spawn attack under player
    }

    public override void OnUpdate() {
        base.OnUpdate();
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), rotationSpeed/3 * Time.deltaTime);
            petal.transform.localPosition = idealPosition;
            if (!reachedIdentity)
            {
                Quaternion idealRotation = Quaternion.RotateTowards(petal.transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
                petal.transform.localRotation = idealRotation;
                if (petal.transform.localRotation != Quaternion.identity) { allReached = false; }
            }
            else
            {
                petal.transform.Rotate(0.01f * Mathf.Sin(Time.time), rotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(Time.time));
            }
        }
        reachedIdentity = allReached;
    }
}
