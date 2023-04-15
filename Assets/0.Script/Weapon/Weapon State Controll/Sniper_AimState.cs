using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class Sniper_AimState : WeaponState
    {
        //������ �ʵ�
        [SerializeField] GameObject ScopeObject;
        [Tooltip("������ ȸ�� ����. �� ������Ʈ�� �������� �������� ȸ��")]
        [SerializeField] GameObject CenterObject;

        [SerializeField] Camera RensCam;
        [Tooltip("������ ���� ���׸���")]
        [SerializeField] Material DotSight_Mtaterial;


        //default size
        Vector3 defaultRot;
        Vector3 defaultCenterPos;

        [Header("������ ������ ����")]
        [Tooltip("���� Ŭ���� ���콺�� ������ �� ī�޶� ũ�� ȸ���մϴ�.")]
        [SerializeField] float CameraAngle = 0.1f;
        [Tooltip("���� Ŭ���� ���콺�� ������ �� �������� ũ�� ȸ���մϴ�.")]
        [SerializeField] float CenterAngle = 0.5f;
        [Tooltip("���� Ŭ���� ȭ�� �߾����κ��� �������� �־����ϴ�")]
        [SerializeField] float CenterPos = 0.1f;
        [Tooltip("������ �����¿� �Ѱ�ġ�Դϴ�.")]
        [SerializeField] float MinMax;


        [Header("������ ���� �ܰ� ����")]
        [Tooltip("��Ʈ����Ʈ ũ�� �ּ� �ִ� ��")]
        [SerializeField] Vector2 DotsiteMinMax = new Vector2(0.5f, 1.5f);
        [Tooltip("����ũ ���� Ȯ�� �ּ� �ִ� ��")]
        [SerializeField] Vector2 ZoomAreaMinMax = new Vector2(3f, 15f);
        [Tooltip("ī�޶� Ȯ�� �ּ� �ִ� ��")]
        [SerializeField] Vector2 CamFieldofView = new Vector2(21f, 29f);

        [Header("Zoom Value")]
        [SerializeField] AnimationCurve aimCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("���콺�ٷ� �� �ϴ� �ӵ��� �����մϴ�..")]
        [Range(0, 1)]
        [SerializeField] float ZoomWheelSpeed = 0.5f;
        [Tooltip("�� �Ÿ��� ���� �� ���ǵ带 �����ϴ� ��Դϴ�.")]
        [SerializeField] AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


        [Header("����͸�")]
        [Tooltip("����͸� :: �� �ӵ��� Ŀ�� �����Դϴ�.")]
        [SerializeField] float CurvedzoomSpeed;

        [Tooltip("����͸� :: ������ �����Դϴ�.")]
        [SerializeField] float SensitivityMouseAim;

        [Tooltip("����͸� :: �� ��ġ�Դϴ�.")]
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
            //������ awake�� �ִ� �ڵ��
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



            //add Cam Angle :: �÷��̾��� �� ī�޶� rotate ȸ��
            //WeaponStateController.Camera3D.deltaPitch += -MousY * 0.01f * SensitivityMouseAim * CameraAngle;
            //WeaponStateController.Camera3D.deltaYaw += MousX * 0.01f * SensitivityMouseAim * CameraAngle;
            

            //add camPos camAngle scopeAngle
            HorizonMouseInput = Mathf.Clamp(HorizonMouseInput + MousY * SensitivityMouseAim, -MinMax, MinMax);
            VerticalMouseInput = Mathf.Clamp(VerticalMouseInput + MousX * SensitivityMouseAim, -MinMax, MinMax);
            Vector3 euler = new Vector3(HorizonMouseInput, VerticalMouseInput, 0);

            //������ �߾��� �������� �� ȸ��. " |>=<| " ������ �������� �� ��, =�κ��� �߾� �� �߽����� �������� ȸ��
           // CenterObject.transform.localEulerAngles = defaultRot + euler * CenterAngle + shakingHands;
            CenterObject.transform.localEulerAngles = defaultRot + euler * CenterAngle;

            //CenterPos�� ������ �������� �� ������ ��ü�� ȸ��. " |>=<|  �� "  ������ �׸��������� ����.
            NewSightCam.transform.localPosition = defaultCenterPos + new Vector3(euler.y, -euler.x, 0) * 0.01f * CenterPos + testffffffff;
        }
        public Vector3 testffffffff;
        public float testEuler = 3;


        [Header("�ն��� �ʵ�")]
        private Vector3 shakingHands;
        [Tooltip("�ն��� ����")]
        [SerializeField] float shakingValue = 0.001f;
        [Tooltip("�ն����� �ӵ�")]
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