using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.Utilities;

namespace Urban_KimHyeonWoo
{
    public class Roll : CharacterState
    {
        [Min(0f)]
        [SerializeField]
        [Tooltip("���۵� �� �� �ӵ�")]
        protected float initialVelocity = 12f;


        [Min(0f)]
        [SerializeField]
        [Tooltip("���ӽð� ����ġ...;;")]
        protected float duration = 1;

        [SerializeField]
        [Tooltip("�Ѹ� �� �߷�")]
        protected float rollGrabity = 1;


        [SerializeField]
        [Tooltip("�����̵� �ӵ��� Ŀ�� �׷���")]
        protected AnimationCurve movementCurve = AnimationCurve.Linear(0, 1, 1, 0);


        [SerializeField]
        [Tooltip("�ӵ��� �������� ���ϴ� ���� ���������� ���� ����")]
        protected bool ignoreSpeedMultipliers = false;

        bool IsFromSuperJump;

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

        protected float rollCursor = 0;

        protected Vector3 rollDirection = Vector2.right;

        protected bool isDone = true;

        protected float currentSpeedMultiplier = 1f;


        //protected Vector3 slideDirection2 = Vector2.right;

        #region Events

        /// <summary>
        /// This event is called when the dash state is entered.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnRollStart;

        /// <summary>
        /// This event is called when the dash action has ended.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnRollEnd;

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
            if (fromState == CharacterStateController.GetState<SuperJump>())
            {
                IsFromSuperJump = true;
            }
            else IsFromSuperJump = false;

            TriggerOnSuperJump = false;

            return true;
        }
        bool TriggerOnSuperJump;//�����Ǿ������� ���� ���� �� �������� ����CharacterActions.jump.value
        public override void CheckExitTransition()
        {

            if (isRollEnd && IsFromSuperJump == true)
            {
                CharacterStateController.Animator.SetBool("IsRoll", false);

                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
            else if (isRollEnd && TriggerOnSuperJump == false)
            {
                CharacterStateController.Animator.SetBool("IsRoll", false);

                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
            else if (isRollEnd && TriggerOnSuperJump == true && IsFromSuperJump == false)
            {
                CharacterStateController.Animator.SetBool("IsRoll", false);
                CharacterStateController.Animator.SetBool("IsSuperJump", true);

                CharacterStateController.EnqueueTransition<SuperJump>();
            }
        }
        #endregion

        #region STATE METHOD - Behavior
        // Write your transitions here
        bool isRollEnd = false;
        public void SetRoolEnd()
        {
            isRollEnd = true;
        }
        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            isRollEnd = false;
            //==== My code ====
            //duration�� �ִϸ����� Ŭ���� ���̷� ����



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
            rollDirection = CharacterActor.Forward;


            //ResetDash();
            CharacterActor.Velocity = Vector3.zero;
            isDone = false;
            rollCursor = 0;

            //Execute the event
            if (OnRollStart != null)
                OnRollStart(rollDirection);

        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            if (OnRollEnd != null)
                OnRollEnd(rollDirection);
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

        // Write your update code here
        public override void UpdateBehaviour(float dt)
        {
            Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(rollCursor) * rollDirection;

            //������ Velocity ���� �ڵ�
            //CharacterActor.Velocity = dashVelocity;

            //===������ �߷� ���� �ڵ�===
            CharacterActor.PlanarVelocity = dashVelocity; // + ���콺 ���� ���� * VelocityForceToMouseDirection
            //CharacterActor.RotateYaw(); //���콺 �������� ȸ��, rotateForceTomouseDirection = if(ĳ���� ���� �������� ���콺 ���⿡ ���� + or - )
            CharacterActor.VerticalVelocity += -CharacterActor.Up * rollGrabity * dt;
            //===========================

            float animationDt = dt / duration;
            rollCursor += animationDt;

            if (rollCursor >= 1)
            {
                isDone = true;
                rollCursor = 0;
            }

            if (CharacterActions.jump.value) TriggerOnSuperJump = true;
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
