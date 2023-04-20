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
        [SerializeField] GameObject CannonPos1;
        [SerializeField] GameObject CannonPos2;
        [SerializeField] bool IsTimeToShotWithCannonPos1 = false;


        [Header("발사 이펙트")]
        [SerializeField] GameObject BigShotEffect;
        [SerializeField] List<GameObject> ShotEffect;


        [Header("무기 발사 쿨타임")]
        [SerializeField] float bigCannonCooldown = 3f;
        [SerializeField] float smallCannonCooldown = 0.5f;
        float bigCannonTimer = 0f;
        float smallCannonTimer = 0f;

        [Header("명중률 설정")]
        [SerializeField] float Inaccuracy = 1;
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

            //좌우 총열 번갈아가면서 쏘기
            IsTimeToShotWithCannonPos1 = !IsTimeToShotWithCannonPos1;
            GameObject CannonMuzzle = IsTimeToShotWithCannonPos1? CannonPos1 : CannonPos2;
            Instantiate(BigShotEffect, CannonMuzzle.transform.position, CannonMuzzle.transform.rotation);
            //CreateBullet(pos, BigCannonball, CannonMuzzle.transform, Inaccuracy);
            NoneTargetCreateBullet(pos, Cannonball, CannonMuzzle.transform, Inaccuracy);
        }

        public void ShotsmallCannon(Vector3 pos)
        {
            if (smallCannonTimer > 0f) return;

            AudioSource.PlayOneShot(Cannon);
            animator.SetTrigger("Trigger_BigCannon");


            //좌우 총열 번갈아가면서 쏘기
            IsTimeToShotWithCannonPos1 = !IsTimeToShotWithCannonPos1;
            GameObject CannonMuzzle = IsTimeToShotWithCannonPos1 ? CannonPos1 : CannonPos2;
            foreach(var shotFlame in ShotEffect)
            {
                Instantiate(shotFlame, CannonMuzzle.transform.position, CannonMuzzle.transform.rotation);
            }
            //CreateBullet(pos, Cannonball, CannonMuzzle.transform, Inaccuracy);
            NoneTargetCreateBullet(pos, Cannonball, CannonMuzzle.transform, Inaccuracy);
        }
        void CreateBullet(Vector3 targetPosition, GameObject gameObject, Transform CannonMuzzle , float inaccuracy)
        {
            GameObject bullet = Instantiate(gameObject);
            Vector3 direction = (targetPosition - CannonMuzzle.transform.position).normalized;
            bullet.transform.position = CannonMuzzle.transform.position;

            // 명중률을 떨어뜨리기 위해 방향 벡터를 랜덤하게 비틀어줍니다.
            float angleX = Random.Range(-inaccuracy, inaccuracy);
            float angleY = Random.Range(-inaccuracy, inaccuracy);
            direction = Quaternion.AngleAxis(angleX, Vector3.right) * Quaternion.AngleAxis(angleY, Vector3.up) * direction;
            /*
            // 명중률을 떨어뜨리기 위해 방향 벡터를 랜덤하게 비틀어줍니다.
            float angle = Random.Range(-inaccuracy, inaccuracy);
            direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            */
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            if (bullet.TryGetComponent(out IProjectileOwner projectileOwner))
            {
                projectileOwner.GetProjectileOwner(this.gameObject);
            }
        }


        void NoneTargetCreateBullet(Vector3 targetPosition, GameObject gameObject, Transform CannonMuzzle, float inaccuracy)
        {
            GameObject bullet = Instantiate(gameObject);
            Vector3 direction = (CannonMuzzle.transform.forward).normalized;
            bullet.transform.position = CannonMuzzle.transform.position;

            // 명중률을 떨어뜨리기 위해 방향 벡터를 랜덤하게 비틀어줍니다.
            float angleX = Random.Range(-inaccuracy, inaccuracy);
            float angleY = Random.Range(-inaccuracy, inaccuracy);
            direction = Quaternion.AngleAxis(angleX, Vector3.right) * Quaternion.AngleAxis(angleY, Vector3.up) * direction;
            /*
            // 명중률을 떨어뜨리기 위해 방향 벡터를 랜덤하게 비틀어줍니다.
            float angle = Random.Range(-inaccuracy, inaccuracy);
            direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            */
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            if(bullet.TryGetComponent(out IProjectileOwner projectileOwner) )
            {
                projectileOwner.GetProjectileOwner(this.gameObject);
            }
        }
    }

}
