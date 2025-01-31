using UnityEngine;
public class Guanyin : Gun
{
    [Header("Guanyin Specific vars")]
    public AudioClip OnHitSound;
    public AudioClip ReloadSound;
    public bool reloading;

    void Update()
    {
        if (fireTimer >= 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if (firing && fireTimer <= 0)
        {
            Shoot();
            fireTimer = 1 / RateOfFire;
        }
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
        Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out RaycastHit hit, range, LayerMask.GetMask("WalkableTerrain", "CameraObstacle", "Enemy"));
        if (hit.transform != null)
        {
            hit.transform.gameObject.TryGetComponent<Shootable>(out Shootable s);
            if (s != null)
            {
                s.HitDetected(playerReference, this);
            }
        }
    }
    public override void TriggerOnHitEffects(Shootable s){
        audioSource.PlayOneShot(OnHitSound);
    }
    public override void Reload()
    {
        audioSource.PlayOneShot(ReloadSound);
    }
}
