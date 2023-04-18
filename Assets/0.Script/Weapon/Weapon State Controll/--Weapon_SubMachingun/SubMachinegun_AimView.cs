using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

namespace Urban_KimHyeonWoo
{
    public class SubMachinegun_AimView : WeaponState
    {
        public override void CheckExitTransition()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                WeaponStateController.EnqueueTransition<SubMachinegun_CloseView>();
            }            
            else if (CharacterActions.Wheelupdown.value < 0f)
            {
                WeaponStateController.EnqueueTransition<SubMachinegun_FarView>();
            }
        }
        [SerializeField] Transform WeaponAimTranformParent;
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            WeaponStateController.WeaponObject.transform.SetParent(WeaponAimTranformParent, false);
            CharacterStateController.IsFixedLookdir = true;
            WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.FirstPerson;
            WeaponStateController.Camera3D.OffsetFromHead = Vector3.zero;
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            CharacterStateController.IsFixedLookdir = false;
            base.ExitBehaviour(dt, toState);
        }
        public override void UpdateBehaviour(float dt)
        {
            base.UpdateBehaviour(dt);
        }
    }
}

