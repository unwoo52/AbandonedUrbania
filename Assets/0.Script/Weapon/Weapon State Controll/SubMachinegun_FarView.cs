using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Urban_KimHyeonWoo;

namespace Urban_KimHyeonWoo
{
    public class SubMachinegun_FarView : WeaponState
    {
        [SerializeField]
        [Tooltip("캐릭터로부터 카메라의 거리")]
        Vector2 ZoomMinMax = new Vector2(5, 12);//<<줌minmax는 의미 없어졌으므로, 고정값으로 수정해야 함
        [SerializeField]
        [Tooltip("캐릭터를 기준으로 어깨 너머 카메라의 위치.")]
        Vector3 ViewOffsetValue = new Vector3(0.4f, -0.1f, 0);

        #region EventTrigger
        bool setCloseStateTrigger = false;
        public void SetCloseState()
        {
            setCloseStateTrigger = true;
        }
        #endregion
        public override void CheckExitTransition()
        {
            if (CharacterActions.Wheelupdown.value > 0f)
            {
                WeaponStateController.EnqueueTransition<SubMachinegun_CloseView>();
            }
            if (setCloseStateTrigger == true)
            {
                setCloseStateTrigger = false;
                WeaponStateController.EnqueueTransition<SubMachinegun_CloseView>();
            }
        }
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            CharacterActor.Animator.SetLayerWeight(1, 0);

            WeaponStateController.ChangeWeaponPos_Hand();

            WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;
            WeaponStateController.Camera3D.OffsetFromHead = ViewOffsetValue;
            WeaponStateController.Camera3D.minZoom = ZoomMinMax.x;
            WeaponStateController.Camera3D.maxZoom = ZoomMinMax.y;
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            CharacterActor.Animator.SetLayerWeight(SideAimRunLayerNum, 0);
            CharacterActor.Animator.SetLayerWeight(FrontAimRunLayerNum, 0);
        }


        [Header("측방사격에 관련된 field")]
        [SerializeField] int FrontAimRunLayerNum = 2;
        float curFrontLayerValue = 0;
        [SerializeField] int SideAimRunLayerNum = 3;
        float curSideLayerValue = 0;

        [SerializeField] float animChangeSpeed = 0.1f;

        public override void UpdateBehaviour(float dt)
        {
            //get angle
            Vector3 cameraForward = WeaponStateController.Cam.transform.forward;
            Vector3 characterForward = CharacterActor.Forward;

            //캐릭터의 진행방향과 카메라의 각도를 계산
            float angle = Vector3.SignedAngle(cameraForward, characterForward, Vector3.up);

            //제한 각도가 110도인 이유는 측방사격 애니메이션이 있는 레이어의 weight가 1일 때, 캐릭터가 110도 측면을 사격하기 때문
            //수정하고 싶다면 캐릭터의 측방사격 애니메이션의 yaw rotate offset값을 수정할 것
            bool isCanLateralFiring = Mathf.Abs(angle) < 110f;

            if(isCanLateralFiring) { weaponController.FireLock = false; }
            else if(!isCanLateralFiring) { weaponController.FireLock = true;}


            //run fire animControll
            float speed = CharacterActor.Animator.GetFloat("PlanarSpeed");

            //총을 발사했다면 사격 애니메이션으로 전환, 아니라면 일반 애니메이션으로 전환
            bool isFiredRecontly = weaponController.IsFiredRecontly();

            if (isFiredRecontly && CharacterStateController.CurrentState == CharacterStateController.GetState<NormalMovement>())
            {//사격중이고, 액션중이 아니고, 장전중이 아닐 때
                if (speed > 4 )
                {//달릴 때
                    SetAnimatorLayer_LateralFiring(dt, isFiredRecontly, angle);
                }
                else//달리지 않을 때
                {
                    Vector3 mouseDir = WeaponStateController.Cam.transform.forward;
                    CharacterActor.SetYaw(mouseDir);

                    //SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);
                    SetAnimControllerSetLayerWeight(ref curSideLayerValue, SideAimRunLayerNum, 0, dt);
                    SetAnimControllerSetLayerWeight(ref curFrontLayerValue, FrontAimRunLayerNum, 0, dt);
                }
            }
            else if(!isFiredRecontly)
            {
                SetAnimControllerSetLayerWeight(ref curSideLayerValue, SideAimRunLayerNum, 0, dt);
                SetAnimControllerSetLayerWeight(ref curFrontLayerValue, FrontAimRunLayerNum, 0, dt);
            }

        }

        public override void EnqueueSelfState()
        {
            WeaponStateController.ForceState(this);
        }

        void SetAnimatorLayer_LateralFiring(float dt, bool isCanLateralFiring, float angle)
        {
            //뒤를 바라보는 방향이라 사격을 못할 때,
            if (!isCanLateralFiring)
            {
                SetAnimControllerSetLayerWeight(ref curSideLayerValue, SideAimRunLayerNum, 0, dt);
                SetAnimControllerSetLayerWeight(ref curFrontLayerValue, FrontAimRunLayerNum, 0, dt);
            }
            else//사격 가능 각도일 때,
            {

                SetAnimControllerSetLayerWeight(ref curSideLayerValue, SideAimRunLayerNum, Mathf.Abs(angle) / 110, dt);
                SetAnimControllerSetLayerWeight(ref curFrontLayerValue, FrontAimRunLayerNum, Mathf.Abs(Mathf.Abs(angle) / 110 - 1), dt);

                if (angle > 0f)//right
                {
                    CharacterActor.Animator.SetBool("IsAimRunRight", true);
                }
                else if (angle <= 0f) // left
                {
                    CharacterActor.Animator.SetBool("IsAimRunRight", false);
                }
            }
        }


        void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum,float destValue, float dt)
        {
            curLayerValue = Mathf.Lerp(curLayerValue, destValue, animChangeSpeed * dt);
            CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
        }
    }
}
