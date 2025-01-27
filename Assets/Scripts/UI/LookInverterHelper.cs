
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class LookInverterHelper : MonoBehaviour
{
    public bool[] inverts;

    // Start is called before the first frame update
    void Start()
    {
        inverts = new bool[2]; //0 1 = invert x, invert y
        inverts[0] = false;
        inverts[1] = true;

        DontDestroyOnLoad(this);
    }

    public void InvertXLook(bool value)
    {
        inverts[0] = value;
        ApplyInvertsToCameras();
    }
    public void InvertYLook(bool value)
    {
        inverts[1] = value;
        ApplyInvertsToCameras();
    }

    public void ApplyInvertsToCameras()
    {
        foreach (GameObject cam in GameObject.FindGameObjectsWithTag("ControllableCamera"))
        {
            cam.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook vcam);
            if (vcam != null)
            {
                vcam.m_XAxis.m_InvertInput = inverts[0];
                vcam.m_XAxis.m_InvertInput = inverts[1];
            }

        }

    }

}
