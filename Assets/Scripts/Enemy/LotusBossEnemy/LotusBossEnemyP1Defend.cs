using Unity.Mathematics;
using UnityEngine;
public class LotusBossEnemyP1Defend : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    public LotusBossEnemyP1Defend(LotusBossEnemy enemy) : base(enemy) { }

    private float DefendRotationSpeed = 360;
    private bool reachedIdentity = false;
    public override void OnEnter()
    {
        base.OnEnter();
        reachedIdentity = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), DefendRotationSpeed/4 * Time.deltaTime);
            petal.transform.localPosition = idealPosition;
            if (!reachedIdentity)
            {
                Quaternion idealRotation = Quaternion.RotateTowards(petal.transform.localRotation, Quaternion.identity, DefendRotationSpeed * Time.deltaTime);
                petal.transform.localRotation = idealRotation;
                if (petal.transform.localRotation != Quaternion.identity) { allReached = false; }
            }
            else
            {
                petal.transform.Rotate(0.01f * Mathf.Sin(Time.time), DefendRotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(Time.time));
            }
        }
        reachedIdentity = allReached;
        if (allReached == true) { Debug.Log("ready to keep spinning"); }
    }
}

