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
    protected override void Shoot()
    {
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * range, Color.cyan, 0.01f);
        primaryParticles.Play();
        audioSource.PlayOneShot(shootSound);

        //Raycast once
        Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out RaycastHit hit, range, LayerMask.GetMask("WalkableTerrain", "CameraObstacle", "Enemy", "EnemyArmor"));
        if (hit.transform != null)
        {
            hit.transform.gameObject.TryGetComponent<Shootable>(out Shootable s);
            if (s != null)
            {
                s.HitDetected(playerReference, this);
            } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyArmor")){
                audioSource.PlayOneShot(ArmorHitSound);
            }
        } 

        //ammo tracking
        currAmmo--;

    }
    public override void TriggerOnHitEffects(Shootable s)
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
