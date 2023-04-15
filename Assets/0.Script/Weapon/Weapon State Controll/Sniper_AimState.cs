using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class Sniper_AimState : WeaponState
    {
        //스코프 필드
        [SerializeField] GameObject ScopeObject;
        [Tooltip("스코프 회전 원점. 이 오브젝트를 기준으로 스코프가 회전")]
        [SerializeField] GameObject CenterObject;

        [SerializeField] Camera RensCam;
        [Tooltip("스코프 렌즈 메테리얼")]
        [SerializeField] Material DotSight_Mtaterial;


        //default size
        Vector3 defaultRot;
        Vector3 defaultCenterPos;

        [Header("스코프 움직임 조절")]
        [Tooltip("값이 클수록 마우스를 움직일 때 카메라가 크게 회전합니다.")]
        [SerializeField] float CameraAngle = 0.1f;
        [Tooltip("값이 클수록 마우스를 움직일 때 스코프가 크게 회전합니다.")]
        [SerializeField] float CenterAngle = 0.5f;
        [Tooltip("값이 클수록 화면 중앙으로부터 스코프가 멀어집니다")]
        [SerializeField] float CenterPos = 0.1f;
        [Tooltip("에임의 상하좌우 한계치입니다.")]
        [SerializeField] float MinMax;


        [Header("스코프 렌즈 줌값 조절")]
        [Tooltip("도트사이트 크기 최소 최대 값")]
        [SerializeField] Vector2 DotsiteMinMax = new Vector2(0.5f, 1.5f);
        [Tooltip("스포크 렌즈 확대 최소 최대 값")]
        [SerializeField] Vector2 ZoomAreaMinMax = new Vector2(3f, 15f);
        [Tooltip("카메라 확대 최소 최대 값")]
        [SerializeField] Vector2 CamFieldofView = new Vector2(21f, 29f);

        [Header("Zoom Value")]
        [SerializeField] AnimationCurve aimCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("마우스휠로 줌 하는 속도를 조절합니다..")]
        [Range(0, 1)]
        [SerializeField] float ZoomWheelSpeed = 0.5f;
        [Tooltip("줌 거리에 따른 줌 스피드를 조절하는 곡선입니다.")]
        [SerializeField] AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


        [Header("모니터링")]
        [Tooltip("모니터링 :: 줌 속도의 커브 상태입니다.")]
        [SerializeField] float CurvedzoomSpeed;

        [Tooltip("모니터링 :: 에임의 감도입니다.")]
        [SerializeField] float SensitivityMouseAim;

        [Tooltip("모니터링 :: 줌 수치입니다.")]
        [SerializeField] float zoomForce = 0.5f; // 0 ~ 1
        Vector2 ZoomMinMax = new Vector2(0, 1);

        //default mouse input field
        float HorizonMouseInput = 0;
        float VerticalMouseInput = 0;

        private void Start()
        {
            WeaponStateController.Cam = transform.parent.parent.GetChild(0).GetComponent<Camera>();
        }

        public float MouseWheel;
        public bool Fire2;
        private void Update()
        {
            MouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
            Fire2 = Input.GetButtonDown("Fire2");
        }
        //======================================
        //======================================

        public override void CheckExitTransition()
        {
            if (Fire2 == true)
            {
                WeaponStateController.EnqueueTransition<Sniper_CloseView>();
            }
        }
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            WeaponStateController.Camera3D.OffsetFromHead = Vector3.zero;
            //기존에 awake에 있던 코드들
            defaultRot = transform.localEulerAngles;
            defaultCenterPos = CenterObject.transform.localPosition;
            //===

            WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.FirstPerson;

            HorizonMouseInput = 0;
            VerticalMouseInput = 0;
            zoomForce = 0.5f;

            ScopeObject.SetActive(true);
            ShakingHand = StartCoroutine(cor());
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;

            ScopeObject.SetActive(false);
            StopCoroutine(ShakingHand);
        }

        [SerializeField] Camera NewSightCam;
        public override void UpdateBehaviour(float dt)
        {
            float MouseWheel = Input.GetAxis("Camera Zoom");
            float MousX = Input.GetAxis("Camera X");
            float MousY = Input.GetAxis("Camera Y");
            Debug.Log($"{MousX} ::: {MousY}");

            //calculate Zoom Force  -------
            CurvedzoomSpeed = zoomCurve.Evaluate(zoomForce);
            float wheelInput = MouseWheel * CurvedzoomSpeed * 0.02f * ZoomWheelSpeed;
            //Zoom Force 0 ~ 1
            zoomForce = Mathf.Clamp(zoomForce - wheelInput, ZoomMinMax.x, ZoomMinMax.y);


            //set Size -dot sight, scopeRens filedOfView  ----------
            float dot_size = Mathf.Lerp(DotsiteMinMax.x, DotsiteMinMax.y, zoomForce);
            float rensCam_FieldofView = Mathf.Lerp(ZoomAreaMinMax.x, ZoomAreaMinMax.y, zoomForce);
            //add Dot Sight size
            DotSight_Mtaterial.SetFloat("Vector1_0bb2c494708d4e73aed6ec3922b741ac", dot_size);
            //add Scope Rense Zoom size
            RensCam.fieldOfView = rensCam_FieldofView;
            //add Player view Camera Field of View
            WeaponStateController.Cam.fieldOfView = Mathf.Lerp(CamFieldofView.x, CamFieldofView.y, zoomForce);

            //modify aim speed
            SensitivityMouseAim = aimCurve.Evaluate(zoomForce);



            //add Cam Angle :: 플레이어의 눈 카메라 rotate 회전
            //WeaponStateController.Camera3D.deltaPitch += -MousY * 0.01f * SensitivityMouseAim * CameraAngle;
            //WeaponStateController.Camera3D.deltaYaw += MousX * 0.01f * SensitivityMouseAim * CameraAngle;
            

            //add camPos camAngle scopeAngle
            HorizonMouseInput = Mathf.Clamp(HorizonMouseInput + MousY * SensitivityMouseAim, -MinMax, MinMax);
            VerticalMouseInput = Mathf.Clamp(VerticalMouseInput + MousX * SensitivityMouseAim, -MinMax, MinMax);
            Vector3 euler = new Vector3(HorizonMouseInput, VerticalMouseInput, 0);

            //스코프 중앙을 기준으로 한 회전. " |>=<| " 왼쪽이 스코프라 할 때, =부분의 중앙 을 중심으로 스코프의 회전
           // CenterObject.transform.localEulerAngles = defaultRot + euler * CenterAngle + shakingHands;
            CenterObject.transform.localEulerAngles = defaultRot + euler * CenterAngle;

            //CenterPos의 원점을 기준으로 한 스코프 전체의 회전. " |>=<|  · "  왼쪽의 그림에서·가 원점.
            NewSightCam.transform.localPosition = defaultCenterPos + new Vector3(euler.y, -euler.x, 0) * 0.01f * CenterPos + testffffffff;
        }
        public Vector3 testffffffff;
        public float testEuler = 3;


        [Header("손떨림 필드")]
        private Vector3 shakingHands;
        [Tooltip("손떨림 강도")]
        [SerializeField] float shakingValue = 0.001f;
        [Tooltip("손떨리는 속도")]
        [SerializeField] float shakingSpeed = 0.05f;
        Coroutine ShakingHand;
        IEnumerator cor()
        {
            while (true)
            {
                Vector3 target = new Vector3(Random.Range(-shakingValue, shakingValue), Random.Range(-shakingValue, shakingValue), shakingHands.z);
                float distance = Vector3.Distance(shakingHands, target);
                float duration = distance / shakingSpeed;

                float t = 0;
                while (t < duration)
                {
                    shakingHands = Vector3.Lerp(shakingHands, target, t / duration);
                    t += Time.deltaTime;
                    yield return null;
                }

                shakingHands = target;
            }
        }
    }


}