using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class WeaponStateController : MonoBehaviour
    {
        [Header("States")]
        public WeaponState initialState = null;
        public WeaponState CurrentState { get; protected set; }
        public WeaponState PreviousState { get; protected set; }

        [Header("weapon and hand position")]
        //weapon and hand position
        [SerializeField] GameObject weaponObject;
        [SerializeField] GameObject handPosition;
        [SerializeField] GameObject backPosition;
        public GameObject WeaponObject => weaponObject;


        [Header("Cam")]
        [SerializeField] Camera3D camera3D;
        public Camera3D Camera3D => camera3D;
        public Camera Cam;

        [SerializeField] ControllCamera3D controllCamera3D;
        public ControllCamera3D ControllCamera3D => controllCamera3D;

        bool machineStarted = false;

        public void ChangeWeaponPos_Hand()
        {
            WeaponObject.transform.SetParent(handPosition.transform, false);
        }

        public void ChangeWeaponPos_Back()
        {
            WeaponObject.transform.SetParent(backPosition.transform, false);
        }

        Queue<WeaponState> transitionsQueue = new Queue<WeaponState>();

        #region unity Callbacks
        private void Awake()
        {
            camera3D = transform.parent.parent.parent.GetChild(0).GetComponent<Camera3D>();
            controllCamera3D = transform.parent.parent.parent.GetChild(0).GetComponent<ControllCamera3D>();
            Cam = camera3D.gameObject.GetComponent<Camera>();


            if (Cam == null || controllCamera3D == null || Camera3D == null)
            {
                Debug.LogError("Transform rig is Changed!!");
            }

            //기본 상태가 없으면,

            if (weaponObject == null)
            {
                weaponObject = transform.GetChild(0).gameObject;
            }

        }
        private void FixedUpdate()
        {
            if (!machineStarted)
            {
                if (initialState == null)
                {
                    enabled = false;
                    return;
                }

                CurrentState = initialState;


                if (CurrentState == null)
                    return;

                if (!CurrentState.isActiveAndEnabled)
                    return;

                machineStarted = true;
                CurrentState.EnterBehaviour(0f, CurrentState);
            }



            if (CurrentState == null)
                return;

            if (!CurrentState.isActiveAndEnabled)
                return;


            if (!machineStarted)
            {
                CurrentState.EnterBehaviour(0f, CurrentState);
                machineStarted = true;
            }


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

        #endregion


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
            {
                Debug.LogError("state를 찾지 못했습니다!");
                return;
            }

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
