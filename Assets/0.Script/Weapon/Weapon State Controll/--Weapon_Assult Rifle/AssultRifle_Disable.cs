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
            //�� ��ġ�� �� �ڷ� ��ȯ
            WeaponStateController.ChangeWeaponPos_Back();

            //set weaponController disble => �� �߻�, ������ �Է��� ���� FSM�� ��Ȱ��ȭ ���·� ��ȯ
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

