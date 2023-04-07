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
        private void Start()
        {
            if (cam == null)
            {
                TryGetComponent(out Camera3D camera);
                cam = camera;
            }

            OriginViewOffsetValue = cam.OffsetFromHead;
            OriginZoomMinMax = new Vector2(cam.minZoom, cam.maxZoom);
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
                cam.OffsetFromHead = OriginViewOffsetValue;
                cam.minZoom = OriginZoomMinMax.x;
                cam.maxZoom = OriginZoomMinMax.y;
                isCloseViewState = false;
            }
            else
            {
                cam.OffsetFromHead = CloseViewOffsetValue;
                cam.minZoom = CloseZoomMinMax.x;
                cam.maxZoom = CloseZoomMinMax.y;
                isCloseViewState = true;
            }
        }
    }
}

