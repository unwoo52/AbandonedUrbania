using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Urban_KimHyeonWoo
{

    public class Interact_ControllRobot : MonoBehaviour
    {
        [SerializeField] GameObject Robot;
        [SerializeField] GameObject PlayerInControllRobot;
        [SerializeField] GameObject PlayerCam;
        bool IsControllState = false;

        [Header("tset")]
        [SerializeField] float CanControllNotebookDistance;


        [Header("로봇컨트롤 제어 속도")]
        [SerializeField] float RobotRotateSpeed = 1;

        [Header("플레이어 화면 고정 세팅")]
        [SerializeField] Vector2 playerBindZoomMinMax;

        RobotActions robotActions;
        RobotBehavior robotBehavior;
        public void OnStartInteract()
        {
        }
        public void OnBeingInteract()
        {

        }
        public void OnCompleteInteract()
        {
            StartControllRobot();
        }
        public void OnCancelInteract()
        {

        }
        private void Update()
        {
            if (!IsControllState) return;

            //거리 이상으로 멀어지면 상호작용 해제
            if (IsPlayerOutRange())
            {
                Debug.Log("일어날일 없음");
                EndControllRobot();
            }

            ControllRobotBehavior();

        }
        void ControllRobotBehavior()
        {
            GetInput();
            InputProcess();
        }

        //input field
        float MoveHorizonKey;
        float MoveVerticalKey;
        float LookHorizonKey;
        float LookVerticalKey;
        bool CancelInteractKey;
        bool ShotKey;
        void GetInput()
        {
            //moveHorizonKey = Input.GetAxis("Movement X");
            //moveVerticalKey = Input.GetAxis("Movement Y");
            LookHorizonKey = Input.GetAxis("Horizontal");
            LookVerticalKey = Input.GetAxis("Vertical");
            MoveHorizonKey = Input.GetAxis("Movement X");
            MoveVerticalKey = Input.GetAxis("Movement Y");
            ShotKey = Input.GetButton("QE");

            CancelInteractKey = (
                Input.GetButtonDown("Interact") ||
                Input.GetButtonDown("Fire2") ||
                Input.GetButtonDown("Jump") ||
                Input.GetButtonDown("Dash") ||
                Input.GetButtonDown("Run")
                );
        }
        //키값 보간 필드
        public int test = 1;
        public Vector3 testvec;
        void InputProcess()
        {
            robotActions.LookTarget_ControllWithInputManager(LookHorizonKey, LookVerticalKey, RobotRotateSpeed);
            robotBehavior.MoveToDir(MoveVerticalKey, -MoveHorizonKey);


            robotBehavior.WeaponInputKey = ShotKey ? 1 : 0;
            if (CancelInteractKey)
            {
                EndControllRobot();
            }
        }
        void StartControllRobot()
        {
            PlayerInControllRobot = GetComponent<InteractObject>().InteractPlayer;
            PlayerCam = GetComponent<InteractObject>().InteractCam;
            IsControllState = true;
            SetBindstatePlayerAndCam(true);
            if (Robot.TryGetComponent(out RobotActions robotActions))
            {
                this.robotActions = robotActions;
            }

            if (Robot.TryGetComponent(out RobotBehavior robotBehavior))
            {
                this.robotBehavior = robotBehavior;
            }

            if (robotBehavior.CurrentState == RobotBehavior.RobotState.Sleep)
            {
                robotBehavior.OrderToRobot_WakeUp();
            }
            robotBehavior.ChangeFSM(RobotBehavior.RobotState.OnControll);
        }
        void EndControllRobot()
        {
            SetBindstatePlayerAndCam(false);
            IsControllState = false;
            PlayerInControllRobot = null;
            PlayerCam = null;
        }
        bool IsPlayerOutRange()
        {
            return Vector3.Distance(this.transform.position, PlayerInControllRobot.transform.position) >= CanControllNotebookDistance;
        }
        void SetBindstatePlayerAndCam(bool isBind)
        {
            //bind player
            if (PlayerInControllRobot.TryGetComponent(out IBindPlayer bindPlayer))
            {
                bindPlayer.BindPlayer(isBind);
            }
            else Debug.LogError($"Cannot Find IBindPlayer at \'{PlayerInControllRobot}\'");
            //bind Cam
            if (PlayerCam.TryGetComponent(out IBindPlayerCam bindPlayerCam))
            {
                bindPlayerCam.BindPlayerCam_To_Object(isBind, this.gameObject, playerBindZoomMinMax);
            }
            else Debug.LogError($"Cannot Find IBindPlayerCam at \'{PlayerCam}\'");
        }
    }
}
