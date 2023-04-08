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

            // ���콺 ���� ��ư Ŭ�� ��
            if (Input.GetMouseButtonDown(0))
            {
                // ��Ÿ���� ������ �ʾ����� �Լ��� �����մϴ�.
                if (elapsedTime < shotDelay)
                {
                    return;
                }

                // �ִϸ����� �Ķ���� Ȱ��ȭ
                animator.SetBool("IsFire", true);

                // �߻� ���� ���.
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


                // �߻� �� ��Ÿ���� �ʱ�ȭ�մϴ�.
                elapsedTime = 0f;
            }
            // ���콺 ���� ��ư ���� ��
            else if (Input.GetMouseButtonUp(0))
            {
                // �ִϸ����� �Ķ���� ��Ȱ��ȭ
                animator.SetBool("IsFire", false);

                // �߻� �� ��Ÿ���� �ʱ�ȭ�մϴ�.
                elapsedTime = 0f;
            }
            // �߻� ��Ÿ���� ����ǰ� ������ elapsedTime�� ������ŵ�ϴ�.
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
