using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class WeaponStateController : MonoBehaviour
    {
        public WeaponState initialState = null;
        public WeaponState CurrentState { get; protected set; }
        public WeaponState PreviousState { get; protected set; }


        [SerializeField] GameObject weaponObject;
        public GameObject WeaponObject => weaponObject;

        [SerializeField] Camera3D camera3D;
        public Camera3D Camera3D => camera3D;
        public Camera Cam;

        [SerializeField] ControllCamera3D controllCamera3D;
        public ControllCamera3D ControllCamera3D => controllCamera3D;


        Queue<WeaponState> transitionsQueue = new Queue<WeaponState>();
        private void Awake()
        {
            camera3D = transform.parent.parent.GetChild(0).GetComponent<Camera3D>();
            controllCamera3D = transform.parent.parent.GetChild(0).GetComponent<ControllCamera3D>();
            Cam = camera3D.gameObject.GetComponent<Camera>();

            //기본 상태가 없으면,
            CurrentState = initialState;

            if (weaponObject == null)
            {
                weaponObject = transform.GetChild(0).gameObject;
            }
        }
        private void FixedUpdate()
        {
            float dt = Time.deltaTime;

            bool validTransition = CheckForTransitions();
            transitionsQueue.Clear();
            if (validTransition)
            {
                PreviousState.ExitBehaviour(dt, CurrentState);
                CurrentState.EnterBehaviour(dt, PreviousState);
            }

            CurrentState.PreUpdateBehaviour(dt);
            CurrentState.UpdateBehaviour(dt);
            CurrentState.PostUpdateBehaviour(dt);
        }

        #region Events                  -----------------

        /// <summary>
        /// This event is called when a state transition occurs. 
        /// 
        /// The "from" and "to" states are passed as arguments.
        /// </summary>
        public event System.Action<WeaponState, WeaponState> OnStateChange;

        #endregion

        #region Transition state Method -----------------

        public void EnqueueTransition<T>() where T : WeaponState
        {
            WeaponState state = GetComponent<T>();

            if (state == null)
                return;

            transitionsQueue.Enqueue(state);
        }
        bool CheckForTransitions()
        {
            CurrentState.CheckExitTransition();

            while (transitionsQueue.Count != 0)
            {
                //바뀔 state
                WeaponState thisState = transitionsQueue.Dequeue();

                if (thisState == null)
                    continue;

                if (!thisState.enabled)
                    continue;

                bool success = thisState.CheckEnterTransition(CurrentState);

                if (success)
                {
                    if (OnStateChange != null)
                        OnStateChange(CurrentState, thisState);

                    PreviousState = CurrentState;
                    CurrentState = thisState;

                    return true;
                }
            }

            return false;

        }
        #endregion 
    }
}
