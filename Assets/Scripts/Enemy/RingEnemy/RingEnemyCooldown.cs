using UnityHFSM;

public class RingEnemyCooldown : EnemyState<RingEnemy, RingEnemyState, RingEnemyEvent> {
    public RingEnemyCooldown(RingEnemy enemy) : base(enemy) { }
}
