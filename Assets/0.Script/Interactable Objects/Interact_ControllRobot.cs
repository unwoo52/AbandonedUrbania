using UnityEngine;

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


        [Header("�κ���Ʈ�� ���� �ӵ�")]
        [SerializeField] float RobotRotateSpeed = 1;

        [Header("�÷��̾� ȭ�� ���� ����")]
        [SerializeField] Vector2 playerBindZoomMinMax;
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

            //�Ÿ� �̻����� �־����� ��ȣ�ۿ� ����
            if (IsPlayerOutRange())
            {
                Debug.Log("�Ͼ�� ����");
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
        float moveHorizonKey;
        float moveVerticalKey;
        bool CancelInteractKey;
        bool ShotKey;
        void GetInput()
        {
            moveHorizonKey = Input.GetAxis("Movement X");
            moveVerticalKey = Input.GetAxis("Movement Y");

            CancelInteractKey = (
                Input.GetButtonDown("Interact") ||
                Input.GetButtonDown("Fire2") ||
                Input.GetButtonDown("Jump") ||
                Input.GetButtonDown("Dash") ||
                Input.GetButtonDown("Run")
                );
        }
        void InputProcess()
        {
            if (Robot.TryGetComponent(out RobotActions robotActions))
            {
                robotActions.LookTarget_ControllWithInputManager(moveHorizonKey, moveVerticalKey, RobotRotateSpeed);
            }
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
