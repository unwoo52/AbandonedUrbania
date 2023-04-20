using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class GrenadeEffect : MonoBehaviour
{
    public float sphereRadius = 5f;
    public float distanceThreshold = 10f;
    GameObject robot;
    [SerializeField] AudioClip AudioClip;


    private void Start()
    {
        PlaySound();
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider collider in colliders)
        {
            robot = collider.gameObject;

            if (!IsColliderRobot()) continue;
            if (!IsRobotInDistance()) continue;
                        
            if (robot.TryGetComponent(out IDrawAttention drawAttention))
            {
                drawAttention.OnDrawAttention(this.gameObject);
            }            
        }
    }

    void PlaySound()
    {
        if (TryGetComponent(out AudioSource audio))
        {
            audio.PlayOneShot(AudioClip);
        }
        else Debug.LogWarning("Audio source does not exist!");
    }

    bool IsColliderRobot()
    {
        return GameManager.instance.Robots.Contains(robot);
    }

    bool IsRobotInDistance()
    {
        return Vector3.Distance(transform.position, robot.transform.position) > distanceThreshold;
    }
}
