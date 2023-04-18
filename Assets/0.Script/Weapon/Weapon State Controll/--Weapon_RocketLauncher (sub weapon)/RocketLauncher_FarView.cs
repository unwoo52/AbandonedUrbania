using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimationLayerMaskIndex;

namespace Urban_KimHyeonWoo
{
    public class RocketLauncher_FarView : WeaponState
    {
        //animation field
        [Header("애니메이션 레이어에 관련된 field")]
        int UpperAimRunLayerNum;
        float curUpperLayerValue = 0;
        float weaponBlendTree;

        //cam field
        [SerializeField]
        [Tooltip("캐릭터로부터 카메라의 거리")]
        Vector2 ZoomMinMax = new Vector2(5, 12);//<<줌minmax는 의미 없어졌으므로, 고정값으로 수정해야 함
        [SerializeField]
        [Tooltip("캐릭터를 기준으로 어깨 너머 카메라의 위치.")]
        Vector3 ViewOffsetValue = new Vector3(0.4f, -0.1f, 0);

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
            if (CharacterActions.Wheelupdown.value > 0f)
            {
                WeaponStateController.EnqueueTransition<RocketLauncher_CloseView>();
            }
        }
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            CharacterActor.Animator.SetFloat("BlendFloat_WeaponType", weaponBlendTree);

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
        public override void UpdateBehaviour(float dt)
        {
            SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);
        }
    }
}

