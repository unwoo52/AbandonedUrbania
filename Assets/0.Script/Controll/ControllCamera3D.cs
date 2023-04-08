using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Urban_KimHyeonWoo
{
    public class ControllCamera3D : MonoBehaviour
    {
        Vector2 OriginZoomMinMax;
        [SerializeField] Vector2 CloseZoomMinMax;
        Camera3D cam;
        bool isCloseViewState = false;
        Vector3 OriginViewOffsetValue;
        [SerializeField] Vector3 CloseViewOffsetValue;
        [SerializeField] Animator animator;
        [SerializeField] GameObject Weapon;
        [SerializeField] GameObject HandHip;
        [SerializeField] GameObject HudeHip;
        private void Start()
        {
            if (cam == null)
            {
                TryGetComponent(out Camera3D camera);
                cam = camera;
            }

            OriginViewOffsetValue = cam.OffsetFromHead;
            OriginZoomMinMax = new Vector2(cam.minZoom, cam.maxZoom);

            UnCloseToPlayer();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ChangeView();
            }
        }

        void ChangeView()
        {
            if (cam != null)
            {
                TryGetComponent(out Camera3D camera);
                cam = camera;
            }

            if (isCloseViewState)
            {
                UnCloseToPlayer();
            }
            else
            {
                CloseToPlayer();
            }
        }

        void CloseToPlayer()
        {
            cam.OffsetFromHead = CloseViewOffsetValue;
            cam.minZoom = CloseZoomMinMax.x;
            cam.maxZoom = CloseZoomMinMax.y;
            isCloseViewState = true;
            animator.SetLayerWeight(1, 0.33f);
            animator.SetFloat("State_Wepon", 1);
            Weapon.transform.SetParent(HandHip.transform, false);
            Weapon.transform.localPosition = Vector3.zero;
            Weapon.transform.localRotation = Quaternion.identity;
        }

        void UnCloseToPlayer()
        {
            cam.OffsetFromHead = OriginViewOffsetValue;
            cam.minZoom = OriginZoomMinMax.x;
            cam.maxZoom = OriginZoomMinMax.y;
            isCloseViewState = false;
            animator.SetLayerWeight(1, 0);
            animator.SetFloat("State_Wepon", 0);
            Weapon.transform.SetParent(HudeHip.transform, false);
            Weapon.transform.localPosition = Vector3.zero;
            Weapon.transform.localRotation = Quaternion.identity;
        }
    }
}

