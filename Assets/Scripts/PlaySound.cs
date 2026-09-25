using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] sounds;

    // Llamar desde una animación, evento o cualquier otro script
    public void Play()
    {
        if (audioSource == null || sounds == null || sounds.Length == 0)
            return;

        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];

        audioSource.pitch = 1;

        audioSource.PlayOneShot(randomSound);

    }

    public void PlayLowPitch()
    {
        if (audioSource == null || sounds == null || sounds.Length == 0)
            return;

        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];

        audioSource.pitch = 0.8f;

        audioSource.PlayOneShot(randomSound);

    }

    public void PlayHighPitch()
    {
        if (audioSource == null || sounds == null || sounds.Length == 0)
            return;

        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];

        audioSource.pitch = 1.5f;

        audioSource.PlayOneShot(randomSound);

    }
}
