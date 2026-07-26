using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class VignetteController : MonoBehaviour
{
    public Volume volume; // arrastra tu Global Volume aquí desde el inspector

    private Vignette vignette;

    public float normalIntensity;

    void Start()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.intensity.overrideState = true; // importante
        }

        normalIntensity = vignette.intensity.value;
    }

    public void SetVignetteIntensity(float value)
    {
        if (vignette != null)
        {
            vignette.intensity.value = value;
        }
    }

    public void ResetVignetteIntensity()
    {
        if (vignette != null)
        {
            vignette.intensity.value = normalIntensity;
        }
    }

    public void PlayDamageEffect()
    {
        StartCoroutine(DamageEffect());
    }

    IEnumerator DamageEffect()
    {
        vignette.intensity.value = 0.5f;

        yield return new WaitForSeconds(0.5f);

        vignette.intensity.value = normalIntensity;
    }
}

