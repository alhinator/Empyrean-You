using UnityEngine;

public class LotusBossEnemyP1Attack : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    public LotusBossEnemyP1Attack(LotusBossEnemy enemy) : base(enemy) { }

    const float attackSize = 1.0f;
    private const float AttackingRotationSpeed = 5;
    private const float LerpSpeed = 5;
    public float BeamLerpSpeed = 5;

    private float BobbingAccumulator = 0.0f;

    private float TimeShooting;
    private const float MaxShootingTime = 5f;

    private float lastHitTime;
    private const float timeBetweenHits = 0.5f;
    private bool inBeam;

    private Vector3 targetPosition;


    public override void OnEnter()
    {
        base.OnEnter();

        this.targetPosition = this.Enemy.actualAttackPos;
        Enemy.CentralCharger.Play();
        Enemy.FireParticlesOuter.Play();
        Enemy.FireParticlesInner.Play();
        TimeShooting = 0;
    }
    public override void OnExit()
    {
        base.OnExit();

        Enemy.CentralCharger.Stop();
        Enemy.FireParticlesInner.Stop();
        Enemy.FireParticlesOuter.Stop();

    }

    public override void OnLogic()
    {
        base.OnLogic();

        if (this.PlayerInSphereCast(out CombatEntity player))
        {
            // TODO damage with weapon
            if (inBeam == false)
            { //player has just entered beam this frame
              //do damage
                new DamageInstance(Enemy, Enemy.GetComponent<LotusBeamAttack>(), player);
                lastHitTime = 0;
                inBeam = true;
            }
            else
            { //staying in beam for mult frames
                lastHitTime += Time.deltaTime;
                if (lastHitTime > timeBetweenHits)
                {
                    new DamageInstance(Enemy, Enemy.GetComponent<LotusBeamAttack>(), player);
                    lastHitTime = 0;
                    inBeam = true;
                }
            }
        }
        else
        {
            inBeam = false;
            lastHitTime = 0;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        this.BobbingAccumulator += Time.deltaTime;
        this.BobbingAccumulator %= 2 * Mathf.PI;

        DoPetalPositionRotation();
        DoBeamPositionRotation();
        DoAimAdjustment();

        TimeShooting += Time.deltaTime;
    }

    public bool PlayerInSphereCast(out CombatEntity combatEntity)
    {
        if (Physics.SphereCast(
            new Ray(this.Enemy.CentralCharger.transform.position, (this.targetPosition - this.Enemy.CentralCharger.transform.position).normalized * Enemy.AttackRange),
            attackSize,
            out RaycastHit hit,
            Vector3.Distance(this.targetPosition, this.Enemy.CentralCharger.transform.position),
            this.Enemy.visionLayerMask
        ))
        {
            if (hit.transform.gameObject.TryGetComponent<CombatEntity>(out CombatEntity tg))
            {
                combatEntity = tg;
                //Debug.Log("player is standing in beam");
                return true;
            }
        }
        combatEntity = null;
        return false;
    }

    private void DoPetalPositionRotation()
    {
        //petals 0, 2, 4 stay orbiting, petals 1, 3, 5 float above to aim
        int i = 0;
        foreach (GameObject petal in Enemy.Petals)
        {
            if (i % 2 == 0)
            { // 0, 2, 4
                petal.transform.Rotate(0.01f * Mathf.Sin(this.BobbingAccumulator), AttackingRotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(this.BobbingAccumulator));

            }
            else
            { // 1 , 3, 5
                Vector3 idealPosition = Vector3.Lerp(petal.transform.localPosition, new Vector3(0, 8, 0), Time.deltaTime);
                //Quaternion idealRotation = Quaternion.Slerp(petal.transform.localRotation, Quaternion.LookRotation(Enemy._player.transform.position - petal.transform.position, Vector3.right), Time.deltaTime);
                Quaternion idealRotation = Quaternion.FromToRotation(Vector3.up, Enemy._player.transform.position - petal.transform.position);
                idealRotation = Quaternion.Slerp(petal.transform.localRotation, idealRotation, LerpSpeed * Time.deltaTime);
                //Debug.DrawLine(petal.transform.position, petal.transform.forward);
                petal.transform.SetLocalPositionAndRotation(idealPosition, idealRotation);


            }
            i++;
        }
    }

    private void DoBeamPositionRotation()
    {
        Vector3 beamOrigin = Enemy.CentralCharger.transform.position;
        Vector3 beamForward = (targetPosition - Enemy.CentralCharger.transform.position).normalized;
        Debug.DrawRay(beamOrigin, beamForward * Enemy.AttackRange, Color.cyan, 1f);

        //Firstly, we want to draw a raycast *through* the current aimPoint until we hit a scene object or hit our max range.
        //This will determine our max raycast length when checking for hits against enemies.
        Physics.Raycast(beamOrigin, beamForward, out RaycastHit hit, Enemy.AttackRange, LayerMask.GetMask("WalkableTerrain", "CameraObstacle"));
        //now set our end position.
        //Debug.Log(hit.transform);
        Vector3 endposition = hit.transform ? hit.point : beamOrigin + beamForward * Enemy.AttackRange;
        Vector3 startPosition = beamOrigin;
        //Particle system line code modified from https://discussions.unity.com/t/emit-particles-throughout-a-line-ray/227355/3

        Vector3 particlePosition = (endposition - startPosition) / 2 + startPosition; // particle system position is delta middle + start position
        float distance = Vector3.Distance(endposition, startPosition) / 2; //distance is half the total distance since line extends both ways
        int numParticles = (int)(distance * 50);


        //Do this for inner and outer particles
        Enemy.FireParticlesOuter.transform.position = particlePosition; //update the system's position
        Enemy.FireParticlesInner.transform.position = particlePosition; //update the system's position
        Enemy.FireParticlesOuter.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.
        Enemy.FireParticlesInner.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.

        ParticleSystem.ShapeModule sm = Enemy.FireParticlesOuter.shape;
        sm.radius = distance; //adjust the line size
        ParticleSystem.ShapeModule sm2 = Enemy.FireParticlesInner.shape;
        sm2.radius = distance; //adjust the line size

        ParticleSystem.EmissionModule em = Enemy.FireParticlesInner.emission;
        ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
        em.SetBurst(0, b); //set burst to be the number of desired particles.

    }

    private void DoAimAdjustment()
    {
        targetPosition = Vector3.MoveTowards(targetPosition, Enemy.lastSeenPosition, BeamLerpSpeed * Time.deltaTime);

    }

    public bool IsDone
    {
        get
        {
            return TimeShooting > MaxShootingTime;
        }
    }
}
