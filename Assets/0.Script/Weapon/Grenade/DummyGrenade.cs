using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class DummyGrenade : MonoBehaviour
    {
        public LayerMask groundLayer; // 바닥 레이어마스크
        public float maxRollingTime = 5f; // 수류탄이 굴러가는 최대 시간
        public float decelerationTime = 2f; // 수류탄이 감속하는 시간
        public float explosionTime = 3f;

        [SerializeField] Rigidbody rb;
        private bool isGrounded = false; // 수류탄이 바닥에 닿았는지 여부를 판단하는 변수
        private float rollingTime = 0f; // 수류탄이 굴러간 시간을 측정하는 변수
        [SerializeField] GameObject GrenadeEffect;

        void Start()
        {
            if(rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            Invoke(nameof(Explode), explosionTime);
        }

        void FixedUpdate()
        {
            // 수류탄이 바닥에 닿았으면, 점점 감속하다가 멈춘다.
            if (isGrounded)
            {
                if (rollingTime < maxRollingTime)
                {
                    // 감속 시작
                    float deceleration = 1 - (rollingTime / maxRollingTime);
                    rb.velocity *= deceleration;
                    rollingTime += Time.fixedDeltaTime;
                }
                else
                {
                    // 굴러가는 시간이 최대 시간을 넘으면 수류탄은 멈춘다.
                    rb.velocity = Vector3.zero;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 수류탄이 바닥에 닿았는지 여부를 판단한다.
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                isGrounded = true;
            }
        }

        private void Explode()
        {
            GameObject effect = Instantiate(GrenadeEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

