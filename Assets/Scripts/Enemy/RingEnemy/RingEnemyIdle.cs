using UnityEngine;
using UnityHFSM;

public class RingEnemyIdle : EnemyState<RingEnemy, RingEnemyIdleState, RingEnemyEvent> {
    public RingEnemyIdle(RingEnemy enemy) : base(enemy) {
        
    }

    public override void OnLogic() {
        base.OnLogic();
        
        Enemy.OuterOrientation.Advance(Time.deltaTime);
        Enemy.MiddleOrientation.Advance(Time.deltaTime);
        Enemy.InnerOrientation.Advance(Time.deltaTime);
        
        Enemy.outerTransform.localRotation = Enemy.OuterOrientation.Sample();
        Enemy.middleTransform.localRotation = Enemy.outerTransform.localRotation * Enemy.MiddleOrientation.Sample();
        Enemy.innerTransform.localRotation = Enemy.middleTransform.localRotation * Enemy.InnerOrientation.Sample();
    }
}
