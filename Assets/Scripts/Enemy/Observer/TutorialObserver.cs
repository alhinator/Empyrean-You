using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialObserver : EnemiesKilledCondition
{
    // Start is called before the first frame update
    private HUDManager playerHud;
    void Start()
    {
        onKillsReached.AddListener(GoToNextLevel);
        playerHud = GameObject.FindGameObjectWithTag("Player").GetComponent<HUDManager>();
    }

    private void GoToNextLevel()
    {
        playerHud.Blackout();
        StartCoroutine(NextCoroutine());
    }
    private IEnumerator NextCoroutine()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("TutorialLevel");

        //mayube do a save game here again too?
    }
}
