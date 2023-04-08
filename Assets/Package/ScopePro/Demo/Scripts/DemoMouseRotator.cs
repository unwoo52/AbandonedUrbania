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
        }

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
        private void Update()
        {
            Vector2 output = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y")) * SensitivityMouseAim; //vector2(Input.GetAxis("camera X"),Input.GetAxis("Camera Y"))
            Vector3 euler = transform.eulerAngles;

            /*
            euler.y = -Mathf.Lerp(HorizontalMinMax.x, HorizontalMinMax.y, output.y);
            euler.x = Mathf.Lerp(VerticalMinMax.x, VerticalMinMax.y, output.x);
            euler.y += 180;
            */

            //testA -= output.y;
            //testA = Mathf.Clamp(testA + output.y, VerticalMinMax.x, VerticalMinMax.y);
            testA = Mathf.Clamp(testA + output.y, -MinMax, MinMax);
            //testB += output.x;
            //testB = Mathf.Clamp(testB + output.x, HorizontalMinMax.x, HorizontalMinMax.y);
            testB = Mathf.Clamp(testB + output.x, -MinMax, MinMax);

            euler.x = -testA;
            euler.y = testB;

            Debug.Log(euler + "::::" + output);

            RotCenter.transform.eulerAngles = defaultRot + euler * CenterAngle;
            m_Camera.transform.eulerAngles = (camOriginRot + euler) * CameraAngle;

            Vector3 newver = new Vector3(euler.y, -euler.x, 0);
            Debug.Log(newver);
            RotCenter.transform.position = (CenterOriginPos + newver * CenterPos);
        }
    }
}