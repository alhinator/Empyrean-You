using System.Collections;

using UnityEngine;

public class PlayerMatSwapper : MonoBehaviour
{
    /// <summary>
    /// The saved material that will persist between scenes
    /// </summary>
    public Material savedMaterial;
    /// <summary>
    /// All black material.
    /// </summary>
    public Material baseMaterial;
    /// <summary>
    /// The active player material to change
    /// </summary>
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
        //on scene load, set active color to saved material.
        SetPlayerColor(savedMaterial);
        DebugInvis = false;

    }

    /// <summary>
    /// Set the persisting saved value for either the line or fill color.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="doBase">if true, set fill, if false, do line.</param>
    public void SetSavedColor(Color c, bool doBase)
    {
        if (doBase)
        {
            savedMaterial.SetColor(Shader.PropertyToID("_BaseColor"), c);
        }
        else
        {
            savedMaterial.SetColor(Shader.PropertyToID("_WireColor"), c);
        }
    }
    public void LerpPlayerColor(Material next)
    {
        StartCoroutine(LerpFade(variableMaterial, next));
    }
    public void LerpPlayerColor(){
        StartCoroutine(LerpFade(variableMaterial, savedMaterial));

    }

    // Update is called once per frame
    public void SetPlayerColor(Color line, Color fill)
    {
        variableMaterial.SetColor(Shader.PropertyToID("_WireColor"), line);
        variableMaterial.SetColor(Shader.PropertyToID("_BaseColor"), fill);

    }
    public void SetPlayerColor(Material m)
    {
        variableMaterial.CopyMatchingPropertiesFromMaterial(m);
    }
    public void SetInvis(bool enabled)
    {
        if (enabled)
        {
            variableMaterial.shader = WireframeTrans;
            coreMaterial.shader = WireframeTrans;
            StartCoroutine(LerpFade( savedMaterial, invisibleMaterial));
        }
        else
        {
            variableMaterial.shader = WireframeOpaque;
            coreMaterial.shader = WireframeOpaque;
            StartCoroutine(LerpFade( invisibleMaterial, savedMaterial));

        }
    }
    IEnumerator LerpFade(Material previous, Material next)
    {
        variableMaterial.CopyMatchingPropertiesFromMaterial(previous);
        for (int i = 0; i <= 100; i++)
        {
            variableMaterial.Lerp(previous, next, i * 0.01f);
            yield return new WaitForSeconds(0.005f);
        }
    }

    void OnDestroy()
    {
        //SetPlayerColor(Color.black, Color.black);
        variableMaterial.shader = WireframeOpaque;
        coreMaterial.shader = WireframeOpaque;
    }
}
