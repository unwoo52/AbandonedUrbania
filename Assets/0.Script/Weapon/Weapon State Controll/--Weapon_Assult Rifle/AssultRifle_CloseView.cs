using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;
namespace Urban_KimHyeonWoo
{
    public class AssultRifle_CloseView : WeaponState
    {
        //animation field
        [Header("�ִϸ��̼� ���̾ ���õ� field")]
        int UpperAimRunLayerNum;
        float curUpperLayerValue = 0;
        float weaponBlendTree;

        //cam field
        [SerializeField]
        [Tooltip("ĳ���ͷκ��� ī�޶��� �Ÿ�")]
        Vector2 CloseZoomMinMax = new Vector2(0.7f, 0.8f);//<<��minmax�� �ǹ� ���������Ƿ�, ���������� �����ؾ� ��
        [SerializeField]
        [Tooltip("ĳ���͸� �������� ��� �ʸ� ī�޶��� ��ġ.")]
        Vector3 CloseViewOffsetValue = new Vector3(0.3f, -0.22f, 0.27f);

        #region unity Callbacks
        private void Start()
        {
            UpperAimRunLayerNum = (int)AnimationLayerMaskIndex.LayerDictionary.Upper_Layer;
            weaponBlendTree = (float)AnimationLayerMaskIndex.WeaponBlendTree.Rifle / 10;
            weaponViews = WeaponStateType.Close;
        }
        #endregion

        public override void CheckExitTransition()
        {
            if (CharacterActions.Wheelupdown.value < 0f)
            {
                WeaponStateController.EnqueueTransition<AssultRifle_FarView>();
            }
        }

        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            CharacterActor.Animator.SetFloat("BlendFloat_WeaponType", weaponBlendTree);

            CharacterStateController.IsFixedLookdir = true;

            WeaponStateController.ChangeWeaponPos_Hand();

            WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;
            WeaponStateController.Camera3D.OffsetFromHead = CloseViewOffsetValue;
            WeaponStateController.Camera3D.minZoom = CloseZoomMinMax.x;
            WeaponStateController.Camera3D.maxZoom = CloseZoomMinMax.y;
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            CharacterStateController.IsFixedLookdir = false;
        }

        public override void UpdateBehaviour(float dt)
        {
            SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);

            //�������������� �׻� ��ƲŸ�ӻ��·�
            if (CharacterStateController.CurrentState == CharacterStateController.GetState<SuperJump>())
            {
                weaponController.currBattleTime = 0.2f;
            }
        }

        #region weapon Swap Method
        public override void EnqueueSelfState()
        {
            WeaponStateController.ForceState(this);
        }
        #endregion
    }
}

