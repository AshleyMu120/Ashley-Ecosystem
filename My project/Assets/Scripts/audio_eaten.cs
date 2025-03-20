using UnityEngine;

public class audio_eaten : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dartfish"))
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                if (!audioSource.enabled)
                {
                    audioSource.enabled = true; // Enable the AudioSource if it is disabled
                }
                audioSource.Play(); // Play the AudioSource
            }
            else
            {
                Debug.LogWarning("No AudioSource component found on this GameObject!");
            }
        }
    }
}