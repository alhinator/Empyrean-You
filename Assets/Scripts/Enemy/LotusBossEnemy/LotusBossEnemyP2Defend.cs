using System;
using UnityEngine;

public class LotusBossEnemyP2Defend : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    private LotusBossEnemyP2Idle concurrent;
    public LotusBossEnemyP2Defend(LotusBossEnemy enemy) : base(enemy) { concurrent = new(enemy); }

    private float DefendRotationSpeed = 300;
    private bool reachedIdentity = false;
    public override void OnEnter()
    {
        base.OnEnter();
        concurrent.OnEnter();
        reachedIdentity = false;
        SpawnZoneCenteredOnMe();
    }
    public override void OnExit()
    {
        base.OnExit();
        concurrent.OnExit();
    }


    public override void OnLogic()
    {
        base.OnLogic();
        concurrent.OnLogic();
        // TODO decide when to spawn attack
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        concurrent.OnLogic();
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), DefendRotationSpeed / 4 * Time.deltaTime);
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
    }

    private void SpawnZoneCenteredOnMe()
    {

        Vector3 originPos = Enemy.transform.position;
        var newZone = GameObject.Instantiate(Enemy.DangerZonePrefab);
        newZone.transform.position = originPos + Vector3.up * 2;



    }
}
