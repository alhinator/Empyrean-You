using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering;

public class LotusBossEnemyP2Attack : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    private LotusBossEnemyP1Attack concurrentAttack;
    private LotusBossEnemyP1Aim concurrentAim;

    public LotusBossEnemyP2Attack(LotusBossEnemy enemy) : base(enemy) { concurrentAim = new(enemy); concurrentAttack = new(enemy); }

    [Header("AoE zone variables")]
    private float timeBetweenZones = 2;
    private float timeBetweenBursts = 5;
    public float zoneTimer;

    private int zonesPerBurst = 3;
    private int currBurstZones;

    private float TimeInP2Attack = 0;
    private float MaxAttackTime = 20;

    private bool amFiring;



    public override void OnEnter()
    {
        base.OnEnter();
        concurrentAim.OnEnter();

        zoneTimer = 0;
        currBurstZones = 0;
        TimeInP2Attack = 0;
        amFiring = false;
    }
    public override void OnLogic()
    {
        base.OnLogic();

        //flag here
        if (concurrentAim.IsDone && amFiring == false)
        {
            amFiring = true;
            concurrentAim.OnExit();
            concurrentAttack.OnEnter();
        }
        else if (concurrentAttack.IsDone && amFiring)
        {
            amFiring = false;
            concurrentAttack.OnExit();
            concurrentAim.OnEnter();
        }

        if (!amFiring)
        {
            concurrentAim.OnLogic();
        }
        else
        {
            concurrentAttack.OnLogic();
        }

    }
    public override void OnExit()
    {
        base.OnExit();
        concurrentAim.OnExit();
        concurrentAttack.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!amFiring)
        {
            concurrentAim.OnUpdate();
        }
        else
        {
            concurrentAttack.OnUpdate();
        }

        DoZoneTimer();

        TimeInP2Attack += Time.deltaTime;

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

            newZone.transform.position = hit.point + Vector3.up * 2;

        }
    }

    public bool IsDone
    {
        get
        {
            return TimeInP2Attack > MaxAttackTime;
        }
    }
}
