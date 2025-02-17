using UnityEngine;
using UnityHFSM;
[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider))]
public class RingEnemy : CombatEntity
{
    [Header("References")]
    public GameObject _player;

    private StateMachine<RingEnemyState, RingEnemyEvent> stateMachine;
    public LineRenderer aimParticles;
    public ParticleSystem fireParticles;
    private readonly float[] _largePrimes = {
        101f, 103f, 107f, 109f, 113f, 127f,
        131f, 137f, 139f, 149f, 151f, 157f,
        163f, 167f, 173f, 179f, 181f, 191f
    };
    public RingEnemyAttack myAttack;

    // TODO should these be get; private set;?

    [Header("Rings")]
    [SerializeField] public Transform outerTransform;
    [SerializeField] public Transform middleTransform;
    [SerializeField] public Transform innerTransform;

    public SO3Path OuterOrientation;
    private const float OuterScale = 9.0f;

    private const float MiddleScale = 6.0f;
    public SO3Path MiddleOrientation;

    private const float InnerScale = 3.0f;
    public SO3Path InnerOrientation;

    [Header("Collisions")]
    public SphereCollider hitbox;
    public Rigidbody rb;

    [Header("Vision and Detection")]
    public float visionRange = 75;
    public bool isInRange = false;
    public bool isInLoS = false;
    public Vector3 lastSeenPosition;
    public Quaternion aimingDirection;
    public Vector3 actualAttackPos;
    public float timeSinceLoS = 0;
    private const float LoSLimit = 1f;
    public float AttackRange = 100;

    //[Header("DebugInfo")]


    private void Awake()
    {
        InitStateMachineStates();
        aimParticles.enabled = false;

        aimingDirection = Quaternion.identity;
    }



    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.hitbox = this.GetComponent<SphereCollider>();

        myAttack = GetComponent<RingEnemyAttack>();
        myAttack.SetOwner(this);
        Weapons = new Weapon[1];
        Weapons[0] = myAttack;
        fireParticles.transform.parent = null;
        aimParticles.transform.parent = null;
        aimParticles.useWorldSpace = true;

        currHP = MaximumHP;

        this._player = GameObject.FindGameObjectWithTag("Player");

        SetIndicesAndRadians();

    }

    void FixedUpdate()
    {
        DeterminePlayerInRange();
        this.stateMachine.OnLogic();
    }

    private void DeterminePlayerInRange()
    {
        if(!_player){return;}
        isInRange = Vector3.Distance(this.transform.position, _player.transform.position) < visionRange;
        if (isInRange)
        {
            //do vision raycast here;
            Physics.Raycast(new Ray(this.transform.position, this._player.transform.position - this.transform.position), out RaycastHit hit, Vector3.Distance(this._player.transform.position, this.transform.position), hitbox.includeLayers);
            if (hit.collider.CompareTag("Player"))
            {
                isInLoS = true;
                timeSinceLoS = 0;
                lastSeenPosition = _player.transform.position;
            }
            else
            {
                isInLoS = false;
                timeSinceLoS += Time.deltaTime;
            }
        }
        else
        {
            isInLoS = false;
            timeSinceLoS += Time.deltaTime;
        }
    }

    void Update()
    {
        StateBase<RingEnemyState> active = this.stateMachine.ActiveState;
        if (active is EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent> ringEnemyState)
        {
            ringEnemyState.OnUpdate();
        }

    }

    public override bool HitDetected(DamageInstance d)
    {
        return true; //We have no need to avoid being hit w/ ring enemy logic
    }
    public override bool OnDamage(DamageInstance d)
    {
        //ring enemy has no sort of damage reductions.
        //Debug.Log("took " + d.AdjustedDamage);
        currHP -= d.AdjustedDamage;
        if (CurrentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool OnDeath(DamageInstance d)
    {
        Debug.Log("my name is " + transform.name + " and i just died");
        Destroy(aimParticles);
        Destroy(fireParticles, fireParticles.main.duration);

        Destroy(this.gameObject);
        return true;
    }
    private void SetIndicesAndRadians()
    {
        // TODO permute our indices & our radians
        this.OuterOrientation = new SO3Path(OuterScale,
            this._largePrimes[0], this._largePrimes[1], this._largePrimes[2],
            this._largePrimes[3], this._largePrimes[4], this._largePrimes[5],
            0f, 1f, 2f, 3f, 4f, 5f);
        this.MiddleOrientation = new SO3Path(MiddleScale,
            this._largePrimes[6], this._largePrimes[7], this._largePrimes[8],
            this._largePrimes[9], this._largePrimes[10], this._largePrimes[11],
            6f, 7f, 8f, 9f, 10f, 11f);
        this.InnerOrientation = new SO3Path(InnerScale,
            this._largePrimes[12], this._largePrimes[13], this._largePrimes[14],
            this._largePrimes[15], this._largePrimes[16], this._largePrimes[17],
            12f, 13f, 14f, 15f, 16f, 17f);
    }
    private void InitStateMachineStates()
    {
        this.stateMachine = new StateMachine<RingEnemyState, RingEnemyEvent>();

        this.stateMachine.AddState(RingEnemyState.Idle, new RingEnemyIdle(this));
        this.stateMachine.AddState(RingEnemyState.Warmup, new RingEnemyWarmup(this));
        this.stateMachine.AddState(RingEnemyState.Aiming, new RingEnemyAiming(this));
        this.stateMachine.AddState(RingEnemyState.Lockon, new RingEnemyLockon(this));
        this.stateMachine.AddState(RingEnemyState.Cooldown, new RingEnemyCooldown(this));

        //Transition from any state (other than idle) to idle when player leaves LoS for a certain duration.
        this.stateMachine.AddTransitionFromAny(new Transition<RingEnemyState>(RingEnemyState.Idle, RingEnemyState.Idle, CheckForLostLoS));

        //Transition from idle to warmup when player is in range & LoS
        this.stateMachine.AddTransition(new Transition<RingEnemyState>(RingEnemyState.Idle, RingEnemyState.Warmup, (self) => { return this.isInLoS && this.isInRange; }));

        //Transition from warmup to aiming when warmup is done
        this.stateMachine.AddTransition(new Transition<RingEnemyState>(RingEnemyState.Warmup, RingEnemyState.Aiming, (self) => { return (this.stateMachine.ActiveState as RingEnemyWarmup).IsDone; }));

        //Transition from aiming to lockon when aim is done
        this.stateMachine.AddTransition(new Transition<RingEnemyState>(RingEnemyState.Aiming, RingEnemyState.Lockon, (self) => { return (this.stateMachine.ActiveState as RingEnemyAiming).IsDone; }));

        //Transition from lockon to cooldown after we've fired;
        this.stateMachine.AddTransition(new Transition<RingEnemyState>(RingEnemyState.Lockon, RingEnemyState.Cooldown, (self) => { return (this.stateMachine.ActiveState as RingEnemyLockon).IsDone; }));

        //Transition from cooldown to Aiming after the recharge period is over.
        this.stateMachine.AddTransition(new Transition<RingEnemyState>(RingEnemyState.Cooldown, RingEnemyState.Aiming, (self) => { return (this.stateMachine.ActiveState as RingEnemyCooldown).IsDone; }));

        // TODO transition from Aiming to Cooldown when lost sight of player
        // TODO should lost sight instead incur a chase mode?

        this.stateMachine.SetStartState(RingEnemyState.Idle);

        this.stateMachine.Init();
    }

    private bool CheckForLostLoS(Transition<RingEnemyState> self)
    {
        if (this.stateMachine.ActiveState.name == RingEnemyState.Idle || this.stateMachine.ActiveState.name == RingEnemyState.Cooldown) { return false; }
        else if (!isInLoS && timeSinceLoS >= LoSLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}