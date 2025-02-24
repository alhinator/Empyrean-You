public class LotusBossEnemySpawning : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemySpawning(LotusBossEnemy enemy) : base(enemy) { }

    private bool isReady = false;
    public override void OnEnter()
    {
        isReady = false;
        base.OnEnter();
        
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if(Enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("SpawnDone")){
            isReady = true;
            Enemy.animator.StopPlayback();
            Enemy.animator.enabled = false;
        }
    }
    public bool IsDone{
        get{
            return isReady;
        }
    }
}
