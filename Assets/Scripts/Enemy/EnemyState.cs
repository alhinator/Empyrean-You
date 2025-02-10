using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class EnemyState<TEnemy, TState, TEvent> : State<TState, TEvent> where TEnemy : MonoBehaviour {
    protected readonly TEnemy Enemy;
    protected readonly NavMeshAgent Agent;
    protected readonly Animator Animator;

    public EnemyState(TEnemy enemy) {
        this.Enemy = enemy;
        this.Agent = this.Enemy.GetComponent<NavMeshAgent>();
        this.Animator = this.Enemy.GetComponent<Animator>();
    }

    public override void OnEnter() {
        base.OnEnter();
    }

    public override void OnLogic() {
        base.OnLogic();
    }

    public override void OnExit() {
        base.OnExit();
    }
}
