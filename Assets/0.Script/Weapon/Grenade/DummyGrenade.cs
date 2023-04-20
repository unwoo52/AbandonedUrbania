using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class DummyGrenade : MonoBehaviour
    {
        public LayerMask groundLayer; // �ٴ� ���̾��ũ
        public float maxRollingTime = 5f; // ����ź�� �������� �ִ� �ð�
        public float decelerationTime = 2f; // ����ź�� �����ϴ� �ð�
        public float explosionTime = 3f;

        [SerializeField] Rigidbody rb;
        private bool isGrounded = false; // ����ź�� �ٴڿ� ��Ҵ��� ���θ� �Ǵ��ϴ� ����
        private float rollingTime = 0f; // ����ź�� ������ �ð��� �����ϴ� ����
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
            // ����ź�� �ٴڿ� �������, ���� �����ϴٰ� �����.
            if (isGrounded)
            {
                if (rollingTime < maxRollingTime)
                {
                    // ���� ����
                    float deceleration = 1 - (rollingTime / maxRollingTime);
                    rb.velocity *= deceleration;
                    rollingTime += Time.fixedDeltaTime;
                }
                else
                {
                    // �������� �ð��� �ִ� �ð��� ������ ����ź�� �����.
                    rb.velocity = Vector3.zero;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // ����ź�� �ٴڿ� ��Ҵ��� ���θ� �Ǵ��Ѵ�.
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

