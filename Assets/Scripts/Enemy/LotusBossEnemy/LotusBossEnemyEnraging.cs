using UnityEngine;

public class LotusBossEnemyEnraging : EnemyState<LotusBossEnemy, LotusBossEnemyState, LotusBossEnemyEvent> {
    public LotusBossEnemyEnraging(LotusBossEnemy enemy) : base(enemy) { }

    private const float EnrageTime = 3.5f;
    private const float AttackDelay = 2f;

    private const float BaseRotationSpeed = 720f;
    private const float MaxRotationSpeed = BaseRotationSpeed * 3;
    private const float RampUpTime = 1.5f;
    private const float RampDownTime = 1.0f;
    private const float MovementSpeed = 100f;
    
    private float timeElapsed = 0f;
    private float visualTimeElapsed = 0f;
    private float bobbingAccumulator = 0f;
    private bool triggeredAttack = false;
    private bool reachedIdentity = false;
    
    private static float CurrentSpeed(float t) {
        if (t < RampUpTime) return Mathf.Lerp(BaseRotationSpeed, MaxRotationSpeed, t / RampUpTime);
        else if (t <= EnrageTime - RampDownTime) return MaxRotationSpeed;
        else return Mathf.Lerp(BaseRotationSpeed, MaxRotationSpeed, (EnrageTime - t) / RampDownTime);
    }
    
    public override void OnEnter() {
        base.OnEnter();

        this.timeElapsed = 0f;
        this.visualTimeElapsed = 0f;
        this.bobbingAccumulator = 0f;
        this.triggeredAttack = false;
        this.reachedIdentity = false;
    }

    public override void OnLogic() {
        base.OnLogic();

        this.timeElapsed += Time.fixedDeltaTime;
        if (this.timeElapsed >= AttackDelay) {
            // TODO actually do an attack
        }
    }

    public override void OnUpdate() {
        base.OnUpdate();

        this.visualTimeElapsed += Time.deltaTime;
        this.bobbingAccumulator += Time.deltaTime;
        this.bobbingAccumulator %= Mathf.PI * 2f;
        
        float currentRotationSpeed = CurrentSpeed(this.visualTimeElapsed);
        
        bool allReached = true;

        foreach (GameObject petal in Enemy.Petals)
        {
            Vector3 idealPosition = Vector3.MoveTowards(petal.transform.localPosition, new Vector3(0, 1, 0), MaxRotationSpeed / 4 * Time.deltaTime);
            petal.transform.localPosition = idealPosition;
            if (!reachedIdentity)
            {
                Quaternion idealRotation = Quaternion.RotateTowards(petal.transform.localRotation, Quaternion.identity, currentRotationSpeed * Time.deltaTime);
                petal.transform.localRotation = idealRotation;
                if (petal.transform.localRotation != Quaternion.identity) { allReached = false; }
            }
            else
            {
                petal.transform.Rotate(0.01f * Mathf.Sin(this.bobbingAccumulator), currentRotationSpeed * Time.deltaTime, 0.01f * Mathf.Sin(this.bobbingAccumulator));
            }
        }
        reachedIdentity = allReached;
    }

    public bool IsDone {
        get {
            return timeElapsed >= EnrageTime;
        }
    }
}
