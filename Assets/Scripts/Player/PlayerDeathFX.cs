using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathFX : MonoBehaviour
{
    [SerializeField] ParticleSystem DeathExplosion;
    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    public void DoDeathEffects()
    {
        StartCoroutine(LerpDownTime());
        DeathExplosion.Play();
    }
    private IEnumerator LerpDownTime()
    {
        float timeStep = 0.01f;
        float slowLimit = 0.2f;
        while (Time.timeScale > slowLimit)
        {
            yield return new WaitForSecondsRealtime(timeStep);
            Time.timeScale -= timeStep;
        }
        yield return new WaitForSecondsRealtime(1);
        DeathExplosion.Clear();
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;

    }
}
