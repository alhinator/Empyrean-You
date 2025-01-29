using UnityEngine;
public class Artemis : Gun
{
    public AudioClip chargeSound;
    public AudioClip holdSound;
    void Update()
    {
        if (firing)
        {
            currCharge += Time.deltaTime;
        }
        if (currCharge > chargeTime && audioSource.clip == chargeSound)
        {
            audioSource.clip = holdSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    public override void TriggerDown()
    {
        Debug.Log("In triggerdowm artemis");
        firing = true;
        audioSource.clip = chargeSound;
        audioSource.loop = false;
        audioSource.Play();
        primaryParticles.Play();
        //Shoot();
    }

    public override void TriggerUp()
    {

        //Debug.Log(currCharge);
        if (currCharge > chargeTime)
        {
            Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.red, 0.01f);
            Shoot();
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == chargeSound)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
        audioSource.loop = false;
        currCharge = 0;
        firing = false;
    }
    protected override void Shoot()
    {
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.PlayOneShot(shootSound);
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.cyan, 0.01f);
    }

}
