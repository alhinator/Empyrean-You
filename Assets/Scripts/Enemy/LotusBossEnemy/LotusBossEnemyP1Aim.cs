using UnityEngine;
public class LotusBossEnemyP1Aim : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent>
{
    public LotusBossEnemyP1Aim(LotusBossEnemy enemy) : base(enemy) { }

    private float AggroRotationSpeed = 150;
    private float LerpSpeed = 5;
    private float BeamLerpSpeed = 10;

    private float TimeInAim, TimeAiming, last;
    private float TimeToStartCharging = 1;
    private float TimeToLock = 5f;

    private float BobbingAccumulator = 0.0f;

    private bool hiBeams;

    private Vector3 delayedAimPos;

    public override void OnEnter()
    {
        base.OnEnter();
        //Debug.Log("In Phase 1 aim");

        this.BobbingAccumulator = 0.0f;
        TimeInAim = 0;
        TimeAiming = 0;
        last = 0;
        hiBeams = false;
        delayedAimPos = Enemy.lastSeenPosition;
    }
    public override void OnExit()
    {
        base.OnExit();
        Enemy.CentralCharger.Stop();
        Enemy.actualAttackPos = DelayedAimPosition;
        foreach (Petal p in Enemy.PetalScripts)
        {
            p.aimParticles.enabled = false;
        }
    }



    public override void OnUpdate()
    {
        base.OnUpdate();

        this.BobbingAccumulator += Time.deltaTime;
        this.BobbingAccumulator %= 2 * Mathf.PI;

        DoPetalPositionRotation();

        TimeInAim += Time.deltaTime;
        TimeAiming += Time.deltaTime;

        if (TimeInAim > TimeToStartCharging)
        {
            DoPetalChargeEffects();
            if (!Enemy.CentralCharger.isPlaying) { Enemy.CentralCharger.Play(); }
        }
    }
    private void DoPetalChargeEffects()
    {
        if (!Enemy.isInLoS) { TimeAiming = 0; last = 0; return; }//Don't do beam logic + reset timer if we can't see player
        Enemy.actualAttackPos = Enemy.lastSeenPosition;
        //Debug.Log("In DoPetalEffects");

        delayedAimPos = Vector3.MoveTowards(delayedAimPos, Enemy.lastSeenPosition, BeamLerpSpeed * Time.deltaTime);

        float duration = 0.12f;
        bool flashAll = TimeAiming > last + duration;
        if (flashAll) { last = TimeAiming; }
        foreach (Petal p in Enemy.PetalScripts)
        {
            //Debug.Log(p.gameObject.name);
            p.aimParticles.SetPosition(0, p.ChargePoint.position);


            p.aimParticles.SetPosition(1, delayedAimPos);


            //start flashing the beam now. 

            if (flashAll)
            {
                hiBeams = !hiBeams;
                float col = hiBeams ? 0.5f : 0.3f;
                p.aimParticles.startColor = new Color(col, col, 0, 0.25f);
                p.aimParticles.endColor = new Color(col, col, 0, 0.25f);
                p.aimParticles.enabled = !p.aimParticles.enabled;
            }
            if (TimeAiming > TimeToLock - 0.5f)
            {
                float col = 1;
                p.aimParticles.startColor = new Color(col, col, 0, 0.25f);
                p.aimParticles.endColor = new Color(col, col, 0, 0.25f);
                p.aimParticles.enabled = true;
            }
        }

    }
    private void DoPetalPositionRotation()
    {
        //petals 0, 2, 4 stay orbiting, petals 1, 3, 5 float above to aim
        int i = 0;
        foreach (GameObject petal in Enemy.Petals)
        {
            if (i % 2 == 0)
            { // 0, 2, 4
                petal.transform.Rotate(0.01f * Mathf.Sin(this.BobbingAccumulator), AggroRotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(this.BobbingAccumulator));

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

    public bool IsDone
    {
        get
        {
            return TimeAiming > TimeToLock;
        }
    }
    public Vector3 DelayedAimPosition
    {
        get
        {
            return delayedAimPos;
        }
    }
}
