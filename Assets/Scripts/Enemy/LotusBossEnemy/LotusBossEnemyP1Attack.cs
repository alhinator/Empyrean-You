using UnityEngine;

public class LotusBossEnemyP1Attack : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemyP1Attack(LotusBossEnemy enemy) : base(enemy) { }

    const float attackSize = 1.0f;

    private Vector3 targetPosition;
    const float lerpSpeed = 0.033f;

    public override void OnEnter() {
        base.OnEnter();

        this.targetPosition = this.Enemy.lastSeenPosition;
    }

    public override void OnLogic() {
        base.OnLogic();

        this.targetPosition = Vector3.Lerp(this.targetPosition, this.Enemy._player.transform.position, lerpSpeed);
        if(this.PlayerInSphereCast(out CombatEntity player)) {
            // TODO damage with weapon
        }
    }

    public bool PlayerInSphereCast(out CombatEntity combatEntity) {
        if(Physics.SphereCast(
            new Ray(this.Enemy.transform.position, this.targetPosition - this.Enemy.transform.position),
            attackSize,
            out RaycastHit hit,
            Vector3.Distance(this.targetPosition, this.Enemy.transform.position),
            this.Enemy.visionLayerMask
        )) {
            if(hit.transform.gameObject.TryGetComponent<CombatEntity>(out CombatEntity tg)) {
                combatEntity = tg;
                return true;
            }
        }
        combatEntity = null;
        return false;
    }
}
