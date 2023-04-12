using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchRocket : MonoBehaviour
{
    [SerializeField] GameObject missle;
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip AudioClip;
    public void Launch()
    {
        AudioSource.PlayOneShot(AudioClip);
        missle.SetActive(true);
    }
}
