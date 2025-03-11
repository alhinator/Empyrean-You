using UnityEngine.Events;

public class EnemiesKilledCondition : EnemyObserver {
    public int killsRequired;
    public UnityEvent onKillsReached;
    private bool invoked = false;

    private int totalKillCount = 0;
    
    public override void OnEnemyCreated(Enemy enemy) {
        // do nothing
    }
    public override void OnEnemyKilled(Enemy enemy) {
        totalKillCount++;
        if (totalKillCount >= killsRequired && !invoked) {
            onKillsReached.Invoke();
            invoked = true;
        }
    }
}
