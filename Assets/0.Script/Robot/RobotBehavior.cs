using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class RobotBehavior : MonoBehaviour, IDrawAttention
    {
        #region interface
        public void OnDrawAttention(GameObject gameObject)
        {
            tv = gameObject.transform.position;
            ChangeFSM(RobotState.Searching);
        }

        #endregion

        private List<GameObject> targets = new List<GameObject>();
        private float detectionInterval = 1f;
        //StartCoroutine(DetectTargets());
        Coroutine DetectTargetsCorutine;
        [SerializeField] AIFollowBehaviour aIFollowBehaviour;
        RobotActions robotActions;
        robotWeaponSystem robotWeaponSystem;
        [SerializeField] Animator animator;


        //controlled field
        public float WeaponInputKey;

        [Header("�÷��̾� ���� ����")]
        [SerializeField] float detectionRange = 50;  // ���� �Ÿ�
        [SerializeField] float WeaponRange = 20;
        [SerializeField] float WeaponincreaseRange = 4;

        [Header("���� ����͸�")]
        GameObject trackTarget;


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


        //================


        #region FSM
        public enum RobotState
        {
            Sleep, WakeUp, Wait, Searching, Tracking, InBattle, OnControll, Destroy
        }
        [SerializeField] RobotState currentState = RobotState.Sleep;
        public RobotState CurrentState => currentState;
        public void ChangeFSM(RobotState robotState)
        {
            if(currentState == robotState) { return; }

            //�ٲ�� �� currentState�� ���� �� �۵� �Լ� ����
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    StopCoroutine(DetectTargetsCorutine);
                    // Do wait behavior
                    break;

                case RobotState.Searching:
                    // Do sleep behavior
                    break;

                case RobotState.Tracking:
                    // Do tracking behavior
                    break;

                case RobotState.InBattle:
                    // Do in-battle behavior
                    break;

                case RobotState.OnControll:
                    //�ӽ� �ڵ�. ��Ʈ�� ��Ʈ�ѷ� ������ �κ��� AnimEvent�� SearchingTarget��ȯ�Ǵ°��� ���� ����
                    //���� ���´� OnControll�̰�, �ٲ� ���°� SearchingTarget�̸� ��� return
                    if (robotState == RobotState.Wait)
                    {
                        return;
                    }
                    // Do on-control behavior
                    break;

                case RobotState.Destroy:
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
            currentState = robotState;
            //�ٲ� �� currentState�� ���� �� �۵� �Լ� ����
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.WakeUp:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    aIFollowBehaviour.DisableFollowTarget();
                    if (DetectTargetsCorutine != null) StopCoroutine(DetectTargetsCorutine);
                    DetectTargetsCorutine = StartCoroutine(DetectTargets());
                    break;

                case RobotState.Searching:
                    // Do sleep behavior
                    break;

                case RobotState.Tracking:
                    float minDistance = Mathf.Infinity;
                    foreach (GameObject target in targets)
                    {
                        float dis = Vector3.Distance(transform.position, target.transform.position);
                        if (dis < minDistance)
                        {
                            minDistance = dis;
                            trackTarget = target;
                        }
                    }
                    aIFollowBehaviour.SetFollowTarget(trackTarget.transform);

                    aIFollowBehaviour.IsTrackingState = true;
                    // Do tracking behavior
                    break;

                case RobotState.InBattle:
                    aIFollowBehaviour.DisableFollowTarget();
                    // Do in-battle behavior
                    break;

                case RobotState.OnControll:
                    // Do on-control behavior
                    aIFollowBehaviour.DisableFollowTarget();
                    GetComponent<RobotComponenetManager>().ControllPanel.gameObject.SetActive(true);
                    break;

                case RobotState.Destroy:
                    animator.SetTrigger("Trigger_Destroy");
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
        }
        void StateProcess()
        {
            //currentState�� FSM Update ����
            switch (currentState)
            {
                case RobotState.Sleep:
                    // Do sleep behavior
                    break;

                case RobotState.WakeUp:
                    // Do sleep behavior
                    break;

                case RobotState.Wait:
                    // Do wait behavior
                    break;

                case RobotState.Searching:
                    SearchingBehavior();
                    break;

                case RobotState.Tracking:
                    TrackingBehavior();
                    break;

                case RobotState.InBattle:
                    InBattleBehavior();
                    break;

                case RobotState.OnControll:
                    InControllBehavior();
                    break;

                case RobotState.Destroy:
                    // Do destroy behavior
                    break;

                default:
                    break;
            }
        }
        #endregion


        //================


        #region FSMBehaviors
        [SerializeField] float SearchingDistance = 10;
        Vector3 tv;
        void SearchingBehavior()
        {
            robotActions.looktarget(tv);

            float distance = Vector3.Distance(transform.position, tv);

            if (distance > SearchingDistance)
            {
                aIFollowBehaviour.IsTrackingState = true;
            }
            else
            {
                aIFollowBehaviour.IsTrackingState = false;
            }                
        }
        void TrackingBehavior()
        {
            robotActions.looktarget(trackTarget.transform.position);
            float distance = Vector3.Distance(transform.position, trackTarget.transform.position);
            if (distance < WeaponRange)
            {
                ChangeFSM(RobotState.InBattle);
            }
            if (distance > detectionRange)
            {
                ChangeFSM(RobotState.Wait);
            }
        }

        void InBattleBehavior()
        {
            robotActions.looktarget(trackTarget.transform.position);
            robotWeaponSystem.AllWeaponFire(trackTarget.transform.position);
            //���� ��Ÿ� �� �÷��̾ ������
            float distance = Vector3.Distance(transform.position, trackTarget.transform.position);
            if (distance > WeaponRange + WeaponincreaseRange)
            {
                ChangeFSM(RobotState.Tracking);
            }

            if (targets.Count == 0)
            {
                ChangeFSM(RobotState.Wait);
            }
        }
        void InControllBehavior()
        {
            if(WeaponInputKey > 0)
            {
                Vector3 robotEyeForward = GetComponent<RobotComponenetManager>().RobotEye.transform.forward;
                Vector3 targetPosition = GetComponent<RobotComponenetManager>().RobotEye.transform.position + robotEyeForward * 100f;
                robotWeaponSystem.AllWeaponFire(targetPosition);
            }
        }
        #endregion


        //================

        /*public methods*/
        public void OrderToRobot_WakeUp()
        {
            animator.SetTrigger("Trigger_Awake");
        }
        public void WakeUpRobot()
        {
            ChangeFSM(RobotState.Wait);
        }


        public void MoveToDir(float h, float v)
        {
            aIFollowBehaviour.SetMoveDir(h, v);
        }
        /*codes*/
        void SearchingCloseTarget()
        {
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
                        // Check if there is no object between this object and player
                        RaycastHit hit;
                        Vector3 dir = player.transform.position - transform.position;
                        Vector3 rayOrigin = transform.position + dir*10;
                        if (Physics.Raycast(rayOrigin, (player.transform.position - rayOrigin).normalized, out hit, detectionRange, LayerMask.GetMask("Hitable")))
                        {
                            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Hitable"))
                            {
                                targets.Add(player);
                            }
                        }
                        else
                        {
                            targets.Add(player);
                        }
                    }
                }

            }
        }
        IEnumerator DetectTargets()
        {
            while (true)
            {
                yield return new WaitForSeconds(detectionInterval);

                SearchingCloseTarget();

                if (targets.Count > 0)
                {
                    ChangeFSM(RobotState.Tracking);
                    break;
                }
            }
        }

        
    }

}
