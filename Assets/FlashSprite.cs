using UnityEngine;
using System.Collections;

public class FlashSprite : MonoBehaviour
{
    [Header("Flash Settings")]
    public Material flashMaterial;      // El material blanco

    public Material originalMaterial;  // Material original del sprite
    public SpriteRenderer[] sr;
    private Coroutine flashRoutine;

    void Awake()
    {
        if (originalMaterial == null)
        {
            originalMaterial = sr[0].material;
        }

    }

    public void Flash(float duration = 0.1f)
    {

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        Debug.Log("Flash Sprite");
        for (int i = 0; i < sr.Length; i++)
        {
            sr[i].material = flashMaterial;
        }
        yield return new WaitForSecondsRealtime(duration);
        for (int i = 0; i < sr.Length; i++)
        {
            sr[i].material = originalMaterial;
        }
    }
}

