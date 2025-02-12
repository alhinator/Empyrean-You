using System;
using System.Collections;
using UnityEngine;
public class Guanyin : Gun
{
    [Header("Guanyin Specific vars")]
    public AudioClip OnHitSound;
    public AudioClip ArmorHitSound;
    public AudioClip ReloadSound;
    public bool reloading;

    public ParticleSystem bulletTrail;

    void Start()
    {
        CurrentReserveAmmo = MaximumAmmo;
        currAmmo = MagazineSize;
    }
    void Update()
    {
        if (fireTimer >= 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if (firing && fireTimer <= 0 && CurrentAmmo > 0 && !reloading)
        {
            Shoot();
            fireTimer = 1 / RateOfFire;
        }
        else
        {
            currRecoil = Math.Clamp(currRecoil - recoilRecovery * Time.deltaTime, 0, maxRecoil);
        }


        myHudText.text = CurrentAmmo.ToString();
        myHudSecondaryText.text = CurrentReserveAmmo.ToString();
    }
    public override void TriggerDown()
    {
        firing = true;
    }

    public override void TriggerUp()
    {
        firing = false;
    }
    public override void Shoot()
    {
        Vector3 FiringDir = PickFiringDirection(bulletOrigin.forward, currRecoil);
        Debug.DrawRay(bulletOrigin.position, FiringDir * range, Color.cyan, 1f);
        primaryParticles.Play();
        audioSource.PlayOneShot(shootSound);

        //Raycast once
        Physics.Raycast(bulletOrigin.position, FiringDir, out RaycastHit hit, range, LayerMask.GetMask("WalkableTerrain", "CameraObstacle", "Enemy", "EnemyArmor"));
        if (hit.transform != null)
        {
            hit.transform.gameObject.TryGetComponent<CombatEntity>(out CombatEntity tg);
            if (tg != null)
            {
                new DamageInstance(this.Owner, this, tg);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyArmor"))
            {
                audioSource.PlayOneShot(ArmorHitSound);
            }
        }

        //ammo & recoil tracking
        currAmmo--;
        currRecoil += perShotRecoil;



        // do particle for bullet trail
        Vector3 endposition = hit.transform ? hit.point : bulletOrigin.position + FiringDir * range;
        Vector3 startPosition = bulletOrigin.position;
        //Particle system line code modified from https://discussions.unity.com/t/emit-particles-throughout-a-line-ray/227355/3
        Vector3 particlePosition = (endposition - startPosition) / 2 + startPosition; // particle system position is delta middle + start position
        float distance = Vector3.Distance(endposition, startPosition) / 2; //distance is half the total distance since line extends both ways
        int numParticles = (int)(distance * 50);

        ParticleSystem bulletClone = Instantiate(bulletTrail);

        bulletClone.transform.position = particlePosition; //update the system's position
        bulletClone.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.

        ParticleSystem.ShapeModule sm = bulletClone.shape;
        sm.radius = distance; //adjust the line size

        ParticleSystem.EmissionModule em = bulletClone.emission;
        ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
        em.SetBurst(0, b); //set burst to be the number of desired particles.
        bulletClone.Play();
        //Destroy it after it's done playing & the last particle has disappeared.
        Destroy(bulletClone.gameObject, bulletClone.main.startLifetime.constantMax + bulletClone.main.duration);

    }
    public override void OnHit(DamageInstance d)
    {
        audioSource.PlayOneShot(OnHitSound);
    }
    public override void Reload()
    {
        if (currAmmo < MagazineSize && CurrentReserveAmmo > 0 && !reloading)
        {
            reloading = true;
            StartCoroutine(DoReloading());
        }
    }
    private IEnumerator DoReloading()
    {
        audioSource.PlayOneShot(ReloadSound);

        yield return new WaitForSeconds(1);
        int difference = MagazineSize - currAmmo;
        if (CurrentReserveAmmo >= difference)
        {
            CurrentReserveAmmo -= difference;
            currAmmo += difference;
        }
        else
        {
            currAmmo += CurrentReserveAmmo;
            CurrentReserveAmmo -= CurrentReserveAmmo;
        }

        audioSource.PlayOneShot(ReloadSound); //don't need a second sound once the reload sound actually lasts a second.
        reloading = false;

    }

}
