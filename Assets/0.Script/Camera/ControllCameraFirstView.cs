using Lightbug.CharacterControllerPro.Demo;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class ControllCameraFirstView : MonoBehaviour
    {
        //������ �ʵ�
        [SerializeField] GameObject ScopeObject;
        [Tooltip("������ ȸ�� ����. �� ������Ʈ�� �������� �������� ȸ��")]
        public GameObject ScopeFocus;
        [Tooltip("�������� �ٶ� ī�޶�")]
        Camera cam;

        public Camera RensCam;
        [Tooltip("������ ���� ���׸���")]
        public Material ZoomMaterial;


        //default size
        Vector3 defaultRot;
        Vector3 defaultcamRot;
        Vector3 defaultCenterPos;




        [Header("������ ������ ����")]
        [Tooltip("���� Ŭ���� ���콺�� ������ �� ī�޶� ũ�� ȸ���մϴ�.")]
        public float CameraAngle;
        [Tooltip("���� Ŭ���� ���콺�� ������ �� �������� ũ�� ȸ���մϴ�.")]
        public float CenterAngle;
        [Tooltip("���� Ŭ���� ȭ�� �߾����κ��� �������� �־����ϴ�")]
        public float CenterPos;
        [Tooltip("������ �����¿� �Ѱ�ġ�Դϴ�.")]
        public float MinMax;


        [Header("������ ���� �ܰ� ����")]
        [Tooltip("��Ʈ����Ʈ ũ�� �ּ� �ִ� ��")]
        public Vector2 DotsiteMinMax = new Vector2(0.5f, 1.5f);
        [Tooltip("����ũ ���� Ȯ�� �ּ� �ִ� ��")]
        public Vector2 ZoomAreaMinMax = new Vector2(3f, 15f);
        [Tooltip("ī�޶� Ȯ�� �ּ� �ִ� ��")]
        public Vector2 CamFieldofView = new Vector2(21f, 29f);

        [Header("Zoom Value")]
        public AnimationCurve aimCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("���콺�ٷ� �� �ϴ� �ӵ��� �����մϴ�..")]
        [Range(0, 1)]
        public float ZoomWheelSpeed = 0.5f;
        [Tooltip("�� �Ÿ��� ���� �� ���ǵ带 �����ϴ� ��Դϴ�.")]
        public AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


        [Header("����͸�")]
        [Tooltip("����͸� :: �� �ӵ��� Ŀ�� �����Դϴ�.")]
        public float CurvedzoomSpeed;

        [Tooltip("����͸� :: ������ �����Դϴ�.")]
        public float SensitivityMouseAim;

        [Tooltip("����͸� :: �� ��ġ�Դϴ�.")]
        [SerializeField] float zoomForce = 0.5f; // 0 ~ 1
        Vector2 ZoomMinMax = new Vector2(0, 1);

        //default mouse input field
        public float HorizonMouseInput = 0;
        public float VerticalMouseInput = 0;

        bool isAimState = false;

        //======================================
        //======================================


        #region Unity Callbacks and OnEnable Method
        private void Awake()
        {
            cam = GetComponent<Camera>();
            defaultRot = transform.localEulerAngles;
            defaultcamRot = cam.transform.localEulerAngles;
            defaultCenterPos = ScopeFocus.transform.localPosition;
        }

        private void Update()
        {
            GetInput();
            if(isAimState)
                Scope(MousX, MousY, MouseWheel);
        }

        #endregion


        #region Input Process

        //Key InPut Field
        [SerializeField] float MousX;
        [SerializeField] float MousY;
        [SerializeField] float MouseWheel;
        void GetInput()
        {
            MouseWheel = Input.GetAxis("Camera Zoom");
            MousX = Input.GetAxis("Camera X");
            MousY = Input.GetAxis("Camera Y");
            Debug.Log($"{MousX} ::: {MousY}");
        }
        #endregion

        #region public
        float originFieldOfView;
        Quaternion originRot;
        [SerializeField] Vector2 CamRotateMinMax = new Vector2(-30f, 30f); // ī�޶� ȸ�� �ּ�, �ִ밪 (x: min, y: max)
        [SerializeField] Vector2 curCamMinMax = new Vector2(0, 0);
        public void EnterAim()
        {
            if (TryGetComponent(out Camera3D camera3D))
            {
                originRot = transform.rotation;
            }
            HorizonMouseInput = 0;
            VerticalMouseInput = 0;
            zoomForce = 0.5f;
            originFieldOfView = cam.fieldOfView;
            isAimState = true;
            ScopeObject.SetActive(true);
        }

        public void ExitAim()
        {
            cam.fieldOfView = originFieldOfView;
            isAimState = false;
            ScopeObject.SetActive(false);
        }
        #endregion

        #region Scope Movement
        public float testff = 0.001f;
        void Scope(float mousex, float mousey, float mouseWheel)
        {
            //calculate Zoom Force  -------
            CurvedzoomSpeed = zoomCurve.Evaluate(zoomForce);
            float wheelInput = mouseWheel * CurvedzoomSpeed * 0.02f * ZoomWheelSpeed;
            //Zoom Force 0 ~ 1
            zoomForce = Mathf.Clamp(zoomForce - wheelInput, ZoomMinMax.x, ZoomMinMax.y);


            //set Size -dot sight, scopeRens filedOfView  ----------
            float k = Mathf.Lerp(DotsiteMinMax.x, DotsiteMinMax.y, zoomForce);
            float m = Mathf.Lerp(ZoomAreaMinMax.x, ZoomAreaMinMax.y, zoomForce);
            //add Dot Sight size
            ZoomMaterial.SetFloat("Vector1_0bb2c494708d4e73aed6ec3922b741ac", k);
            //add Scope Rense Zoom size
            RensCam.fieldOfView = m;
            //add Player view Camera Field of View
            cam.fieldOfView = Mathf.Lerp(CamFieldofView.x, CamFieldofView.y, zoomForce);

            //modify aim speed
            SensitivityMouseAim = aimCurve.Evaluate(zoomForce);


            //add camPos camAngle scopeAngle
            HorizonMouseInput = Mathf.Clamp(HorizonMouseInput + mousey * SensitivityMouseAim, -MinMax, MinMax);
            VerticalMouseInput = Mathf.Clamp(VerticalMouseInput + mousex * SensitivityMouseAim, -MinMax, MinMax);

            Vector3 euler = new Vector3(-HorizonMouseInput, VerticalMouseInput, 0);

            //add Scope Angle
            ScopeFocus.transform.localEulerAngles = defaultRot + euler * CenterAngle;

            //add Cam Angle
            if (TryGetComponent(out Camera3D camera3D))
            {
                camera3D.deltaPitch += -mousey * testff * SensitivityMouseAim;
                camera3D.deltaYaw += mousex * testff * SensitivityMouseAim;
            }
            
            //add Cam Transform
            Vector3 newver = new Vector3(euler.y, -euler.x, 0);
            ScopeFocus.transform.localPosition = (defaultCenterPos + newver * CenterPos);
        }
        #endregion
    }
}