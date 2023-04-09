using Lightbug.CharacterControllerPro.Implementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

namespace Lovatto.Demo.ScopePro
{
    public class DemoMouseRotator : MonoBehaviour
    {
        /// <summary>
        /// scope focus : 스코프의 로테이션을 입체적으로 만들기 위한 부모오브젝트 원점.
        /// 스코프와 가까울수록 스코프는 제자리에서 회전하고,
        /// 스코프와 멀수록 스코프가 화면안에서 많이 움직입니다.
        /// </summary>
        [Tooltip("스코프 회전 원점. 이 오브젝트를 기준으로 스코프가 회전")]
        public GameObject ScopeFocus;
        [Tooltip("스코프를 바라볼 카메라")]
        public Camera m_Camera;
        public Camera m_CameraFocus;
        [Tooltip("스코프 렌즈 메테리얼")]
        public Material ZoomMaterial;

        //default size
        Vector3 defaultRot;
        Vector3 camOriginRot;
        Vector3 CenterOriginPos;

        float zoomValue = 0.5f; // 0 ~ 1
        Vector2 ZoomMinMax = new Vector2(0, 1);


        private void Awake()
        {
            defaultRot = transform.eulerAngles;
            camOriginRot = m_Camera.transform.eulerAngles;
            CenterOriginPos = ScopeFocus.transform.position;
        }

        private void Start()
        {
            TESTmakeMouseUnvisible();
        }
        void TESTmakeMouseUnvisible()
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        [Header("스코프 움직임 조절")]
        [Tooltip("값이 클수록 마우스를 움직일 때 카메라가 크게 회전합니다.")]
        public float CameraAngle;
        [Tooltip("값이 클수록 마우스를 움직일 때 스코프가 크게 회전합니다.")]
        public float CenterAngle;
        [Tooltip("값이 클수록 화면 중앙으로부터 스코프가 멀어집니다")]
        public float CenterPos;
        [Tooltip("에임의 상하좌우 한계치입니다.")]
        public float MinMax;


        [Header("스코프 렌즈 줌값 조절")]
        [Tooltip("도트사이트 크기 최소 최대 값")]
        public Vector2 DotsiteMinMax = new Vector2(0.5f, 1.5f);
        [Tooltip("스포크 렌즈 확대 최소 최대 값")]
        public Vector2 ZoomAreaMinMax = new Vector2(3f, 15f);
        [Tooltip("카메라 확대 최소 최대 값")]
        public Vector2 CamFieldofView = new Vector2(21f, 29f);

        [Header("Zoom Value")]
        public AnimationCurve aimCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("마우스휠을 굴릴 때 줌이 더 빨리 커지고 줄어듭니다.")]
        [Range(0,1)]
        public float ZoomWheelSpeed = 0.5f;
        [Tooltip("줌 거리에 따른 줌 스피드를 조절하는 곡선입니다.")]
        public AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


        [Header("모니터링")]
        [Tooltip("줌 속도입니다.")]
        public float zoomSpeed;
        [Tooltip("에임의 감도입니다.")]
        public float SensitivityMouseAim;


        //default mouse input field
        public float HorizonMouseInput = 0;
        public float VerticalMouseInput = 0;
        private void Update()
        {
            //get wheelup value
            zoomSpeed = zoomCurve.Evaluate(zoomValue);
            float v = Input.GetAxis("Camera Zoom") * zoomSpeed * 0.02f * ZoomWheelSpeed;
            zoomValue = Mathf.Clamp(zoomValue - v, ZoomMinMax.x, ZoomMinMax.y);


            float k = Mathf.Lerp(DotsiteMinMax.x, DotsiteMinMax.y, zoomValue);
            float m = Mathf.Lerp(ZoomAreaMinMax.x, ZoomAreaMinMax.y, zoomValue);
            //add Dot Sight
            ZoomMaterial.SetFloat("Vector1_0bb2c494708d4e73aed6ec3922b741ac", k);
            //add Scope Zoom size
            m_CameraFocus.fieldOfView = m;

            //add Camera Field of View
            m_Camera.fieldOfView = Mathf.Lerp(CamFieldofView.x, CamFieldofView.y, zoomValue);

            //modify aim speed
            SensitivityMouseAim = aimCurve.Evaluate(zoomValue);


            //add camPos camAngle scopeAngle
            Vector2 output = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y")) * SensitivityMouseAim;
            Vector3 euler = transform.eulerAngles;

            HorizonMouseInput = Mathf.Clamp(HorizonMouseInput + output.y, -MinMax, MinMax);
            VerticalMouseInput = Mathf.Clamp(VerticalMouseInput + output.x, -MinMax, MinMax);

            euler.x = -HorizonMouseInput;
            euler.y = VerticalMouseInput;

            //add Scope Angle
            ScopeFocus.transform.eulerAngles = defaultRot + euler * CenterAngle;
            //add Cam Angle
            m_Camera.transform.eulerAngles = (camOriginRot + euler) * CameraAngle;

            //add Cam Transform
            Vector3 newver = new Vector3(euler.y, -euler.x, 0);
            ScopeFocus.transform.position = (CenterOriginPos + newver * CenterPos);
        }
    }
}