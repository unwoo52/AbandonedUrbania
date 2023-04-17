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
        [Tooltip("ĳ���ͷκ��� ī�޶��� �Ÿ�")]
        Vector2 ZoomMinMax = new Vector2(5, 12);//<<��minmax�� �ǹ� ���������Ƿ�, ���������� �����ؾ� ��
        [SerializeField]
        [Tooltip("ĳ���͸� �������� ��� �ʸ� ī�޶��� ��ġ.")]
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


        [Header("�����ݿ� ���õ� field")]
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

            //ĳ������ �������� ī�޶��� ������ ���
            float angle = Vector3.SignedAngle(cameraForward, characterForward, Vector3.up);

            //���� ������ 110���� ������ ������ �ִϸ��̼��� �ִ� ���̾��� weight�� 1�� ��, ĳ���Ͱ� 110�� ������ ����ϱ� ����
            //�����ϰ� �ʹٸ� ĳ������ ������ �ִϸ��̼��� yaw rotate offset���� ������ ��
            bool isCanLateralFiring = Mathf.Abs(angle) < 110f;

            if(isCanLateralFiring) { weaponController.FireLock = false; }
            else if(!isCanLateralFiring) { weaponController.FireLock = true;}


            //run fire animControll
            float speed = CharacterActor.Animator.GetFloat("PlanarSpeed");

            //���� �߻��ߴٸ� ��� �ִϸ��̼����� ��ȯ, �ƴ϶�� �Ϲ� �ִϸ��̼����� ��ȯ
            bool isFiredRecontly = weaponController.IsFiredRecontly();

            if (isFiredRecontly && CharacterStateController.CurrentState == CharacterStateController.GetState<NormalMovement>())
            {//������̰�, �׼����� �ƴϰ�, �������� �ƴ� ��
                if (speed > 4 )
                {//�޸� ��
                    SetAnimatorLayer_LateralFiring(dt, isFiredRecontly, angle);
                }
                else//�޸��� ���� ��
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
            //�ڸ� �ٶ󺸴� �����̶� ����� ���� ��,
            if (!isCanLateralFiring)
            {
                SetAnimControllerSetLayerWeight(ref curSideLayerValue, SideAimRunLayerNum, 0, dt);
                SetAnimControllerSetLayerWeight(ref curFrontLayerValue, FrontAimRunLayerNum, 0, dt);
            }
            else//��� ���� ������ ��,
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
