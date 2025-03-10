using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
public class Raijin : Gun
{
    [Header("Weapon Specific Properties")]
    public Transform SecondaryBulletOrigin;

    [SerializeField] ParticleSystem secondaryParticles;
    [SerializeField] ParticleSystem BulletTrail;
    [SerializeField] float BaseRateOfFire, RateOfFireAcceleration, MaxRateOfFire;

    [SerializeField] float BaseRotationSpeed, RotationSpeedAcceleration, MaxRotationSpeed; //Measured in degrees per second
    [SerializeField] Transform topBarrel, BottomBarrel;
    private float CurrRotationSpeed;

    public AudioClip HitSound;
    public AudioClip ArmorHitSound;




    protected override void Start()
    {
        base.Start();
        RateOfFire = BaseRateOfFire;
        currAmmo = MaxMagazineSize;



        myHudSecondaryText.text = gunStrings.GetEntry("ui.ammo").Value;

    }
    void Update()
    {
        CalculateSpinSpeedAndRoF();
        DoBarrelRotation();

        if (fireTimer >= 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if (firing && fireTimer <= 0 && CurrentAmmo > 0)
        {
            Shoot();
            fireTimer = 1 / RateOfFire;
        }
        else
        {
            currRecoil = Math.Clamp(currRecoil - recoilRecovery * Time.deltaTime, 0, maxRecoil);
        }

        myHudText.text = CurrentAmmo.ToString();


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
        secondaryParticles.Play();
        audioSource.PlayOneShot(shootSound);


        //Raycast once
        Physics.Raycast(bulletOrigin.position, FiringDir, out RaycastHit hit, range, LayerMask.GetMask("WalkableTerrain", "CameraObstacle", "Enemy", "EnemyArmor"));
        if (hit.transform != null)
        {
            hit.transform.gameObject.TryGetComponent<CombatEntity>(out CombatEntity tg);
            if (tg != null)
            {
                audioSource.PlayOneShot(HitSound);

                new DamageInstance(this.Owner, this, tg);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyArmor"))
            {
                audioSource.PlayOneShot(ArmorHitSound);
                (Owner as PlayerCombatManager).SpawnFloatingText(hit.point, "BLOCK", Color.red);
            }
        }

        //ammo & recoil tracking
        currAmmo--;
        currRecoil += perShotRecoil;



        // do particle for bullet trail
        Vector3 endposition = hit.transform ? hit.point : bulletOrigin.position + FiringDir * range;
        Vector3 startPosition = bulletOrigin.position;
        Vector3 startPosition2 = SecondaryBulletOrigin.position;
        //Particle system line code modified from https://discussions.unity.com/t/emit-particles-throughout-a-line-ray/227355/3
        Vector3 particlePosition = (endposition - startPosition) / 2 + startPosition; // particle system position is delta middle + start position
        float distance = Vector3.Distance(endposition, startPosition) / 2; //distance is half the total distance since line extends both ways
        Vector3 particlePosition2 = (endposition - startPosition2) / 2 + startPosition2; // particle system position is delta middle + start position

        int numParticles = (int)(distance * 50);

        //instantiate twice now
        ParticleSystem bulletClone, bulletClone2;
        bulletClone = Instantiate(BulletTrail);
        bulletClone2 = Instantiate(BulletTrail);

        bulletClone.transform.position = particlePosition; //update the system's position
        bulletClone2.transform.position = particlePosition2; //update the system's position
        bulletClone.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.
        bulletClone2.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.

        ParticleSystem.ShapeModule sm = bulletClone.shape;
        sm.radius = distance; //adjust the line size
        ParticleSystem.ShapeModule sm2 = bulletClone2.shape;
        sm2.radius = distance; //adjust the line size

        ParticleSystem.EmissionModule em = bulletClone.emission;
        ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
        em.SetBurst(0, b); //set burst to be the number of desired particles.
        bulletClone.Play();

        ParticleSystem.EmissionModule em2 = bulletClone2.emission;
        ParticleSystem.Burst b2 = new ParticleSystem.Burst(0, numParticles);
        em2.SetBurst(0, b); //set burst to be the number of desired particles.
        bulletClone2.Play();

        //Destroy it after it's done playing & the last particle has disappeared.
        Destroy(bulletClone.gameObject, bulletClone.main.startLifetime.constantMax + bulletClone.main.duration);
        Destroy(bulletClone2.gameObject, bulletClone2.main.startLifetime.constantMax + bulletClone2.main.duration);
    }

    private void CalculateSpinSpeedAndRoF()
    {
        if (firing)
        {
            CurrRotationSpeed += Time.deltaTime * RotationSpeedAcceleration;
            RateOfFire += Time.deltaTime * RateOfFireAcceleration;
        }
        else
        {
            CurrRotationSpeed -= Time.deltaTime * (CurrRotationSpeed / 1.5f + 1);
            RateOfFire -= Time.deltaTime * (RateOfFire / 1.5f + 1);
        }
        CurrRotationSpeed = Math.Clamp(CurrRotationSpeed, BaseRotationSpeed, MaxRotationSpeed);
        RateOfFire = Math.Clamp(RateOfFire, BaseRateOfFire, MaxRateOfFire);
    }
    private void DoBarrelRotation()
    {
        topBarrel.Rotate(new Vector3(0, Time.deltaTime * CurrRotationSpeed, 0));
        BottomBarrel.Rotate(new Vector3(0, -Time.deltaTime * CurrRotationSpeed, 0));
    }

}
