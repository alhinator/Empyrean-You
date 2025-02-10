using UnityHFSM;

public class RingEnemyLockon : EnemyState<RingEnemy, RingEnemyCombatState, RingEnemyEvent> {
    public RingEnemyLockon(RingEnemy enemy) : base(enemy) { }
}
