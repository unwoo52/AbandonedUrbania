using Lightbug.CharacterControllerPro.Implementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

namespace Lovatto.Demo.ScopePro
{
    public class DemoMouseRotator : MonoBehaviour
    {
        
        public Vector2 VerticalMinMax;
        public Vector2 HorizontalMinMax;
        public GameObject RotCenter;
        public Camera m_Camera;
        private Vector3 defaultRot;
        private Vector3 camOriginRot;
        private Vector3 CenterOriginPos;
        [Range(-5f, 5f)]
        public float testA = 0;
        [Range(-5f, 5f)]
        public float testB = 0;

        private void Awake()
        {
            defaultRot = transform.eulerAngles;
            camOriginRot = m_Camera.transform.eulerAngles;
            CenterOriginPos = RotCenter.transform.position;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            dotsiteSize = ZoomMaterial.GetFloat("Vector1_0bb2c494708d4e73aed6ec3922b741ac");
            ZoomSize = ZoomMaterial.GetFloat("_RenderSize");


        }

        [Header("스코프 움직임")]
        [Tooltip("값이 클수록 마우스를 움직일 때 카메라가 크게 회전합니다.")]
        public float CameraAngle;
        [Tooltip("값이 클수록 마우스를 움직일 때 스코프가 크게 회전합니다.")]
        public float CenterAngle;
        [Tooltip("값이 클수록 화면 중앙으로부터 스코프가 멀어집니다")]
        public float CenterPos;
        [Tooltip("에임의 상하좌우 한계치입니다.")]
        public float MinMax;
        [Tooltip("에임의 감도입니다.")]
        public float SensitivityMouseAim;


        [Header("스코프 줌")]
        public Material ZoomMaterial;
        public float dotsiteSize;
        public float ZoomSize;
        public float zoomdefault = 0.5f;
        public Vector2 DotsiteMinMax;
        public Vector2 ZoomAreaMinMax;
        public Vector2 ZoomMinMax;


        [Tooltip("값이 클수록 마우스휠을 굴릴 때 줌이 더 빨리 커지고 줄어듭니다.")]
        public float ZoomSpeed;
        private void Update()
        {
                //add zoom
            float zoomvalue = Input.GetAxis("Camera Zoom") * ZoomSpeed;
            zoomdefault -= zoomvalue;
            zoomdefault = Mathf.Clamp(zoomdefault + zoomvalue, ZoomMinMax.x, ZoomMinMax.y);


            dotsiteSize = Mathf.Clamp(dotsiteSize + zoomvalue, -DotsiteMinMax.x, DotsiteMinMax.y);
            ZoomSize = Mathf.Clamp(ZoomSize + zoomvalue, -ZoomAreaMinMax.x, ZoomAreaMinMax.y);

            float k = Mathf.Lerp(DotsiteMinMax.x, DotsiteMinMax.y, zoomdefault);
            float m = Mathf.Lerp(ZoomAreaMinMax.x, ZoomAreaMinMax.y, zoomdefault);
            //add Dot Sight
            ZoomMaterial.SetFloat("Vector1_0bb2c494708d4e73aed6ec3922b741ac", k);
            //add Scope Zoom size
            ZoomMaterial.SetFloat("_RenderSize", m);


                //add camPos camAngle scopeAngle
            Vector2 output = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y")) * SensitivityMouseAim;
            Vector3 euler = transform.eulerAngles;

            testA = Mathf.Clamp(testA + output.y, -MinMax, MinMax);
            testB = Mathf.Clamp(testB + output.x, -MinMax, MinMax);

            euler.x = -testA;
            euler.y = testB;

            //add Scope Angle
            RotCenter.transform.eulerAngles = defaultRot + euler * CenterAngle;
            //add Cam Angle
            m_Camera.transform.eulerAngles = (camOriginRot + euler) * CameraAngle;

            //add Cam Transform
            Vector3 newver = new Vector3(euler.y, -euler.x, 0);
            RotCenter.transform.position = (CenterOriginPos + newver * CenterPos);
        }
    }
}