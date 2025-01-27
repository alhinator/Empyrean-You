
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LookInverterHelper : MonoBehaviour
{
    public bool[] inverts;
    public float[] sens;

    // Start is called before the first frame update
    void Start()
    {
        inverts = new bool[2]; //0 1 = invert x, invert y
        inverts[0] = false;
        inverts[1] = true;
        sens = new float[2];
        sens[0] = 300;
        sens[1] = 5;


        SceneManager.activeSceneChanged += ApplySettingsOnSceneChange;

        DontDestroyOnLoad(this);
    }

    public void InvertXLook(bool value)
    {
        inverts[0] = value;
        ApplySettingsToCameras();
    }
    public void InvertYLook(bool value)
    {
        inverts[1] = value;
        ApplySettingsToCameras();
    }
    public void SetXSens(float value){
        sens[0] = value;
        ApplySettingsToCameras();
    }
        public void SetYSens(float value){
        sens[1] = value;
        ApplySettingsToCameras();
    }


    public void ApplySettingsToCameras()
    {
        foreach (GameObject cam in GameObject.FindGameObjectsWithTag("ControllableCamera"))
        {
            cam.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook vcam);
            if (vcam != null)
            {
                vcam.m_XAxis.m_InvertInput = inverts[0];
                vcam.m_YAxis.m_InvertInput = inverts[1];
                // vcam.m_XAxis.m_MaxSpeed = sens[0];
                // vcam.m_YAxis.m_MaxSpeed = sens[1];
            }
        }
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p != null){p.GetComponent<Player3PCam>().SetInvertBumps(inverts);}

    }
    public void ApplySettingsOnSceneChange(Scene current, Scene next){
        ApplySettingsToCameras();
    }

}
