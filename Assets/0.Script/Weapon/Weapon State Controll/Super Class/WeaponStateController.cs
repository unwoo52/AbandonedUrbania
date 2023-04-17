using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace Urban_KimHyeonWoo
{
    public enum WeaponViews
    {
        Far, Close, Aim, None
    }
    public class WeaponStateController : MonoBehaviour, ISetWeaponDisable, ISetWeaponsAble
    {
        #region interface---------------
        public WeaponViews SetWeaponDisable()
        {
            EnqueueTransition<SubMachinegun_Disable>();
            return CurrentState.weaponViews;
        }
        public void SetWeaponsAble(WeaponViews weaponViews)
        {
            //currWeaponView = weaponViews;
            foreach (var weaponObject in GameObject.FindObjectsOfType<WeaponState>())
            {
                //weaponStates.Add(weaponObject);
                if(weaponObject.weaponViews == weaponViews)
                {
                    string tesmp = weaponObject.name;
                    EnqueueTransition<>();
                    break;
                }
            }



            
        }
        #endregion

        Dictionary<string, WeaponState> states = new Dictionary<string, WeaponState>();

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

        #region unity Callbacks         -----------------
        private void Awake()
        {
            AddStates
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

        #region Init                    ----------------- 
        void AddStates()
        {
            WeaponState[] statesArray = FindObjectsOfType<WeaponState>();
            for (int i = 0; i < statesArray.Length; i++)
            {
                WeaponState state = statesArray[i];
                string stateName = state.GetType().Name;

                // The state is already included, ignore it!
                if (GetState(stateName) != null)
                {
                    Debug.Log("Warning: GameObject " + state.gameObject.name + " has the state " + stateName + " repeated in the hierarchy.");
                    continue;
                }

                states.Add(stateName, state);
            }

        }
        #endregion

        #region GetStateMethod

        /// <summary>
        /// Searches for a particular state.
        /// </summary>
        public WeaponState GetState(string stateName)
        {
            states.TryGetValue(stateName, out WeaponState state);

            return state;
        }

        /// <summary>
        /// Searches for a particular state.
        /// </summary>
        public WeaponState GetState<T>() where T : WeaponState
        {
            string stateName = typeof(T).Name;
            return GetState(stateName);
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
