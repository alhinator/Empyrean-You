public enum LotusBossEnemyState {
    Spawning,
    // TODO look into moving OnUpdate into an interface to have both EnemyState and EnemyStateMachine
    Phase1Idle,
    Phase1Aim,
    Phase1Attack,
    Phase1Defend,
    /// <summary>
    /// Phase 1 to Phase 2 transition state
    /// </summary>
    Enraging,
    Phase2Idle,
    Phase2Attack,
    Phase2Defend,
    Dying
}
