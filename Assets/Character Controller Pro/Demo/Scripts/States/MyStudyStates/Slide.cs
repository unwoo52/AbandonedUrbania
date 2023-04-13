using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Urban_KimHyeonWoo
{
    public class Slide : CharacterState
    {
        [Min(0f)]
        [SerializeField]
        [Tooltip("���۵� �� �����̵� �ӵ�")]
        protected float initialVelocity = 12f;


        [Min(0f)]
        [SerializeField]
        [Tooltip("���ӽð�")]
        protected float duration = 0.4f;

        [Range(0, 0.9f)]
        [SerializeField]
        [Tooltip("�ִϸ��̼��� ������ ����. 0 ~ 1")]
        protected float endPointAnimation = 0.8f;

        [SerializeField]
        [Tooltip("�����̵� �� �߷�")]
        protected float slideGrabity = 1;

        [SerializeField]
        [Tooltip("�����̵� �ӵ��� Ŀ�� �׷���")]
        protected AnimationCurve movementCurve = AnimationCurve.Linear(0, 1, 1, 0);


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

        protected float slideCursor = 0;

        protected Vector3 slideDirection = Vector2.right;

        protected bool isDone = true;

        protected float currentSpeedMultiplier = 1f;


        //protected Vector3 slideDirection2 = Vector2.right;

        #region Events

        /// <summary>
        /// This event is called when the dash state is entered.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnSlideStart;

        /// <summary>
        /// This event is called when the dash action has ended.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnSlideEnd;

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
            //===my code ===
            if (!CharacterActor.IsGrounded)
                return false;
            //===

            return true;
        }
        public override void CheckExitTransition()
        {
            if (isDone)
            {
                //update behavior���� ���ӽð��� endPointAnimation(8/10) ������ �� false�� ȣ���
                //CharacterStateController.Animator.SetBool("IsSlide", false);
                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
            else if (CharacterActions.crouch.value)
            {
                CharacterStateController.Animator.SetBool("IsSlide", false);
                CharacterStateController.Animator.SetBool("IsRoll", true);

                CharacterStateController.EnqueueTransition<Roll>();
            }
        }
        #endregion

        #region STATE METHOD - Behavior
        // Write your transitions here
        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            //==== My code ====
            CharacterStateController.Animator.SetTrigger("Trigger_Slide");
            CharacterStateController.Animator.SetBool("IsSlide", true);



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
            slideDirection = CharacterActor.Forward;


            //ResetDash();
            CharacterActor.Velocity = Vector3.zero;
            isDone = false;
            slideCursor = 0;

            //Execute the event
            if (OnSlideStart != null)
                OnSlideStart(slideDirection);

        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            if (OnSlideEnd != null)
                OnSlideEnd(slideDirection);
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
            Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(slideCursor) * slideDirection;

            //������ Velocity ���� �ڵ�
            //CharacterActor.Velocity = dashVelocity;

            //===������ �߷� ���� �ڵ�===
            CharacterActor.PlanarVelocity = dashVelocity;
            CharacterActor.VerticalVelocity += -CharacterActor.Up * slideGrabity * dt;
            //===========================


            slideCursor += dt / duration;

            if (slideCursor >= 1)
            {
                isDone = true;
                slideCursor = 0;
                CharacterStateController.Animator.SetBool("IsSlide", false);
            }

            if(slideCursor > endPointAnimation)
            {
                CharacterStateController.Animator.SetBool("IsSlide", false);
            }
        }


        #endregion

        #region CheckContacts, �����̵� �� �ٸ� ��ü�� �浹���� ���� ����
        public override void PostUpdateBehaviour(float dt)
        {
            isDone |= CheckContacts();
        }
        [Header("Cancel On Contact Fields")]
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
