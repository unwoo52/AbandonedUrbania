using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class ThrowGrenade : MonoBehaviour
    {
        [SerializeField] GameObject grenadePrefab;
        [SerializeField] Camera Cam;
        [SerializeField] float ThrowPower = 1;
        [SerializeField] LayerMask layerMask;
        private void Update()
        {
            if(Input.GetButtonDown("Skill q"))
            {
                Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hitInfo;
                Vector3 hitPoint;
                if (Physics.Raycast(ray, out hitInfo, 1000, layerMask))
                {
                    hitPoint = hitInfo.point;
                    // 충돌한 지점의 위치를 사용하는 코드
                }
                else
                {
                    hitPoint = ray.origin + ray.direction * 1000;
                    // 레이의 사거리 끝 지점을 사용하는 코드
                }
                Vector3 dir = (hitPoint - transform.position);
                dir.Normalize();

                Grenade(grenadePrefab, dir, ThrowPower);
            }
        }
        [SerializeField] Vector3 rotPower;
        public void Grenade(GameObject grenadePrefab, Vector3 Direction, float throwPower)
        {
            Vector3 orginpos = transform.position + (Direction * 3) + Vector3.up*2;
            GameObject newGrenade = Instantiate(grenadePrefab, orginpos, Quaternion.identity);
            Rigidbody grenadeRigidbody = newGrenade.GetComponent<Rigidbody>();
            grenadeRigidbody.AddForce(Direction * throwPower, ForceMode.Impulse);

            Rigidbody rb = newGrenade.GetComponent<Rigidbody>();
            rb.AddRelativeTorque(Random.Range(0f, rotPower.x), Random.Range(0f, rotPower.y), Random.Range(0f, rotPower.z), ForceMode.Impulse);
        }

    }
}

