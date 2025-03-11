using System;

public class Enemy : CombatEntity {
    protected EnemyObserver[] _observers;
    public EnemyObserver[] Observers => _observers;

    public virtual void Start() {
        this._observers = FindObjectsOfType<EnemyObserver>();

        foreach (var enemyObserver in this._observers) {
            enemyObserver.OnEnemyCreated(this);
        }
    }

    // OnDeath observations are handled in DamageInstance
}
