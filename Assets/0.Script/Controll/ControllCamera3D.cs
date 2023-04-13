using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace Urban_KimHyeonWoo
{
    public interface IBindPlayerCam
    {
        void BindPlayerCam_To_Object(bool IsBind, GameObject interactObject, Vector2 ZoomMinMax);
    }
    public interface IGetRayAtCamera
    {
        bool GetRayAtCamera(out Ray ray);
    }
    public class ControllCamera3D : MonoBehaviour, IBindPlayerCam, IGetRayAtCamera
    {

        Vector2 OriginZoomMinMax;
        [SerializeField] Vector2 CloseZoomMinMax;
        Camera3D cam;
        Vector3 OriginViewOffsetValue;
        [SerializeField] Vector3 CloseViewOffsetValue;
        [SerializeField] Animator animator;
        [SerializeField] GameObject Weapon;
        [SerializeField] GameObject HandHip;
        [SerializeField] GameObject HudeHip;
        [SerializeField] GameObject Sniper;
        [SerializeField] GameObject Player;
        enum CameraViewState
        {
            ThirdPersonView_Far, ThirdPersonView_Close, AmingView, OtherObjectView
        }
        CameraViewState currentViewState = CameraViewState.ThirdPersonView_Far;
        CameraViewState beforeState = default;
        void ChangeViewState(CameraViewState viewState)
        {
            if(currentViewState == viewState) return;
            CameraViewState tempBeforeState = currentViewState;

            //exit methods
            switch (currentViewState)
            {
                case CameraViewState.ThirdPersonView_Far:
                    break;
                case CameraViewState.ThirdPersonView_Close:
                    break;
                case CameraViewState.AmingView:
                    cam.cameraMode = Camera3D.CameraMode.ThirdPerson;
                    Sniper.SetActive(false);
                    break;
                case CameraViewState.OtherObjectView:
                    break;
            }

            currentViewState = viewState;


            //enter methods
            switch (currentViewState)
            {
                case CameraViewState.ThirdPersonView_Far:
                    cam.cameraMode = Camera3D.CameraMode.ThirdPerson;
                    SetCloseToTarget(OriginViewOffsetValue, OriginZoomMinMax);
                    SetPlayerAnimAndWeaponPos(false);
                    break;
                case CameraViewState.ThirdPersonView_Close:
                    cam.cameraMode = Camera3D.CameraMode.ThirdPerson;
                    SetCloseToTarget(CloseViewOffsetValue, CloseZoomMinMax);
                    SetPlayerAnimAndWeaponPos(true);
                    break;
                case CameraViewState.AmingView:
                    cam.cameraMode = Camera3D.CameraMode.FirstPerson;
                    Sniper.SetActive(true);
                    break;
                case CameraViewState.OtherObjectView:
                    beforeState = tempBeforeState;
                    cam.cameraMode = Camera3D.CameraMode.ThirdPerson;
                    SetPlayerAnimAndWeaponPos(false);
                    break;
            }
        }

        private void Start()
        {
            if (cam == null)
            {
                TryGetComponent(out Camera3D camera);
                cam = camera;
            }

            OriginViewOffsetValue = cam.OffsetFromHead;
            OriginZoomMinMax = new Vector2(cam.minZoom, cam.maxZoom);

            SetCloseToTarget( OriginViewOffsetValue, OriginZoomMinMax);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void Update()
        {
            GetInput();
            InputProcsee();
        }

        //Input Fields
        bool AmingKey = false;
        float SwapViewKey;

        void GetInput()
        {
            AmingKey = Input.GetButtonDown("Fire2");            
            SwapViewKey = Mathf.Clamp(Input.GetAxisRaw("Camera Zoom"), -1f, 1f);
        }
        void InputProcsee()
        {
            if (AmingKey) Enter_AmingView();

            if (SwapViewKey == 1)
            {
                Swap_FirstOrThirdView(true);
            }
            else if (SwapViewKey == -1)
            {
                Swap_FirstOrThirdView(false);
            }
        }


        void Swap_FirstOrThirdView(bool isClose)
        {            
            if (currentViewState == CameraViewState.OtherObjectView) return;
            ChangeViewState(isClose ? CameraViewState.ThirdPersonView_Close : CameraViewState.ThirdPersonView_Far);
        }
        void Enter_AmingView()
        {
            if (currentViewState == CameraViewState.OtherObjectView) return;
            ChangeViewState(currentViewState == CameraViewState.AmingView? CameraViewState.ThirdPersonView_Close : CameraViewState.AmingView);            
        }

        //플레이어의 애니메이션 상태(true면 upperAvatar 가중치 높히고, State_Wepon활성화)를 변경하고, 무기의 위치를(true면 손으로) 변경
        void SetPlayerAnimAndWeaponPos(bool IsCloseView)
        {
            animator.SetLayerWeight(1, IsCloseView?0.33f:0);
            animator.SetFloat("State_Wepon", IsCloseView?1:0);

            Weapon.transform.SetParent(IsCloseView? HandHip.transform : HudeHip.transform, false);
            Weapon.transform.localPosition = Vector3.zero;
            Weapon.transform.localRotation = Quaternion.identity;
        }
        /// <summary> ViewOffsetValue : 타겟으로부터 추가 위치 보정 값, ZoomMinMax : 줌 최대 최소 값 </summary>
        void SetCloseToTarget(Vector3 ViewOffsetValue, Vector2 ZoomMinMax)
        {
            cam.OffsetFromHead = ViewOffsetValue;
            cam.minZoom = ZoomMinMax.x;
            cam.maxZoom = ZoomMinMax.y;
        }

        #region interfaces
        public void BindPlayerCam_To_Object(bool isBind, GameObject targetObject, Vector2 zoomMinMax)
        {
            if (isBind)
                ChangeViewState(CameraViewState.OtherObjectView);
            else
                ChangeViewState(beforeState);

            //카메라 타겟 지정
            cam.TargetTransform = isBind ? targetObject.transform : Player.transform;
            //줌 설정
            if (isBind)
            {
                SetCloseToTarget(new Vector3(0, -1.21f, 0), zoomMinMax);
            }
            else
            {
                // SetCloseToTarget을 ChangeViewState(beforeState);에서 이미 실행하기 때문에 x
            }
        }

        public bool GetRayAtCamera(out Ray ray)
        {
            if(TryGetComponent(out Camera camera))
            {
                ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                return true;
            }
            else
            {
                ray = default;
                Debug.LogError("Cannot find Camera Object!!");
                return false;
            }
        }
        #endregion
    }
}

