using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TESTSCrupt : MonoBehaviour
{
    [SerializeField] UnityEvent test;
    void Start()
    {
        test?.Invoke();
    }
}
