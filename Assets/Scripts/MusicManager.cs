using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Se till att bara en MusicManager finns
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);  // Gör så att objektet inte förstörs vid scenbyte

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;   // Säkerställ loop
            audioSource.Play();        // Starta musiken
        }
        else
        {
            Debug.LogWarning("Ingen AudioSource hittades på MusicManager!");
        }
    }
}