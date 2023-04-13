using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.Utilities;
using UnityEngine.UI;

namespace Urban_KimHyeonWoo
{
    public class SuperJump : CharacterState
    {
        [Min(0f)]
        [SerializeField]
        [Tooltip("���۵� �� ���� �Ŀ� �ӵ�")]
        protected float initialUpVelocity = 12f;
        protected float initialForwardVelocity = 12f;


        [Min(0f)]
        [SerializeField]
        [Tooltip("���� ä���ð�")]
        protected float duration = 1;

        [SerializeField]
        [Tooltip("���� �� �߷�")]
        protected float inJumpGrabity = 1;


        [SerializeField]
        [Tooltip("ĳ������ ���� �ۿ��ϴ� �� Ŀ�� �׷���")] // ó���� �İ������� ���Ҵٰ� ���� �پ��
        protected AnimationCurve upforceCurve = AnimationCurve.Linear(0, 1, 1, 0);


        [SerializeField]
        [Tooltip("�ӵ��� �������� ���ϴ� ���� ���������� ���� ����")]
        protected bool ignoreSpeedMultipliers = false;

        //[SerializeField]
        //[Tooltip("true�� �÷��̾��� alwaysNotGrounded�� true�� �����ؼ� State���� �÷��̾� ���¸� �׻� �������� ó��")]
        //�����̵�� ���߿��� �Ⱦ��Ƿ� �ּ�ó��
        //protected bool forceNotGrounded = true;

        /*
        [Min(0f)]
        [SerializeField]
        protected int availableNotGroundedDashes = 1;
        //���� �ƴ� �� ����� �� �ִ� ��� Ƚ��.
        //���� �ƴ� �� �����̵�� ���ҰŴϱ� �ּ� ó��
        */

        // ������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������
        // ������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������

        //��, ���� ���� ������ �޾ƿ� �ʵ�
        protected MaterialController materialController = null;

        //���߿��� ���� ��� Ƚ��
        //protected int airDashesLeft;

        protected float SuperJumpCursor = 0;

        protected Vector3 JumpProgressDirection = Vector2.right;
        protected Vector3 JumpDirection = Vector2.right;

        protected bool isDone = false;

        protected float currentSpeedMultiplier = 1f;

        //===mycode

        //protected Vector3 slideDirection2 = Vector2.right;

        #region Events

        /// <summary>
        /// This event is called when the dash state is entered.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnSuperJumpStart;

        /// <summary>
        /// This event is called when the dash action has ended.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnSuperJumpEnd;

        #endregion

        #region OnGroundedStateEnter�� ���ٽ� �߰�
        //���� ������, ���� ��� Ƚ����
        //availableNotGroundedDashes(1)�� �����ϴ� �Լ�, �� ���� ������ ��� ���� Ƚ�� �ʱ�ȭ.
        //�� �Լ���
        //CharacterActor.OnGroundedStateEnter�� �߰��ϴ� �ڵ�
        /*
        void OnEnable() => CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
        void OnDisable() => CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
        void OnGroundedStateEnter(Vector3 localVelocity)
        {
            airDashesLeft = availableNotGroundedDashes;
        }
        */
        #endregion

        #region unity callbacks
        // Write your initialization code here
        protected override void Awake()
        {
            base.Awake();

            materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();


            //���߿��� ��� ���� Ƚ���� availableNotGroundedDashes(1)�� �ʱ�ȭ
            //airDashesLeft = availableNotGroundedDashes;
        }
        #endregion

        #region STATE METHOD - Enter and Exit State Transition

        // Write your transitions here
        public override bool CheckEnterTransition(CharacterState fromState)
        {
            return true;
        }
        public override void CheckExitTransition()
        {
            if(CharacterActor.IsGrounded && CharacterActor.IsStable && isDone)
            {
                //roll
                CharacterStateController.Animator.SetBool("IsSuperJump", false);
                CharacterStateController.Animator.SetBool("IsRoll", true);
                CharacterStateController.EnqueueTransition<Roll>();
            }
            else if (CharacterActor.IsGrounded && !CharacterActor.IsStable && isDone)
            {
                //normal
                CharacterStateController.Animator.SetBool("IsSuperJump", false);
                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
        }
        #endregion

        #region STATE METHOD - Behavior
        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            //===testcode== ������ī���� �����ϰ� ������ �� =============<<<<<<<<<<<<<<<<<<
            currFrameCount = FrameCount;
            //==== My code ====

            if (CharacterActor.IsGrounded)
                CharacterActor.ForceNotGrounded();

            //==== Legacy Demo Code ====
            //�׻� ���� �ȴ��� ó��
            //�����̵�� ��ÿ� �ٸ��� ���� ���� ó���� �ؾ� ��
            //if (forceNotGrounded) CharacterActor.alwaysNotGrounded = true;

            //��Ʈ��� �Ⱦ��Ŵϱ�
            CharacterActor.UseRootMotion = false;

            //���� ����� �� ����ó��
            if (CharacterActor.IsGrounded)
            {
                //���� ��ġ ���� ���� �ʵ带 ��Ȱ��ȭ �Ͽ��ٸ�,
                if (!ignoreSpeedMultipliers)
                {
                    //���� �ӵ� ���ϱ� ���� 
                    currentSpeedMultiplier =
                        materialController != null ?
                                //�̿� ���� ����
                                materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier
                                : 1f;
                }
            }
            //������ �� ���� ó��        
            else
            {
                if (!ignoreSpeedMultipliers)
                {
                    currentSpeedMultiplier = materialController != null ? materialController.CurrentVolume.speedMultiplier : 1f;
                }
                //airDashesLeft--;
            }

            //Set the dash direction
            JumpProgressDirection = CharacterActor.Up;
            JumpDirection = CharacterActor.Forward;

            //ResetDash(); <<===================================================�����ؾ� ������, ���� ������ ���ӵ� ������ ������ ���� ����
            Debug.Log($"{CharacterActor.Velocity.x} , {CharacterActor.Velocity.y}, {CharacterActor.Velocity.z}");
            //CharacterActor.Velocity = Vector3.zero;
            isDone = false;
            SuperJumpCursor = 0;

            //Execute the event
            if (OnSuperJumpStart != null)
                OnSuperJumpStart(JumpProgressDirection);
        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            if (OnSuperJumpStart != null)
                OnSuperJumpStart(JumpProgressDirection);
            //forceNotGrounded(�׻� ���� �ȴ�� ó���ϴ� �ʵ�) �� true�̸�

            /*
            if (forceNotGrounded)
                CharacterActor.alwaysNotGrounded = false;
            */

            //���� Behaviour���� �ٽ� ������� alwaysNotGrounded = false.
            //(Dash������ ������ �� true�� �����߾���. slide�� ExitBehaviour �ڵ�� EnterBehaviour �ڵ带 �ּ�ó��.)

            //===CharacterActor.alwaysNotGrounded ����===
            //alwaysNotGrounde�� false�� ���������ν�, �÷��̾ �ٽ� ���� ������, ���� ���������� ó���� �� �ֵ��� ��
            //�ݴ�� alwaysNotGrounded�� ���� true��� ���� ��Ƶ� ���� ���������� ó���� ���ߴ�.
        }
        public Scrollbar TESTSCROLLBAR;
        public RawImage Up;
        public RawImage Down;
        Vector3 JumpVelocity;
        public float testupperPositionY;
        public int FrameCount;
        int currFrameCount;
        // Write your update code here
        public override void UpdateBehaviour(float dt)
        {
            //������ dashVelocity  �ڵ�
            //Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * upforceCurve.Evaluate(SuperJumpCursor) * JumpProgressDirection;
            //===������ ���� ���� �ڵ�
            JumpVelocity = initialUpVelocity * currentSpeedMultiplier * upforceCurve.Evaluate(SuperJumpCursor) * JumpProgressDirection;
            
            TESTSCROLLBAR.size = JumpVelocity.y * 10;

            //������ Velocity ���� �ڵ�
            //CharacterActor.Velocity = dashVelocity;

            //===������ �߷� ���� �ڵ�===
            //CharacterActor.PlanarVelocity = initialForwardVelocity * currentSpeedMultiplier * JumpDirection; // + ���콺 ���� ���� * VelocityForceToMouseDirection
            //CharacterActor.RotateYaw(); //���콺 �������� ȸ��, rotateForceTomouseDirection = if(ĳ���� ���� �������� ���콺 ���⿡ ���� + or - )
            if(currFrameCount > 0)
            {
                float temf = (1.0f / currFrameCount) - 1.0f;
                //Debug.Log(temf);

                
                CharacterActor.Position = CharacterActor.Position + new Vector3(0, testupperPositionY * -temf, 0);
                currFrameCount--;
            }
            CharacterActor.VerticalVelocity += JumpVelocity * inJumpGrabity;


            



            //===========================

            if (!isDone)
            {
                float animationDt = dt / duration;
                SuperJumpCursor += animationDt;

                if (SuperJumpCursor >= 1)
                {
                    isDone = true;
                    SuperJumpCursor = 1;
                }
            }


            //==tsetcode
            if (isDone)
            {
                Up.gameObject.SetActive(false);
                Down.gameObject.SetActive(true);
            }
            else
            {
                Up.gameObject.SetActive(true);
                Down.gameObject.SetActive(false);
            }
            //===


        }



        #endregion

        #region CheckContacts, �����̵� �� �ٸ� ��ü�� �浹���� ���� ����
        public override void PostUpdateBehaviour(float dt)
        {
            isDone |= CheckContacts();
        }
        [Tooltip("�ٸ� ������ٵ�� �浹���� ��, ĳ���͸� ������ų���� ���� ����.. true��, ������ ���� ")]
        [SerializeField]
        protected bool cancelOnContact = true;

        [Tooltip("���� �ӵ�(�Ը�)�� �� ������ ũ�� ��ð� ��� ��ҵ˴ϴ�.")]
        [SerializeField]
        protected float contactVelocityTolerance = 5f;
        bool CheckContacts()
        {
            //
            if (!cancelOnContact)
                return false;

            for (int i = 0; i < CharacterActor.Contacts.Count; i++)
            {
                Contact contact = CharacterActor.Contacts[i];

                if (contact.pointVelocity.magnitude > contactVelocityTolerance)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
