using UnityHFSM;

public class RingEnemyWander : EnemyState<RingEnemy, RingEnemyIdleState, RingEnemyEvent> {
    public RingEnemyWander(RingEnemy enemy) : base(enemy) { }
}
