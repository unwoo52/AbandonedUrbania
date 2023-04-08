using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private LayerMask canHit;
        [SerializeField] GameObject Effect;


        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, speed * Time.deltaTime, canHit))
            {
                Hit(hitInfo.transform.gameObject, hitInfo.point);
            }
        }


        void Hit(GameObject gameObject, Vector3 vetor3)
        {
            Instantiate(Effect, vetor3, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }

}
