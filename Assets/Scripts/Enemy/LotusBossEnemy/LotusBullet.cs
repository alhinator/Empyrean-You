using System;
using UnityEngine;

public class LotusBullet : MonoBehaviour {
    private const float RiseSpeed = 1f;
    private const float ShootSpeed = 1f;
    
    public LotusBossEnemy owner;
    public Petal associatedPetal;

    private float flightTime;
    private bool lockedIn;
    private Vector3 lockedInPosition;
    
    public static LotusBullet CreateAsChildOf(LotusBullet prefab, LotusBossEnemy boss, Petal petal) {
        LotusBullet next = GameObject.Instantiate(prefab, boss.transform);
        next.gameObject.SetActive(false);
        next.owner = boss;
        next.associatedPetal = petal;
        return next;
    }

    private void Update() {
        if (this.flightTime < 1f || (!this.lockedIn && !this.owner.isInLoS)) {
            this.transform.position += Vector3.up * (Time.deltaTime * RiseSpeed);
        }
        else if(this.owner.isInLoS && !this.lockedIn) {
            this.lockedIn = true;
            this.lockedInPosition = this.owner.lastSeenPosition;
        } else {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.lockedInPosition, Time.deltaTime * ShootSpeed);
        }
        this.flightTime += Time.deltaTime;
        if(this.flightTime >= 10f) this.Deactivate();
    }

    public void Activate() {
        this.transform.position = this.associatedPetal.ChargePoint.position;
        this.flightTime = 0f;
        this.lockedIn = false;
        this.gameObject.SetActive(true);
    }

    public bool IsInFlight() {
        return this.gameObject.activeSelf;
    }

    public void Deactivate() {
        this.gameObject.SetActive(false);
    }
}
