using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public interface IDamageSystem
    {
        void OnDam(float dmg);
    }
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private LayerMask canHit;
        [SerializeField] GameObject Effect;
        [SerializeField] float Damage;


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
            if (gameObject.TryGetComponent(out IDamageSystem testDamageSystem))
            {
                testDamageSystem.OnDam(Damage);
            }
            Destroy(this.gameObject);
        }

    }

}
