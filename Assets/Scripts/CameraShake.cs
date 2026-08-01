using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Coroutine shakeCoroutine;
    private Vector3 shakeOffset = Vector3.zero;

    public Vector3 CurrentShakeOffset => shakeOffset;

    public void Shake(float duration = 0.1f, float magnitude = 0.15f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCamera(duration, magnitude));
    }


    public IEnumerator ShakeCamera(float duracion, float intensidad)
    {
        float TiempoTranscurrido = 0;

        Vector3 PosInicial = transform.position;

        while (TiempoTranscurrido < duracion)
        {
            TiempoTranscurrido += Time.deltaTime;

            float PTT = TiempoTranscurrido / duracion;

            float compuerta = 1 - Mathf.Clamp(PTT * 4 - 3, 0f, 1f);

            float x = Random.Range(-1f, 1f) * compuerta * intensidad;
            float y = Random.Range(-1f, 1f) * compuerta * intensidad;
            float z = Random.Range(-1f, 1f) * compuerta * intensidad;

            transform.position += new Vector3(x, y, z);

            yield return null;
        }

        transform.position = PosInicial;
    }
}
