using UnityEngine;
using System.Collections;
using System;

public class LotusBossEnemyP2Attack : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    public LotusBossEnemyP2Attack(LotusBossEnemy enemy) : base(enemy) { }

    private float rotationSpeed = 10;
    private bool reachedIdentity = false;

    [Header("AoE zone variables")]
    private float timeBetweenZones = 2;
    private float timeBetweenBursts = 5;
    public float zoneTimer;

    private int zonesPerBurst = 3;
    private int currBurstZones;




    public override void OnEnter()
    {
        base.OnEnter();
        zoneTimer = 0;
        currBurstZones = 0;

    }
    public override void OnLogic()
    {
        base.OnLogic();

        DoZoneTimer();


    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), rotationSpeed / 3 * Time.deltaTime);
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
    private void DoZoneTimer()
    {
        zoneTimer += Time.deltaTime;
        if (zoneTimer > timeBetweenZones)
        {
            zoneTimer = 0;
            currBurstZones++;
            SpawnZone();
            if (currBurstZones >= zonesPerBurst)
            {
                currBurstZones = 0;
                zoneTimer = -timeBetweenBursts;
            }
        }
    }
    private void SpawnZone()
    {
        //first, get position to spawn at:
        //track player pos, raycast straight down, angle at the normal direction
        Vector3 originPos = Enemy._player.transform.position;
        Physics.Raycast(originPos, Vector3.down, out RaycastHit hit, 100, LayerMask.GetMask("WalkableTerrain", "CameraObstacle"));
        if (hit.point != null && hit.normal != null)
        {
            var newZone = GameObject.Instantiate(Enemy.DangerZonePrefab);

            newZone.transform.position = hit.point + Vector3.up*2;

        }



    }
}
