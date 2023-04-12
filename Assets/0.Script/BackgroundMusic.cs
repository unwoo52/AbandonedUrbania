using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip bgm;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.Play();
    }
}
