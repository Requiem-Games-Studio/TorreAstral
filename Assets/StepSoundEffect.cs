using UnityEngine;

public class StepSoundEffect : MonoBehaviour
{
    public enum SurfaceType
    {
        Grass,
        Concrete,
        Wood
    }

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Grass - Left")]
    public AudioClip grassLeft1;
    public AudioClip grassLeft2;

    [Header("Grass - Right")]
    public AudioClip grassRight1;
    public AudioClip grassRight2;

    [Header("Concrete - Left")]
    public AudioClip concreteLeft1;
    public AudioClip concreteLeft2;

    [Header("Concrete - Right")]
    public AudioClip concreteRight1;
    public AudioClip concreteRight2;

    [Header("Wood - Left")]
    public AudioClip woodLeft1;
    public AudioClip woodLeft2;

    [Header("Wood - Right")]
    public AudioClip woodRight1;
    public AudioClip woodRight2;

    [Header("Ground Detection")]
    public LayerMask groundLayers;
    public float rayDistance = 1f;

    [Header("Pitch Variation")]
    [Range(0f, 1f)]
    public float pitchVariation = 1f;

    // Llamado desde la animación
    // true = izquierdo
    // false = derecho
    public void PlayStep1()
    {
        SurfaceType surface = DetectSurface();

        AudioClip clip = GetRandomStepClip(surface, true);

        if (clip == null)
            return;

        audioSource.pitch = Random.Range(
            1f - pitchVariation,
            1f + pitchVariation
        );

        audioSource.PlayOneShot(clip);

        audioSource.pitch = 1f;
    }

    public void PlayStep2()
    {
        SurfaceType surface = DetectSurface();

        AudioClip clip = GetRandomStepClip(surface, false);

        if (clip == null)
            return;

        audioSource.pitch = Random.Range(
            1f - pitchVariation,
            1f + pitchVariation
        );

        audioSource.PlayOneShot(clip);

        audioSource.pitch = 1f;
    }

    private SurfaceType DetectSurface()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            rayDistance,
            groundLayers
        );

        if (hit.collider == null)
            return SurfaceType.Grass;

        if (hit.collider.CompareTag("Concrete"))
            return SurfaceType.Concrete;

        if (hit.collider.CompareTag("Wood"))
            return SurfaceType.Wood;

        if (hit.collider.CompareTag("Grass"))
            return SurfaceType.Grass;

        return SurfaceType.Grass;
    }

    private AudioClip GetRandomStepClip(
        SurfaceType surface,
        bool leftFoot)
    {
        AudioClip clip1 = null;
        AudioClip clip2 = null;

        switch (surface)
        {
            case SurfaceType.Grass:

                if (leftFoot)
                {
                    clip1 = grassLeft1;
                    clip2 = grassLeft2;
                }
                else
                {
                    clip1 = grassRight1;
                    clip2 = grassRight2;
                }

                break;

            case SurfaceType.Concrete:

                if (leftFoot)
                {
                    clip1 = concreteLeft1;
                    clip2 = concreteLeft2;
                }
                else
                {
                    clip1 = concreteRight1;
                    clip2 = concreteRight2;
                }

                break;

            case SurfaceType.Wood:

                if (leftFoot)
                {
                    clip1 = woodLeft1;
                    clip2 = woodLeft2;
                }
                else
                {
                    clip1 = woodRight1;
                    clip2 = woodRight2;
                }

                break;
        }

        return Random.Range(0, 2) == 0 ? clip1 : clip2;
    }
}
