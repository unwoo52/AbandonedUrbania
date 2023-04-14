using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class Weapon : MonoBehaviour
{
    [SerializeField] AudioClip FireSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Camera m_CameraFocus;


    [Header("bullet")]
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform muzzleTransform;
    [SerializeField] float MaxShotDistance = 1000f;
    [SerializeField] LayerMask hitableMask;

    bool Fire1;
    private void Update()
    {
        GetInput();
        InputProcess();
    }
    void GetInput()
    {
        Fire1 = Input.GetButtonDown("Fire1");
    }
    void InputProcess()
    {
        if (Fire1) Fire();
    }
    void Fire()
    {
        if(audioSource!= null)
            audioSource.PlayOneShot(FireSound);

        Ray ray = m_CameraFocus.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 direction;

        if (Physics.Raycast(ray, out RaycastHit hit, MaxShotDistance, hitableMask))
        {
            Debug.Log($"hitObject ::: {hit.transform.name}");

            Debug.DrawRay(m_CameraFocus.transform.position, hit.point - m_CameraFocus.transform.position, Color.red, 3f);

            direction = (hit.point - muzzleTransform.transform.position).normalized;
        }
        else
        {
            direction = (ray.GetPoint(MaxShotDistance) - muzzleTransform.transform.position).normalized;
        }


        GameObject bullet = Instantiate(Bullet);
        bullet.transform.position = muzzleTransform.position;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
    }
}
