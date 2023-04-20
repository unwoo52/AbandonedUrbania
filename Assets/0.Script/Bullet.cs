using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public interface IDamageSystem
    {
        void OnDam(float dmg, GameObject projectileOwner);
    }
    public interface IProjectileOwner
    {
        void GetProjectileOwner(GameObject projectileOwner);
    }
    public class Bullet : MonoBehaviour, IProjectileOwner
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private LayerMask canHit;
        [SerializeField] GameObject Effect;
        [SerializeField] float Damage;
        [SerializeField] float DmgRadius = 1;
        GameObject projectileOwner;


        private void Update()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, speed * Time.deltaTime, canHit))
            {
                Hit(hitInfo.transform.gameObject, hitInfo.point);
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }


        void Hit(GameObject gameObject, Vector3 vetor3)
        {
            Instantiate(Effect, vetor3, Quaternion.identity);

            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, DmgRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out IDamageSystem testDamageSystem))
                {
                    testDamageSystem.OnDam(Damage, projectileOwner);
                }
            }


            
            Destroy(this.gameObject);
        }

        public void GetProjectileOwner(GameObject projectileOwner)
        {
            this.projectileOwner = projectileOwner;
        }
    }

}
