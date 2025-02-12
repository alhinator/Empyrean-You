using UnityHFSM;

public class RingEnemyAiming : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent> {
    public RingEnemyAiming(RingEnemy enemy) : base(enemy) { }
}
