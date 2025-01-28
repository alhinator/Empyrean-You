using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    /// <summary>
    /// The value equal to how many times per second this gun fires.
    /// </summary>
    public float RateOfFire;
    /// <summary>
    /// The transform at which to originate raycasts when firing.
    /// </summary>
    public Transform bulletOrigin;
    public float bulletDamage;
    public float perShotRecoil;
    public Transform pivotPoint;
    private float fireTimer;

    public ParticleSystem primaryParticles;
    void Update(){
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.cyan, 0.01f);
    }
    public void Shoot()
    {
    }

    Vector3 PickFiringDirection(Vector3 aimDirection, float spreadRadius)
    {
        //this code taken from https://gamedev.stackexchange.com/questions/169893/how-do-i-implement-bullet-spread-in-three-dimensional-space
        Vector3 candidate = UnityEngine.Random.insideUnitSphere * spreadRadius + aimDirection;
        return candidate.normalized;
    }
}
