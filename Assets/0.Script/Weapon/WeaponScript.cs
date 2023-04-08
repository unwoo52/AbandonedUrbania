using UnityEngine;

namespace Urban_KimHyeonWoo
{

    public class WeaponScript : MonoBehaviour
    {
        [SerializeField] AudioClip fireSoundClip;
        [SerializeField] GameObject bulletPrefab;
        [SerializeField] GameObject WeaponMuzzle;
        [SerializeField] float shotDelay = 0.2f;
        [SerializeField] LayerMask HitTarget;
        [SerializeField] float bulletMaxDistance;

        Animator animator;
        AudioSource audioSource;
        private float elapsedTime = 0f;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {

            // 마우스 왼쪽 버튼 클릭 시
            if (Input.GetMouseButtonDown(0))
            {
                // 쿨타임이 지나지 않았으면 함수를 종료합니다.
                if (elapsedTime < shotDelay)
                {
                    return;
                }

                // 애니메이터 파라미터 활성화
                animator.SetBool("IsFire", true);

                // 발사 사운드 재생.
                audioSource.PlayOneShot(fireSoundClip);


                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, bulletMaxDistance, HitTarget))
                {
                    CreateBullet(hit.point);
                }
                else
                {
                    CreateBullet(ray.GetPoint(bulletMaxDistance));
                }


                // 발사 후 쿨타임을 초기화합니다.
                elapsedTime = 0f;
            }
            // 마우스 왼쪽 버튼 떼기 시
            else if (Input.GetMouseButtonUp(0))
            {
                // 애니메이터 파라미터 비활성화
                animator.SetBool("IsFire", false);

                // 발사 후 쿨타임을 초기화합니다.
                elapsedTime = 0f;
            }
            // 발사 쿨타임이 적용되고 있으면 elapsedTime을 증가시킵니다.
            if (elapsedTime < shotDelay)
            {
                elapsedTime += Time.deltaTime;
            }


            void CreateBullet(Vector3 targetPosition)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                Vector3 direction = (targetPosition - WeaponMuzzle.transform.position).normalized;
                bullet.transform.position = WeaponMuzzle.transform.position;

                bullet.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
