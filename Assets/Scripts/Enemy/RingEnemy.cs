using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class RingEnemy : CombatEntity
{
    private readonly float[] _largePrimes = {
        101f, 103f, 107f, 109f, 113f, 127f,
        131f, 137f, 139f, 149f, 151f, 157f,
        163f, 167f, 173f, 179f, 181f, 191f
    };

    private const float OuterScale = 9.0f;
    private SO3Path _outerOrientation;

    private const float MiddleScale = 6.0f;
    private SO3Path _middleOrientation;

    private const float InnerScale = 3.0f;
    private SO3Path _innerOrientation;
    
    [SerializeField]
    private Transform outerTransform;
    [SerializeField]
    private Transform middleTransform;
    [SerializeField]
    private Transform innerTransform;
    
    [SerializeField]
    private LayerMask raycastMask;

    private const float WalkDistance = 10.0f;
    private const float TimeBetweenWalkSec = 10.0f;
    private float _timeSinceLastWalkSec = 0.0f;

    private GameObject _player;
    
    void Start()
    {
        maxHP = 200;
        currHP = MaximumHP;
        this.Abilities = new Ability[0];
        
        this._player = GameObject.FindGameObjectWithTag("Player");

        this._outerOrientation = new SO3Path(OuterScale,
            this._largePrimes[0], this._largePrimes[1], this._largePrimes[2],
            this._largePrimes[3], this._largePrimes[4], this._largePrimes[5],
            0f, 1f, 2f, 3f, 4f, 5f);
        this._middleOrientation = new SO3Path(MiddleScale,
            this._largePrimes[6], this._largePrimes[7], this._largePrimes[8],
            this._largePrimes[9], this._largePrimes[10], this._largePrimes[11],
            6f, 7f, 8f, 9f, 10f, 11f);
        this._innerOrientation = new SO3Path(InnerScale,
            this._largePrimes[12], this._largePrimes[13], this._largePrimes[14],
            this._largePrimes[15], this._largePrimes[16], this._largePrimes[17],
            12f, 13f, 14f, 15f, 16f, 17f);
    }

    void Update()
    {
        this._outerOrientation.Advance(Time.deltaTime);
        this._middleOrientation.Advance(Time.deltaTime);
        this._innerOrientation.Advance(Time.deltaTime);
        
        this.outerTransform.localRotation = this._outerOrientation.Sample();
        this.middleTransform.localRotation = this.outerTransform.localRotation * this._middleOrientation.Sample();
        this.innerTransform.localRotation = this.middleTransform.localRotation * this._innerOrientation.Sample();
        
        // TODO something better for deciding a random walk
        this._timeSinceLastWalkSec += Time.deltaTime;
        if (this._timeSinceLastWalkSec > TimeBetweenWalkSec) {
            this._timeSinceLastWalkSec -= TimeBetweenWalkSec;

            var tryDirection = Random.onUnitSphere;
            // TODO this should be a sphere cast
            if (!Physics.Raycast(this.transform.position, tryDirection, WalkDistance, this.raycastMask)) {
                // TODO make this into a fluid movement coroutine
                this.transform.position += tryDirection * WalkDistance;
            }
        }
    }

    public override bool HitDetected(DamageInstance d)
    {
        return true; //We have no need to avoid being hit w/ ring enemy logic
    }
    public override bool OnDamage(DamageInstance d)
    {
        //ring enemy has no sort of damage reductions.
        //Debug.Log("took " + d.AdjustedDamage);
        currHP -= d.AdjustedDamage;
        if (CurrentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool OnDeath(DamageInstance d)
    {
        Debug.Log("my name is " + transform.name + " and i just died");
        Destroy(this.gameObject);
        return true;
    }

}