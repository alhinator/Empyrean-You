using System;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Artemis : Gun
{
    [Header("Artemis Unique")]
    public AudioClip chargeSound;
    public AudioClip holdSound;
    public ParticleSystem firedParticles;
    void Start()
    {
        firedParticles.transform.parent = null;
        myHudSecondaryText.text = "CHG";
    }
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
        myHudText.text = ((int)Math.Clamp(currCharge / chargeTime * 100, 0, 100)).ToString();

    }
    public override void TriggerDown()
    {
        //Debug.Log("In triggerdowm artemis");
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
            Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * range, Color.red, 0.01f);
            Shoot();
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == chargeSound)
            {
                primaryParticles.Stop();
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
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * range, Color.cyan, 1f);

        //Firstly, we want to draw a raycast *through* the current aimPoint until we hit a scene object or hit our max range.
        //This will determine our max raycast length when checking for hits against enemies.
        Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out RaycastHit hit, range, LayerMask.GetMask("WalkableTerrain", "CameraObstacle"));
        //now set our end position.
        //Debug.Log(hit.transform);
        Vector3 endposition = hit.transform ? hit.point : bulletOrigin.position + bulletOrigin.forward * range;
        Vector3 startPosition = bulletOrigin.position;
        //Particle system line code modified from https://discussions.unity.com/t/emit-particles-throughout-a-line-ray/227355/3

        Vector3 particlePosition = (endposition - startPosition) / 2 + startPosition; // particle system position is delta middle + start position
        float distance = Vector3.Distance(endposition, startPosition) / 2; //distance is half the total distance since line extends both ways
        int numParticles = (int)(distance * 50);

        firedParticles.transform.position = particlePosition; //update the system's position
        firedParticles.transform.LookAt(endposition); //adjust the look rotation. NOTE: the particle system has a y rotation of 90 in the shape module for this to work.

        ParticleSystem.ShapeModule sm = firedParticles.shape;
        sm.radius = distance; //adjust the line size

        ParticleSystem.EmissionModule em = firedParticles.emission;
        ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
        em.SetBurst(0, b); //set burst to be the number of desired particles.
        firedParticles.Play();

        //Now RaycastAll to enemy layer using endPos as our target position.
        RaycastHit[] hits = Physics.RaycastAll(new Ray(startPosition, (endposition - startPosition).normalized), distance * 2, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit h in hits){
            h.transform.gameObject.TryGetComponent<Shootable>(out Shootable s);
            if(s != null){
                s.HitDetected(playerReference, this);
            }
        }
    }

}
