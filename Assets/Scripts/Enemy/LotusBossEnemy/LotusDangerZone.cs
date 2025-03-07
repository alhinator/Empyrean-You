using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotusDangerZone : MonoBehaviour
{
    private bool hiBeams = false;
    private float timeAlive;
    private float fuseTime = 2f;
    private bool slowPlayer = false;

    [SerializeField] Material lowBright, midBright, hiBright;
    private MeshRenderer mesh;
    public ParticleSystem explodeParticles;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        StartCoroutine(Flashing());
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if(timeAlive > fuseTime && !slowPlayer)
        {
            Explode();
        }

    }
    private void Explode()
    {
        mesh.enabled = false;
        explodeParticles.Play();
        slowPlayer = true;
        Destroy(this.gameObject, 11f);
    }
    private void OnTriggerStay(Collider other)
    {
        if (slowPlayer && other && other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().velocity *= 0.8f;
        }
    }

    private IEnumerator Flashing()
    {
        yield return new WaitForSeconds(0.2f);

        hiBeams = !hiBeams;
        var mats = mesh.materials;
        mats[0] = hiBeams ? midBright : lowBright;
        mesh.materials = mats;

        if (timeAlive > fuseTime - 0.5f)
        {
            mats = mesh.materials;
            mats[0] = hiBright;
            mesh.materials = mats;
        }
        StartCoroutine(Flashing());
    }
}
