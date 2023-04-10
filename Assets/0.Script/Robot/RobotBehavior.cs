using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class RobotBehavior : MonoBehaviour
    {
        [SerializeField] float WeaponArrange = 100;

        [Header("Tracking Player")]
        private List<GameObject> targets = new List<GameObject>();
        public float detectionRange = 10f;  // 일정 거리
        private float detectionInterval = 1f;
        //StartCoroutine(DetectTargets());
        Coroutine DetectTargetsCorutine;
        [SerializeField] AIFollowBehaviour aIFollowBehaviour;
        RobotActions robotActions;
        robotWeaponSystem robotWeaponSystem;

        #region FSM
        public enum RobotState
        {
            Sleep, Wait, Tracking, InBattle, OnControll, Destroy
        }
        public RobotState currentState = RobotState.Sleep;
        void ChangeFSM(RobotState robotState)
        {
            if(currentState == robotState) { return; }

            //바뀌기 전 currentState의 종료 시 작동 함수 실행
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    // Do wait behavior
                    break;

                case RobotState.Tracking:
                    // Do tracking behavior
                    break;

                case RobotState.InBattle:
                    // Do in-battle behavior
                    break;

                case RobotState.OnControll:
                    // Do on-control behavior
                    break;

                case RobotState.Destroy:
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
            currentState = robotState;
            //바뀐 뒤 currentState의 시작 시 작동 함수 실행
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    StartCoroutine(DetectTargets());
                    break;

                case RobotState.Tracking:
                    // Do tracking behavior
                    break;

                case RobotState.InBattle:
                    // Do in-battle behavior
                    break;

                case RobotState.OnControll:
                    // Do on-control behavior
                    break;

                case RobotState.Destroy:
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
        }
        void StateProcess()
        {
            //currentState의 FSM Update 실행
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    // Do wait behavior
                    break;

                case RobotState.Tracking:
                    //TrackingBehavior();
                    break;

                case RobotState.InBattle:
                    //InBattleBehavior();
                    break;

                case RobotState.OnControll:
                    // Do on-control behavior
                    break;

                case RobotState.Destroy:
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
        }
        #endregion
        #region FSMBehaviors
        void SearchTarget()
        {
            //if get,

            
            ChangeFSM(RobotState.Tracking);
        }
        void WaitBehavior()
        {
            if(targets.Count != 0)
            {
                StopCoroutine(DetectTargetsCorutine);
                ChangeFSM(RobotState.Tracking);
            }
        }
        void TrackingBehavior(GameObject Target)
        {
            
            robotActions.looktarget(Target.transform.position);

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance < WeaponArrange)
            {
                ChangeFSM(RobotState.InBattle);
            }

            TrackingTarget();
        }
        void InBattleBehavior(GameObject Target)
        {
            ChekcTargetIsNull();
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance > WeaponArrange)
            {
                ChangeFSM(RobotState.Tracking);
            }
            AttackTarget(Target);
        }
        #endregion
        #region MonobehaviorCallbacks
        private void Start()
        {
            robotActions = GetComponent<RobotActions>();
            robotWeaponSystem = GetComponent<robotWeaponSystem>();
        }
        private void Update()
        {
            StateProcess();
        }
        #endregion
        void TrackingTarget()
        {
            ChekcTargetIsNull();
            aIFollowBehaviour.IsTrackingState = true;
        }

        void AttackTarget(GameObject Target)
        {
            aIFollowBehaviour.IsTrackingState = false;
            robotWeaponSystem.AllWeaponFire(Target.transform.position);
        }
        void ChekcTargetIsNull()
        {
            if (targets.Count == 0)
            {
                ChangeFSM(RobotState.Wait);
            }
        }
        IEnumerator DetectTargets()
        {
            while (true)
            {
                yield return new WaitForSeconds(detectionInterval);

                for (int i = targets.Count - 1; i >= 0; i--)
                {
                    if (Vector3.Distance(transform.position, targets[i].transform.position) > detectionRange)
                    {
                        targets.RemoveAt(i);
                    }
                }

                foreach (GameObject player in GameManager.instance.Players)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
                    {
                        if (!targets.Contains(player))
                        {
                            targets.Add(player);
                        }
                    }
                }
            }
        }

    }

}
