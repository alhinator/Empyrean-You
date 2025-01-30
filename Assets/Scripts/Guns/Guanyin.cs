using UnityEngine;
public class Guanyin : Gun
{
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
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.cyan, 0.01f);
        primaryParticles.Play();
        audioSource.PlayOneShot(shootSound);
    }
}
