using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityHFSM;

public class LotusBossEnemy : CombatEntity
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

    [Header("Behavior")]
    public float p1DefenseThreshold;

    private void Awake()
    {
        this.InitStateMachineStates();
    }
    private void Start()
    {
        this._player = GameObject.FindGameObjectWithTag("Player");

        this.animator = GetComponent<Animator>();

        PetalScripts = new Petal[3];
        int i = 0;
        foreach (var petal in Petals)
        {
            if (petal.TryGetComponent(out Petal p))
            {
                Debug.Log("found a scr");
                PetalScripts[i] = p;
                i++;
                p.aimParticles.useWorldSpace = true;
            }
        }
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
        this._stateMachine.AddState(LotusBossEnemyState.Phase2Aim, new LotusBossEnemyP2Aim(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase2Attack, new LotusBossEnemyP2Attack(this));
        this._stateMachine.AddState(LotusBossEnemyState.Phase3Defend, new LotusBossEnemyP3Defend(this));
        this._stateMachine.AddState(LotusBossEnemyState.Dying, new LotusBossEnemyDying(this));

        this._stateMachine.SetStartState(LotusBossEnemyState.Spawning);

        // enter phase 1 when done spawning
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Spawning, LotusBossEnemyState.Phase1Idle, (self) => { return (this._stateMachine.ActiveState as LotusBossEnemySpawning).IsDone; }));

        // phase 1 attack loop
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Phase1Aim, self => { return this.CurrentHP < this.MaximumHP; }));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Aim, LotusBossEnemyState.Phase1Attack, self => false));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Attack, LotusBossEnemyState.Phase1Idle, self => false));

        // phase 1 defend loop
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Phase1Defend, shouldDefend));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Aim, LotusBossEnemyState.Phase1Defend, shouldDefend));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Defend, LotusBossEnemyState.Phase1Idle, self => !shouldDefend(self)));

        // defend if player is too near
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Attack, LotusBossEnemyState.Phase1Defend, shouldDefend));

        // enter phase 2
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Phase1Idle, LotusBossEnemyState.Enraging, self => false));
        this._stateMachine.AddTransition(new Transition<LotusBossEnemyState>(LotusBossEnemyState.Enraging, LotusBossEnemyState.Phase2Aim, self => false));

        this._stateMachine.Init();
    }

    private bool shouldDefend(Transition<LotusBossEnemyState> self) {
        return Vector3.Distance(transform.position, _player.transform.position) <= p1DefenseThreshold;
    }
}
