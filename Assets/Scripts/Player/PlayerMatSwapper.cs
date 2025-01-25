using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class PlayerMatSwapper : MonoBehaviour
{
    public Material baseMaterial;
    public Material variableMaterial;
    public Material invisibleMaterial;
    public Material coreMaterial;

    public Shader WireframeOpaque;
    public Shader WireframeTrans;

    public bool DebugInvis;

    // Start is called before the first frame update
    void Start()
    {
        variableMaterial.shader = WireframeOpaque;
        SetPlayerColor(Color.black, Color.black);
        DebugInvis = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SPACE"))
        {
            DebugInvis = !DebugInvis;
            SetInvis(DebugInvis);
        }
    }

    public void SetPlayerColor(Color line, Color fill)
    {
        variableMaterial.SetColor(Shader.PropertyToID("_WireColor"), line);
        variableMaterial.SetColor(Shader.PropertyToID("_BaseColor"), fill);
    }
    public void SetInvis(bool enabled)
    {
        if (enabled)
        {
            variableMaterial.shader = WireframeTrans;
            coreMaterial.shader = WireframeTrans;
            StartCoroutine(LerpFade(variableMaterial, baseMaterial, invisibleMaterial));
        }
        else
        {
            variableMaterial.shader = WireframeOpaque;
            coreMaterial.shader = WireframeOpaque;
            StartCoroutine(LerpFade(variableMaterial, invisibleMaterial, baseMaterial));

        }
    }
    IEnumerator LerpFade(Material active, Material previous, Material next)
    {
        active.CopyMatchingPropertiesFromMaterial(previous);
        for (int i = 0; i <= 100; i++)
        {
            active.Lerp(previous, next, i * 0.01f);
            yield return new WaitForSeconds(0.005f);
        }
    }

    void OnDestroy()
    {
        SetPlayerColor(Color.black, Color.black);
        variableMaterial.shader = WireframeOpaque;
        coreMaterial.shader = WireframeOpaque;
    }
}
