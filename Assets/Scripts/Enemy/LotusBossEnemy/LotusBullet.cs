using System;
using System.Collections;
using UnityEngine;

public class LotusBullet : MonoBehaviour
{
    private const float RiseSpeed = 5f;
    private const float ShootSpeed = 25f;

    public ParticleSystem explodeParticles;
    public LotusBulletWeapon myWeapon;

    public LotusBossEnemy owner;
    public Petal associatedPetal;

    private float flightTime;
    private bool lockedIn;
    private Vector3 lockedInPosition;

    public static LotusBullet CreateAsChildOf(LotusBullet prefab, LotusBossEnemy boss, Petal petal)
    {
        LotusBullet next = GameObject.Instantiate(prefab, boss.transform);
        next.gameObject.SetActive(false);
        next.owner = boss;
        next.associatedPetal = petal;
        return next;
    }
    private void Start()
    {
        this.myWeapon.SetOwner(owner);
    }
    private void Update()
    {
        MoveMe();

    }
    private void MoveMe()
    {
        if (this.flightTime < 1f || (!this.lockedIn && !this.owner.isInLoS))
        {
            this.transform.position += Vector3.up * (Time.deltaTime * RiseSpeed);
        }
        else if (this.owner.isInLoS && this.flightTime > 3f && !this.lockedIn )
        {
            this.lockedIn = true;
            this.lockedInPosition = this.owner.lastSeenPosition;
        }
        else if (flightTime > 3f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.lockedInPosition, Time.deltaTime * ShootSpeed);
        }
        this.flightTime += Time.deltaTime;
        if (this.flightTime >= 13f) this.Deactivate();
    }

    public void Activate()
    {
        this.transform.position = this.associatedPetal.ChargePoint.position;
        this.flightTime = 0f;
        this.lockedIn = false;
        this.gameObject.SetActive(true);
    }

    public bool IsInFlight()
    {
        return this.gameObject.activeSelf;
    }
    private IEnumerator DeactivateWithDelay(float delay){
        yield return new WaitForSeconds(delay);
        Deactivate();
    }
    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider col){
            Debug.Log("Lotus bullet hit trigger" + col.name);
            Explode();
        
    }
    private void Explode(){
        explodeParticles.Play();
        myWeapon.Shoot();
        StartCoroutine(DeactivateWithDelay(1f));
    }
}
