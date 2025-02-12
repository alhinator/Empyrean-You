using UnityHFSM;

public class RingEnemyLockon : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent> {
    public RingEnemyLockon(RingEnemy enemy) : base(enemy) { }
}
