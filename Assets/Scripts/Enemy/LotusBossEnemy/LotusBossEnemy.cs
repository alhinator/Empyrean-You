using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityHFSM;

public class LotusBossEnemy : Enemy
{

    private StateMachine<LotusBossEnemyState, LotusBossEnemyEvent> _stateMachine;

    public GameObject _player;

    [Header("Animation")]
    public Animator animator;
    [SerializeField] public GameObject[] Petals;
    public Petal[] PetalScripts;
    public ParticleSystem CentralCharger;
    public ParticleSystem FireParticlesOuter;
    public ParticleSystem FireParticlesInner;

    [Header("Vision")]
    public bool isInLoS = false;
    public Vector3 lastSeenPosition;
    public Vector3 actualAttackPos;
    [SerializeField] public LayerMask visionLayerMask;

    [Header("Attack Parts")]
    public float AttackRange = 100f;
    // TODO replace me with a collider!
    public float EnrageAttackRange = 100f;
    public LotusBullet PetalBulletPrefab;
    public readonly LotusBullet[] PetalBullets = new LotusBullet[3];
    public GameObject DangerZonePrefab;

    [Header("Behavior")]
    public float p1DefenseThreshold;
    public float p2AttackThreshold;
    public float p2DefenseThreshold;

    private void Awake()
    {
        this.InitStateMachineStates();
    }
    public override void Start()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");

        this.animator = GetComponent<Animator>();

        PetalScripts = new Petal[3];
        int i = 0;
        foreach (var petal in Petals)
        {
            if (petal.TryGetComponent(out Petal p))
            {
                //Debug.Log("found a scr");
                PetalScripts[i] = p;
                PetalBullets[i] = LotusBullet.CreateAsChildOf(PetalBulletPrefab, this, p);
                i++;
                p.aimParticles.useWorldSpace = true;
            }
        }
    }

    public override bool OnDeath(DamageInstance d)
    {
        Destroy(this.gameObject);
        return base.OnDeath(d);
    }

    private void FixedUpdate()
    {
        this.CheckPlayerLoS();
        this._stateMachine.OnLogic();
    }

    private void CheckPlayerLoS()
    {
        Physics.Raycast(new Ray(this.transform.position, this._player.transform.position - this.transform.position), out RaycastHit hit, Vector3.Distance(this._player.transform.position, this.transform.position), this.visionLayerMask);
        if (hit.collider && hit.collider.CompareTag("Player"))
        {
            isInLoS = true;
            lastSeenPosition = _player.transform.position;
        }
        else
        {
            isInLoS = false;
        }
    }

    private void Update()
    {
        StateBase<LotusBossEnemyState> active = this._stateMachine.ActiveState;
        if (active is EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> lotusBossEnemyState)
        {
            lotusBossEnemyState.OnUpdate();
        }
        GameObject.FindGameObjectWithTag("BossDebug").GetComponent<TMP_Text>().text = "current state: " + _stateMachine.ActiveStateName; 
    }

    private void InitStateMachineStates()
    {
        this._stateMachine = new StateMachine<LotusBossEnemyState, LotusBossEnemyEvent>();

        this._stateMachine.AddState(LotusBossEnemyState.Spawning, new LotusBossEnemySpawning(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase1Idle, new LotusBossEnemyP1Idle(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase1Aim, new LotusBossEnemyP1Aim(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase1Attack, new LotusBossEnemyP1Attack(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase1Defend, new LotusBossEnemyP1Defend(this));
        this._stateMachine.AddState(LotusBossEnemyState.Enraging, new LotusBossEnemyEnraging(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase2Idle, new LotusBossEnemyP2Idle(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase2Attack, new LotusBossEnemyP2Attack(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase2Defend, new LotusBossEnemyP2Defend(this));
        this._stateMachine.AddState(LotusBossEnemyState.Dying, new LotusBossEnemyDying(this));

        this._stateMachine.SetStartState(LotusBossEnemyState.Spawning);

        // enter phase 1 when done spawning
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Spawning, LotusBossEnemyState.Phase1Idle, (self) => { return (this._stateMachine.ActiveState as LotusBossEnemySpawning).IsDone; }));

        // phase 1 attack loop
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Phase1Aim, self => { return this.CurrentHP < this.MaximumHP; }));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Aim, LotusBossEnemyState.Phase1Attack, ShouldFinishAim));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Attack, LotusBossEnemyState.Phase1Idle, self => { return (this._stateMachine.ActiveState as LotusBossEnemyP1Attack).IsDone; }));

        // phase 1 defend loop
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Phase1Defend, ShouldDefendP1));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Aim, LotusBossEnemyState.Phase1Defend, ShouldDefendP1));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Defend, LotusBossEnemyState.Phase1Idle, self => !ShouldDefendP1(self)));

        // enter phase 2
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Enraging, ShouldEnrage));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Aim, LotusBossEnemyState.Enraging, ShouldEnrage));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Defend, LotusBossEnemyState.Enraging, ShouldEnrage));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Enraging, LotusBossEnemyState.Phase2Idle, self => { return (this._stateMachine.ActiveState as LotusBossEnemyEnraging).IsDone; }));

        // phase 2 attack
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase2Idle, LotusBossEnemyState.Phase2Attack, ShouldAttackP2));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase2Attack, LotusBossEnemyState.Phase2Idle, ShouldIdleP2));

        // phase 2 defend
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase2Idle, LotusBossEnemyState.Phase2Defend, ShouldDefendP2));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase2Defend, LotusBossEnemyState.Phase2Idle, ShouldIdleP2));

        this._stateMachine.Init();
    }

    private bool ShouldDefendP1(Transition<LotusBossEnemyState> self)
    {
        return Vector3.Distance(transform.position, _player.transform.position) <= p1DefenseThreshold;
    }
    private bool ShouldFinishAim(Transition<LotusBossEnemyState> self)
    {
        return (this._stateMachine.ActiveState as LotusBossEnemyP1Aim).IsDone;
    }
    private bool ShouldEnrage(Transition<LotusBossEnemyState> self)
    {
        return this.CurrentHP <= (this.MaximumHP / 2.0f);
    }
    private bool ShouldAttackP2(Transition<LotusBossEnemyState> self)
    {
        return (this._stateMachine.ActiveState as LotusBossEnemyP2Idle).DoneFiringBullets || this._stateMachine.ActiveStateName != LotusBossEnemyState.Phase2Idle && Vector3.Distance(transform.position, _player.transform.position) > p2AttackThreshold;
    }
    private bool ShouldIdleP2(Transition<LotusBossEnemyState> self)
    {
        return (this._stateMachine.ActiveState as LotusBossEnemyP2Attack).IsDone || this._stateMachine.ActiveStateName != LotusBossEnemyState.Phase2Attack && Vector3.Distance(transform.position, _player.transform.position) > p2DefenseThreshold;
    }
    private bool ShouldDefendP2(Transition<LotusBossEnemyState> self)
    {
        return Vector3.Distance(transform.position, _player.transform.position) <= p2DefenseThreshold;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 direction = (actualAttackPos - CentralCharger.transform.position).normalized;
        for(int i = 0; i < Vector3.Distance(actualAttackPos ,CentralCharger.transform.position); i++)
        {
            Gizmos.DrawWireSphere(CentralCharger.transform.position + direction * i, 0.5f);

        }
    }
}
