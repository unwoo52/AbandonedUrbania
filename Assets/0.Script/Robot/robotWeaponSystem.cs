using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class robotWeaponSystem : MonoBehaviour
    {
        [SerializeField] AudioSource AudioSource;
        [SerializeField] AudioClip BigCannon;
        [SerializeField] AudioClip Cannon;
        [SerializeField] Animator animator;

        [Header("Bullet Object")]
        [SerializeField] GameObject BigCannonball;
        [SerializeField] GameObject Cannonball;


        [Header("총열 입구")]
        [SerializeField] GameObject CannonPos;


        [Header("발사 이펙트")]
        [SerializeField] GameObject BigShotEffect;
        [SerializeField] GameObject ShotEffect;


        [Header("무기 발사 쿨타임")]
        [SerializeField] float bigCannonCooldown = 3f;
        [SerializeField] float smallCannonCooldown = 0.5f;
        float bigCannonTimer = 0f;
        float smallCannonTimer = 0f;
        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (bigCannonTimer > 0f)
                bigCannonTimer -= Time.deltaTime;

            if (smallCannonTimer > 0f)
                smallCannonTimer -= Time.deltaTime;
        }
        public void AllWeaponFire(Vector3 pos)
        {
            if (bigCannonTimer <= 0f)
            {
                ShotBigCannon(pos);
                bigCannonTimer = bigCannonCooldown;
            }

            if (smallCannonTimer <= 0f)
            {
                ShotsmallCannon(pos);
                smallCannonTimer = smallCannonCooldown;
            }
        }

        public void ShotBigCannon(Vector3 pos)
        {
            if (bigCannonTimer > 0f) return;

            AudioSource.PlayOneShot(BigCannon);
            animator.SetTrigger("Trigger_BigCannon");
            Instantiate(BigShotEffect, CannonPos.transform.position, CannonPos.transform.rotation);
            CreateBullet(pos, BigCannonball);
        }

        public void ShotsmallCannon(Vector3 pos)
        {
            if (smallCannonTimer > 0f) return;

            AudioSource.PlayOneShot(Cannon);
            animator.SetTrigger("Trigger_BigCannon");
            Instantiate(ShotEffect, CannonPos.transform.position, CannonPos.transform.rotation);
            CreateBullet(pos, Cannonball);
        }
        void CreateBullet(Vector3 targetPosition, GameObject gameObject)
        {
            GameObject bullet = Instantiate(gameObject);
            Vector3 direction = (targetPosition - CannonPos.transform.position).normalized;
            bullet.transform.position = CannonPos.transform.position;

            bullet.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
