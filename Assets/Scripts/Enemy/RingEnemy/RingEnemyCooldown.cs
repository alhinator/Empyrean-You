using UnityHFSM;

public class RingEnemyCooldown : EnemyState<RingEnemy, RingEnemyCombatState, RingEnemyEvent> {
    public RingEnemyCooldown(RingEnemy enemy) : base(enemy) { }
}
