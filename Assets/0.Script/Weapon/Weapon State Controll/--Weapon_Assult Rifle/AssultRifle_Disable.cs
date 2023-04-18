using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;
namespace Urban_KimHyeonWoo
{
    public class AssultRifle_Disable : WeaponState
    {
        private void Start()
        {
            weaponViews = WeaponStateType.Disable;
        }
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            CharacterActor.Animator.SetTrigger("Trigger_SwapWeapon");
            //총 위치를 등 뒤로 전환
            WeaponStateController.ChangeWeaponPos_Back();

            //set weaponController disble => 총 발사, 재장전 입력을 막고 FSM을 비활성화 상태로 전환
            weaponController.ChangeWeaponFireState(WeaponController.WeaponFireState.Disable);
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            WeaponStateController.ChangeWeaponPos_Hand();

            weaponController.ChangeWeaponFireState(WeaponController.WeaponFireState.CanFire);
        }

        #region weapon Swap Method
        public override void EnqueueSelfState()
        {
            WeaponStateController.ForceState(this);
        }
        #endregion
    }
}

