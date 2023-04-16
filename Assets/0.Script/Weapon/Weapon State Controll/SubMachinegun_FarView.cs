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
            /*
            if (CharacterActions.Fire2.value == true)
            {
                WeaponStateController.EnqueueTransition<Sniper_AimState>();
            }*/
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
            base.ExitBehaviour(dt, toState);
        }


        [Header("측방사격에 관련된 field")]
        [SerializeField] int FrontAimRunLayerNum = 2;
        [SerializeField] int SideAimRunLayerNum = 3;
        public override void UpdateBehaviour(float dt)
        {
            float speed = CharacterActor.Animator.GetFloat("PlanarSpeed");
            
            if (speed > 4 && CharacterStateController.CurrentState == CharacterStateController.GetState<NormalMovement>())
            {
                //get camdir
                Vector3 cameraForward = WeaponStateController.Cam.transform.forward;
                Vector3 characterForward = CharacterActor.Forward;


                float angle = Vector3.SignedAngle(cameraForward, characterForward, Vector3.up);
                           

                bool isCanLateralFiring = Mathf.Abs(angle) < 110f;
                if (!isCanLateralFiring)
                {
                    CharacterActor.Animator.SetLayerWeight(SideAimRunLayerNum, 0);
                    CharacterActor.Animator.SetLayerWeight(FrontAimRunLayerNum, 1);
                }
                else
                {
                    if (CharacterActions.Fire1.value == true)
                    {
                        WeaponStateController.DoFire();
                    }

                    CharacterActor.Animator.SetLayerWeight(SideAimRunLayerNum, Mathf.Abs(angle) / 110);
                    CharacterActor.Animator.SetLayerWeight(FrontAimRunLayerNum, Mathf.Abs(Mathf.Abs(angle) / 110 - 1));
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
            else
            {
                CharacterActor.Animator.SetLayerWeight(SideAimRunLayerNum, 0);
                CharacterActor.Animator.SetLayerWeight(FrontAimRunLayerNum, 0);
            }
        }
    }
}
