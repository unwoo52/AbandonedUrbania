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

        #region self enqeue Methods
        public virtual void EnqueueSelfState()
        {
            Debug.LogError("\'EnqueueThisState\'를 구현하지 않았습니다.");
        }
        #endregion

        #region unity CallBacks
        private void Awake()
        {
            CharacterActor = this.GetComponentInBranch<CharacterActor>();
            CharacterBrain = this.GetComponentInBranch<CharacterActor, CharacterBrain>();
            WeaponStateController = GetComponent<WeaponStateController>();
            CharacterStateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
            weaponController = GetComponent<WeaponController>();
        }
        #endregion

        #region Anim Controll Method
        //애니메이터의 LayerMask의 SetLayerWeight속도를 조절하는 필드
        [SerializeField] [Tooltip("캐릭터의 모션이 전환되는 속도를 조절")] float animChangeSpeed = 0.1f;
        public void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum, float destValue, float dt)
        {
            curLayerValue = Mathf.Lerp(curLayerValue, destValue, animChangeSpeed * dt);
            CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
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

        #region Adapter
        //characterState Adapter
        public virtual void ActionStateChangeListener(CharacterState state)
        {
        }

        #endregion
    }

}
