using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchRocket : MonoBehaviour
{
    [SerializeField] GameObject missle;
    public void Launch()
    {
        missle.SetActive(true);
    }
}
