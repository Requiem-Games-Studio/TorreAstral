using UnityEngine;
using System.Collections;

public class GameFeelManager : MonoBehaviour
{
    [Header("HITLAG")]
    public float defaultHitlag = 0.05f;

    [Header("FLASH")]
    public float flashDuration = 0.05f;

    [Header("SHAKE")]
    public CameraShake cameraShake;
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    public static GameFeelManager Instance;
    public VignetteController controller;

    void Awake()
    {
        Instance = this;
    }

    public void DoImpactPlayer()
    {
        Debug.Log("Impact To Player");
        controller.PlayDamageEffect();
        DoSlowMotion(0.4f, 0.2f);
        cameraShake?.Shake(0.08f, 0.05f);
    }
    public void DoImpact()
    {
        Debug.Log("Impact");
        DoSlowMotion(0.4f, 0.2f);
        cameraShake?.Shake(0.06f, 0.03f);
    }

    public void DoImpactToKill()
    {
        Debug.Log("Impact To Kill");
        DoSlowMotion(0.1f, 0.8f);
        cameraShake?.Shake(0.1f, 0.1f);
    }

    public void DoParryImpact()
    {
        Debug.Log("Impact Parry");
        DoSlowMotion(0.2f, 0.5f);
        cameraShake?.Shake(0.12f, 0.25f);       // shake más potente
    }



    public void DoSlowMotion(float slowFactor = 0.2f, float duration = 0.5f)
    {
        StartCoroutine(SlowMotionRoutine(slowFactor, duration));
    }

    private IEnumerator SlowMotionRoutine(float slowFactor, float duration)
    {
        // Bajar tiempo
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // necesario para físicas

        yield return new WaitForSecondsRealtime(duration);

        // Regresar el tiempo suavemente
        float t = 0f;
        float recoveryTime = 0.3f; // tiempo de volver a normal

        while (t < recoveryTime)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowFactor, 1f, t / recoveryTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
