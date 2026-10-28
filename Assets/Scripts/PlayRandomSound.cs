using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] sounds;
    public AudioClip[] sounds2;

    [Header("Pitch")]
    [Range(0f, 1f)]
    public float pitchVariation = 1f;

    // Llamar desde una animación, evento o cualquier otro script
    private void Start()
    {
        PlaySound();
    }
    public void PlaySound()
    {
        if (audioSource == null || sounds == null || sounds.Length == 0)
            return;

        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];

        audioSource.pitch = Random.Range(
            1f - pitchVariation,
            1f + pitchVariation
        );

        audioSource.PlayOneShot(randomSound);

        audioSource.pitch = 1f;
    }

    public void PlaySound2()
    {
        if (audioSource == null || sounds2 == null || sounds2.Length == 0)
            return;

        AudioClip randomSound = sounds2[Random.Range(0, sounds2.Length)];

        audioSource.pitch = Random.Range(
            1f - pitchVariation,
            1f + pitchVariation
        );

        audioSource.PlayOneShot(randomSound);

        audioSource.pitch = 1f;
    }
}
