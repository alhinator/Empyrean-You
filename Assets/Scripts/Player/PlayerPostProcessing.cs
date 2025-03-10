
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerPostProcessing : MonoBehaviour
{
    private Volume ingameVol;
    private Volume HUDVol;

    private ChromaticAberration aberr;
    private MotionBlur blur;
    private LensDistortion fisheye;
    private ColorAdjustments grayscale;
    private void Start()
    {
        ingameVol = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
        HUDVol = GameObject.FindGameObjectWithTag("UIVolume").GetComponent<Volume>();

        if (ingameVol.profile.TryGet<ChromaticAberration>(out var ab))
        {
            aberr = ab;
        }
        if (ingameVol.profile.TryGet<MotionBlur>(out var bl))
        {
            blur = bl;
        }
        if (ingameVol.profile.TryGet<LensDistortion>(out var len))
        {
            fisheye = len;
        }
        if (HUDVol.profile.TryGet<ColorAdjustments>(out var gr))
        {
            grayscale = gr;
        }

    }
    private void Update()
    {
        ReduceAberration();
    }

    private void ReduceAberration()
    {
        aberr.intensity.value = Mathf.Lerp(aberr.intensity.value, 0, 2 * Time.deltaTime);
        fisheye.intensity.value = Mathf.Lerp(fisheye.intensity.value, 0, 2 * Time.deltaTime);
    }
    public void TriggerAberration()
    {
        aberr.intensity.value = 1;
        fisheye.intensity.value = -0.25f;
    }

    public void TurnOnBlur()
    {
        blur.active = true;
        StartCoroutine(TurnOffBlur());
    }
    private IEnumerator TurnOffBlur()
    {
        yield return new WaitForSeconds(0.3f);
        blur.active = false;
    }
    private bool GrayScaleActive
    {
        get
        {
            return grayscale.active;
        }
        set
        {
            grayscale.active = value;
        }
    }
}
