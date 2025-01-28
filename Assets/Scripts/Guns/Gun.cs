
using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [Header("Objects")]
    /// <summary>
    /// The origin location of the raycast, or projectile spawn
    /// </summary>
    public Transform bulletOrigin;
    /// <summary>
    /// The point about which to rotate the gun when it's aiming at targets (STRETCH)
    /// </summary>
    public Transform pivotPoint;
    /// <summary>
    /// The primary particle system to play when firing this weapon.
    /// </summary>
    public ParticleSystem primaryParticles;


    [Header("Gun Attributes")]
    /// <summary>
    /// The value equal to how many times per second this gun fires.
    /// </summary>
    public float RateOfFire;


    /// <summary>
    /// How long it takes for the gun to "wind up" before firing"
    /// </summary>
    public float chargeTime;

    /// <summary>
    /// Damage dealt on hit
    /// </summary>
    public float bulletDamage;
    /// <summary>
    /// Number of degrees to add to the gun's bullet spread per shot.
    /// </summary>
    public float perShotRecoil;
    /// <summary>
    /// Does this weapon pierce armor?
    /// </summary>
    public bool APR;
    protected float currRecoil;

    protected float fireTimer;
    protected bool firing = false;

    [Header("Ammunition")]

    /// <summary>
    /// Indicates whether or not this weapon uses ammunition.
    /// </summary>
    public bool UsesAmmunition;
    /// <summary>
    /// Indicates whether or not this weapon has a maximum pool of ammunition to draw from.
    /// </summary>
    public bool UsesMaxAmmunition;
    /// <summary>
    /// The total number of rounds that can be fired before this gun no longer fires.
    /// </summary>
    public int maximumAmmo;
    /// <summary>
    /// If ammunition is used, how many shots may be fired before reloading.
    /// </summary>
    public int MagazineSize;
    /// <summary>
    /// If ammunition is used, how much is remaining in the gun before needing to reload.
    /// </summary>
    protected int currAmmo;

    public abstract void TriggerDown();
    public abstract void TriggerUp();

    protected abstract void Shoot();

    protected Vector3 PickFiringDirection(Vector3 aimDirection, float spreadRadius)
    {
        //this code taken from https://gamedev.stackexchange.com/questions/169893/how-do-i-implement-bullet-spread-in-three-dimensional-space
        Vector3 candidate = UnityEngine.Random.insideUnitSphere * spreadRadius + aimDirection;
        return candidate.normalized;
    }
    public int CurrentAmmo
    {
        get
        {
            return currAmmo;
        }
    }
}
