using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//Throwaway class used for stuff that doesn't fit elsewhere

public class PlayerController : MonoBehaviour
{
    private Animator animController;
    private Player3PCam player;

    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem boostLeft, boostRight, hoverLeft, hoverRight;

    [SerializeField] AudioClip hoverSound, jumpSound, dodgeSound;


    private void Start()
    {
        animController = GetComponentInChildren<Animator>();
        player = GetComponent<Player3PCam>();
    }

    void Update()
    {
        if (player.IsHovering)
        {
            hoverLeft.Play();
            hoverRight.Play();
            if (audioSource.clip == null)
            {
                audioSource.loop = true;
                audioSource.clip = hoverSound;
                audioSource.Play();
            }

        }
        else
        {
            audioSource.loop = false;
            audioSource.clip = null;
        }
    }
    public void OnMove(InputValue v)
    {
        Vector2 rawInput = v.Get<Vector2>();
        animController.SetFloat("InputXAxis", rawInput.x);
        animController.SetFloat("InputYAxis", rawInput.y);
    }

    public void OnDodge(InputValue v)
    {
        if (player.BoostsRemaining > 0)
        {
            boostLeft.Play();
            boostRight.Play();
            audioSource.PlayOneShot(dodgeSound);
        }

    }
    public void OnJump(InputValue v)
    {
        if (player.BoostsRemaining > 0)
        {
            StartCoroutine(ParticlesAfterDelay(0.2f));
        }
    }

    private IEnumerator ParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        boostLeft.Play();
        boostRight.Play();
        audioSource.PlayOneShot(jumpSound);

    }



}
