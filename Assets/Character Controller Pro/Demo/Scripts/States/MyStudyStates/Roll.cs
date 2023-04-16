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
        [Tooltip("시작될 때 롤 속도")]
        protected float initialVelocity = 12f;


        [Min(0f)]
        [SerializeField]
        [Tooltip("지속시간 예상치...;;")]
        protected float duration = 1;

        [SerializeField]
        [Tooltip("롤링 중 중력")]
        protected float rollGrabity = 1;


        [SerializeField]
        [Tooltip("슬라이드 속도의 커브 그래프")]
        protected AnimationCurve movementCurve = AnimationCurve.Linear(0, 1, 1, 0);


        [SerializeField]
        [Tooltip("속도에 지형값을 곱하는 것을 무시할지에 대한 여부")]
        protected bool ignoreSpeedMultipliers = false;

        bool IsFromSuperJump;

        //[SerializeField]
        //[Tooltip("true면 플레이어의 alwaysNotGrounded을 true로 설정해서 State도중 플레이어 상태를 항상 공중으로 처리")]
        //슬라이드는 공중에서 안쓰므로 주석처리
        //protected bool forceNotGrounded = true;

        /*
        [Min(0f)]
        [SerializeField]
        protected int availableNotGroundedDashes = 1;
        //땅이 아닐 때 사용할 수 있는 대시 횟수.
        //땅이 아닐 때 슬라이드는 안할거니까 주석 처리
        */

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        //늪, 얼음 등의 지형을 받아올 필드
        protected MaterialController materialController = null;

        //공중에서 남은 사용 횟수
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

        #region OnGroundedStateEnter에 람다식 추가
        //땅에 닿으면, 남은 대시 횟수를
        //availableNotGroundedDashes(1)로 설정하는 함수, 즉 땅에 닿으면 대시 가능 횟수 초기화.
        //위 함수를
        //CharacterActor.OnGroundedStateEnter에 추가하는 코드
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


            //공중에서 대시 가능 횟수를 availableNotGroundedDashes(1)로 초기화
            //airDashesLeft = availableNotGroundedDashes;
        }
        #endregion

        #region STATE METHOD - Enter and Exit State Transition

        // Write your transitions here
        public override bool CheckEnterTransition(CharacterState fromState)
        {
            if (fromState == CharacterStateController.GetState<SuperJump>())
            {
                rollDirection = fromState.CharacterActor.Velocity.normalized;
                IsFromSuperJump = true;
            }
            else
            {
                rollDirection = CharacterActor.Forward;
                IsFromSuperJump = false;
            }

            TriggerOnSuperJump = false;

            return true;
        }
        bool TriggerOnSuperJump;//설정되어있으면 롤이 끝날 때 슈퍼점프 실행CharacterActions.jump.value
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
            //==== My code ====
            isRollEnd = false;

            //==== Legacy Demo Code ====

            //루트모션 안쓸거니까
            CharacterActor.UseRootMotion = false;

            //땅에 닿았을 때 지형처리
            if (CharacterActor.IsGrounded)
            {
                //지형 수치 무시 여부 필드를 비활성화 하였다면,
                if (!ignoreSpeedMultipliers)
                {
                    //현재 속도 곱하기 값을 
                    currentSpeedMultiplier =
                        materialController != null ?
                                //이와 같이 설정
                                materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier
                                : 1f;
                }
            }
            //공중일 때 지형 처리        
            else
            {
                if (!ignoreSpeedMultipliers)
                {
                    currentSpeedMultiplier = materialController != null ? materialController.CurrentVolume.speedMultiplier : 1f;
                }
                //airDashesLeft--;
            }

            //Set the dash direction


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
            //forceNotGrounded(항상 땅에 안닿게 처리하는 필드) 가 true이면

            /*
            if (forceNotGrounded)
                CharacterActor.alwaysNotGrounded = false;
            */

            //퇴장 Behaviour에서 다시 원래대로 alwaysNotGrounded = false.
            //(Dash에서는 시작할 때 true로 설정했었음. slide는 ExitBehaviour 코드와 EnterBehaviour 코드를 주석처리.)

            //===CharacterActor.alwaysNotGrounded 설명===
            //alwaysNotGrounde를 false로 설정함으로써, 플레이어가 다시 땅에 닿으면, 땅에 닿은것으로 처리될 수 있도록 함
            //반대로 alwaysNotGrounded가 만약 true라면 땅에 닿아도 땅에 닿은것으로 처리를 안했다.
        }

        // Write your update code here
        [Tooltip("높을수록 마우스 방향으로 캐릭터가 빨리 바라봅니다.")]
        [SerializeField] float RotatePower = 200;
        public override void UpdateBehaviour(float dt)
        {
            Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(rollCursor) * rollDirection;

            //기존의 Velocity 고정 코드
            //CharacterActor.Velocity = dashVelocity;

            //===개선한 중력 적용 코드===
            CharacterActor.PlanarVelocity = dashVelocity; // + 마우스 방향 정면 * VelocityForceToMouseDirection

            //마우스 방향으로 회전, rotateForceTomouseDirection = if(캐릭터 정면 기준으로 마우스 방향에 따라 + or - )
            //다음과 같은 코드를 완성해줘. actorDir는 내 캐릭터의 정면 방향이고, mouseDir은 내 마우스가 바라보고 있는 방향이야.
            //이 코드는 UpdateBehaviour에 작성되고 있어. CharacterActor.RotateYaw(); 를 이용해서 myRotateSpeed속도로 캐릭터가 마우스 방향으로 회전하기를 원해
            Vector3 mouseDir = CharacterActor.CurCam.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(mouseDir.x, 0f, mouseDir.z));
            Quaternion newRotation = Quaternion.RotateTowards(CharacterActor.transform.rotation, targetRotation, RotatePower * Time.deltaTime);
            CharacterActor.RotateYaw(newRotation.eulerAngles.y - CharacterActor.transform.rotation.eulerAngles.y);



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

        #region CheckContacts, 슬라이드 중 다른 객체와 충돌했을 때에 연산
        public override void PostUpdateBehaviour(float dt)
        {
            isDone |= CheckContacts();
        }
        [Header("Cancel On Contact Fields")]
        [Tooltip("다른 리지드바디와 충돌했을 때, 캐릭터를 정지시킬지에 대한 여부.. true면, 접촉한 대상과 ")]
        [SerializeField]
        protected bool cancelOnContact = true;

        [Tooltip("접점 속도(규모)가 이 값보다 크면 대시가 즉시 취소됩니다.")]
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
