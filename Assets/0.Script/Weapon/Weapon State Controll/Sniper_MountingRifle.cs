using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class Sniper_MountingRifle : WeaponState
    {
        public override bool CheckEnterTransition(WeaponState fromState)
        {
            return base.CheckEnterTransition(fromState);
        }
        public override void CheckExitTransition()
        {
            base.CheckExitTransition();
        }
        public override void EnterBehaviour(float dt, WeaponState fromState)
        {
            base.EnterBehaviour(dt, fromState);
        }
        public override void ExitBehaviour(float dt, WeaponState toState)
        {
            base.ExitBehaviour(dt, toState);
        }
        public override void UpdateBehaviour(float dt)
        {
            base.UpdateBehaviour(dt);
        }
    }
}

