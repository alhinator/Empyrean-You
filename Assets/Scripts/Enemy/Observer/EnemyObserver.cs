using UnityEngine;

public abstract class EnemyObserver : MonoBehaviour {
    // Add functions as seen fit
    public abstract void OnEnemyCreated(Enemy enemy);
    public abstract void OnEnemyKilled(Enemy enemy);
}
