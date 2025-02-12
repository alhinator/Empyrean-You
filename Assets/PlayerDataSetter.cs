using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataSetter : MonoBehaviour
{
    public int Frame;
    public int LeftWeapon;
    public int RightWeapon;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += SetPlayerData;
    }

    private void SetPlayerData(Scene prev, Scene next){
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            Debug.Log("Found Player!");
            PlayerCombatManager pcm = p.GetComponent<PlayerCombatManager>();
            if (pcm != null){
                pcm.AssignWeapons(LeftWeapon, RightWeapon);
                pcm.AssignFrame(Frame);
            }
        } else{
            throw new System.Exception("Could not find player upon new scene loading.");
        }
    }

}
