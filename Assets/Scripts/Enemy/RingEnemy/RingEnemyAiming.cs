using UnityHFSM;

public class RingEnemyAiming : EnemyState<RingEnemy, RingEnemyCombatState, RingEnemyEvent> {
    public RingEnemyAiming(RingEnemy enemy) : base(enemy) { }
}
