using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class RocketLauncher_CloseView : WeaponState
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

        private void Start()
        {
            UpperAimRunLayerNum = (int)AnimationLayerMaskIndex.LayerDictionary.Upper_Layer;
            weaponBlendTree = (float)AnimationLayerMaskIndex.WeaponBlendTree.RocketLauncher / 10;
        }

        public override bool CheckEnterTransition(WeaponState fromState)
        {
            return base.CheckEnterTransition(fromState);
        }
        public override void CheckExitTransition()
        {
            if (CharacterActions.Wheelupdown.value < 0f)
            {
                WeaponStateController.EnqueueTransition<RocketLauncher_FarView>();
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
        }
    }
}
