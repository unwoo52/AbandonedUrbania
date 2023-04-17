using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class WeaponState : MonoBehaviour
    {
        [HideInInspector] public WeaponViews weaponViews;
        [HideInInspector] public WeaponStateController WeaponStateController { get; private set; }

        protected WeaponController weaponController;
        public CharacterStateController CharacterStateController;
        /// <summary>
        /// Gets the CharacterActor component of the gameObject.
        /// </summary>
        public CharacterActor CharacterActor { get; private set; }
        /// <summary>
        /// Gets the CharacterBrain component of the gameObject.
        /// </summary>
        // public CharacterBrain CharacterBrain{ get; private set; }
        CharacterBrain CharacterBrain = null;
        /// <summary>
        /// Gets the current brain actions CharacterBrain component of the gameObject.
        /// </summary>
        public CharacterActions CharacterActions
        {
            get
            {
                return CharacterBrain == null ? new CharacterActions() : CharacterBrain.CharacterActions;
            }
        }

        #region Public Methods
        public WeaponViews SetDisable()
        {
            WeaponStateController.EnqueueTransition<SubMachinegun_Disable>();

            return WeaponStateController.currWeaponView;
        }
        #endregion


        #region unity CallBacks
        private void Awake()
        {
            CharacterActor = this.GetComponentInBranch<CharacterActor>();
            CharacterBrain = this.GetComponentInBranch<CharacterActor, CharacterBrain>();
            WeaponStateController = this.GetComponentInBranch<CharacterActor, WeaponStateController>();
            CharacterStateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
            weaponController = GetComponent<WeaponController>();
        }
        #endregion

        #region Check Transition
        /// <summary>
        /// Checks if the required conditions to exit this state are true. If so it returns the desired state (null otherwise). After this the state machine will
        /// proceed to evaluate the "enter transition" condition on the target state.
        /// </summary>
        public virtual void CheckExitTransition()
        {
        }

        /// <summary>
        /// Checks if the required conditions to enter this state are true. If so the state machine will automatically change the current state to the desired one.
        /// </summary>  
        public virtual bool CheckEnterTransition(WeaponState fromState)
        {
            return true;
        }
        #endregion

        #region Behaviors
        /// <summary>
        /// This method runs once when the state has entered the state machine.
        /// </summary>
        public virtual void EnterBehaviour(float dt, WeaponState fromState)
        {
        }
        /// <summary>
        /// This method runs once when the state has exited the state machine.
        /// </summary>
        public virtual void ExitBehaviour(float dt, WeaponState toState)
        {
        }


        /// <summary>
        /// This methods runs before the main Update method.
        /// </summary>
        public virtual void PreUpdateBehaviour(float dt)
        {
        }

        /// <summary>
        /// This method runs frame by frame, and should be implemented by the derived state class.
        /// </summary>
        public virtual void UpdateBehaviour(float dt)
        {
        }

        /// <summary>
        /// This methods runs after the main Update method.
        /// </summary>
        public virtual void PostUpdateBehaviour(float dt)
        {
        }
        #endregion

    }

}
