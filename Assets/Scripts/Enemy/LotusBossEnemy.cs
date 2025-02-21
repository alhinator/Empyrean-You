using System;
using UnityHFSM;

public class LotusBossEnemy : CombatEntity {

    private StateMachine<LotusBossEnemyState, LotusBossEnemyEvent> _stateMachine;
    
    private void Awake() {
        this.InitStateMachineStates();
    }

    private void FixedUpdate() {
        this._stateMachine.OnLogic();
    }

    private void Update() {
        StateBase<LotusBossEnemyState> active = this._stateMachine.ActiveState;
        if (active is EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> lotusBossEnemyState)
        {
            lotusBossEnemyState.OnUpdate();
        }
    }
    
    private void InitStateMachineStates() {
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
        this._stateMachine.Init();
    }
}
