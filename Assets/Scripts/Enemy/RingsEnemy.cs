using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RingsEnemy : Stateful<RingsEnemy, RingsEnemy.State> {
    private float[] largePrimes = {
        101f, 103f, 107f, 109f, 113f, 127f,
        131f, 137f, 139f, 149f, 151f, 157f,
        163f, 167f, 173f, 179f, 181f, 191f
    };
    
    private float outerScale = 9.0f;
    private SO3Path outerOrientation;
    
    private float middleScale = 6.0f;
    private SO3Path middleOrientation;

    private float innerScale = 3.0f;
    private SO3Path innerOrientation;
    
    [SerializeField]
    private Transform outerTransform;
    [SerializeField]
    private Transform middleTransform;
    [SerializeField]
    private Transform innerTransform;

    private float timeBetweenWalkSec = 10.0f;
    private float timeSinceLastWalkSec = 0.0f;

    private GameObject player;
    
    public override State InitialState() {
        return State.Idle.Instance.Value;
    }
    
    // Start is called before the first frame update
    new void Start() {
        base.Start();
        
        this.player = GameObject.FindGameObjectWithTag("Player");

        this.outerOrientation = new SO3Path(this.outerScale,
            this.largePrimes[0], this.largePrimes[1], this.largePrimes[2],
            this.largePrimes[3], this.largePrimes[4], this.largePrimes[5],
            0f, 1f, 2f, 3f, 4f, 5f);
        this.middleOrientation = new SO3Path(this.middleScale,
            this.largePrimes[6], this.largePrimes[7], this.largePrimes[8],
            this.largePrimes[9], this.largePrimes[10], this.largePrimes[11],
            6f, 7f, 8f, 9f, 10f, 11f);
        this.innerOrientation = new SO3Path(this.innerScale,
            this.largePrimes[12], this.largePrimes[13], this.largePrimes[14],
            this.largePrimes[15], this.largePrimes[16], this.largePrimes[17],
            12f, 13f, 14f, 15f, 16f, 17f);
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
    }
    
    public abstract class State : StateTag<RingsEnemy, State> {
        private State() {}

        public sealed class Idle : State {
            public static Lazy<Idle> Instance = new(() => new Idle());
        
            private Idle() { }
        
            public override State Update(RingsEnemy self, float deltaTime) {
                self.outerOrientation.Advance(deltaTime);
                self.middleOrientation.Advance(deltaTime);
                self.innerOrientation.Advance(deltaTime);
        
                self.outerTransform.localRotation = self.outerOrientation.Sample();
                self.middleTransform.localRotation = self.outerTransform.localRotation * self.middleOrientation.Sample();
                self.innerTransform.localRotation = self.middleTransform.localRotation * self.innerOrientation.Sample();
        
                // TODO something better for deciding a random walk
                // TODO figure out how to make "random events" a part of GOAP
                self.timeSinceLastWalkSec += deltaTime;
                if (self.timeSinceLastWalkSec > self.timeBetweenWalkSec) {
                    self.timeSinceLastWalkSec -= self.timeBetweenWalkSec;
                }

                return null;
            }
        }
    }
}
