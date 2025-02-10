using UnityHFSM;

public class RingEnemyWarmup : EnemyState<RingEnemy, RingEnemyCombatState, RingEnemyEvent> {
    public RingEnemyWarmup(RingEnemy enemy) : base(enemy) { }
}
