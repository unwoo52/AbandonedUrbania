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
        [Tooltip("시작될 때 점프 파워 속도")]
        protected float initialUpVelocity = 12f;
        protected float initialForwardVelocity = 12f;


        [Min(0f)]
        [SerializeField]
        [Tooltip("공중 채류시간")]
        protected float duration = 1;

        [SerializeField]
        [Tooltip("점프 중 중력")]
        protected float inJumpGrabity = 1;


        [SerializeField]
        [Tooltip("캐릭터의 위로 작용하는 힘 커브 그래프")] // 처음에 파격적으로 높았다가 점점 줄어듬
        protected AnimationCurve upforceCurve = AnimationCurve.Linear(0, 1, 1, 0);


        [SerializeField]
        [Tooltip("속도에 지형값을 곱하는 것을 무시할지에 대한 여부")]
        protected bool ignoreSpeedMultipliers = false;

        //-----------
        [Header("슈퍼점프 이펙트에 관련된 field")]
        [Tooltip("이펙트가 발생하는 위치")]
        [SerializeField] Transform FootPositionR;
        [SerializeField] Transform FootPositionL;
        [SerializeField] GameObject EffectPrefab;



        //-----------
        [Header("슈퍼점프 시작 시 발생하는 폭팔적인 점프에 관련된 field")]
        [Tooltip("슈퍼 점프 시작 시 폭팔적으로 위로 가려는 힘")]
        [SerializeField] float JumpUp_UpPower;

        [Tooltip("슈퍼 점프시 폭팔적으로 위로 가는 힘이 가해질 프레임들. 만약 20이면 20프레임동안 폭팔적인 상승을 \'CharacterActor.Position = \'으로 JumpUp_UpperPowerCurv만큼 실행")]
        [SerializeField] int JumpUp_FrameCount;

        [Tooltip("슈퍼 점프시 폭팔적으로 위로 가는 힘의 곡선")]
        [SerializeField] AnimationCurve JumpUp_UpperPowerCurv;
        int JumpUp_currFrameCount;

        //-----------
        [Header("이전 롤 state에서 넘어온 힘을 관리하는 field")]
        [Tooltip("이전 롤 state에서 넘어온 Velocity가 남아있는 양. 높으면 롤 속도만큼 멀리 밀려나고, 적으면 WASD키방향의 힘만 온전히 적용.")]
        [SerializeField] float BehindExitRoll_MultipleRollVelocity = 1;
        // 롤 이후 점프할 때, 앞 방향으로 가해지는 힘
        [Tooltip("롤 이후 점프할 때, WASD키 방향으로 가해지는 힘.")]
        [SerializeField] float BehindExitRoll_JumpDirForce = 1;

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
            //==== My code ====

            JumpUp_currFrameCount = 0;
            if (CharacterActor.IsGrounded)
                CharacterActor.ForceNotGrounded();

            Instantiate(EffectPrefab, FootPositionL.position, Quaternion.identity);
            Instantiate(EffectPrefab, FootPositionL.position, Quaternion.identity);

            //==== Legacy Demo Code ====
            //항상 땅에 안닿은 처리
            //슬라이드는 대시와 다르게 땅에 닿은 처리를 해야 함
            //if (forceNotGrounded) CharacterActor.alwaysNotGrounded = true;

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
            JumpProgressDirection = CharacterActor.Up;
            JumpDirection = CharacterActor.Forward;

            //ResetDash(); <<===================================================수정해야 할지도, 점프 이전의 가속도 영향을 받을지 말지 결정
            Debug.Log($"{CharacterActor.Velocity.x} , {CharacterActor.Velocity.y}, {CharacterActor.Velocity.z}");

            //wasd 입력으로 벡터 생성. w만 누르면 x1, s누르면 x-1, a,d도 y값을 동일하게 조정
            Vector3 newJumpDir = 
                CharacterActor.Right * Input.GetAxisRaw("Movement X") + 
                CharacterActor.Forward * Input.GetAxisRaw("Movement Y");
            Debug.Log(newJumpDir);

            CharacterActor.Velocity = CharacterActor.Velocity * BehindExitRoll_MultipleRollVelocity + newJumpDir * BehindExitRoll_JumpDirForce;
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
        public override void UpdateBehaviour(float dt)
        {
            //기존의 dashVelocity  코드
            //Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * upforceCurve.Evaluate(SuperJumpCursor) * JumpProgressDirection;
            //===개선한 점프 방향 코드
            Vector3 JumpVelocity = initialUpVelocity * currentSpeedMultiplier * upforceCurve.Evaluate(SuperJumpCursor) * JumpProgressDirection;
            

            //기존의 Velocity 고정 코드
            //CharacterActor.Velocity = dashVelocity;

            //===개선한 중력 적용 코드===
            //CharacterActor.PlanarVelocity = initialForwardVelocity * currentSpeedMultiplier * JumpDirection; // + 마우스 방향 정면 * VelocityForceToMouseDirection
            //CharacterActor.RotateYaw(); //마우스 방향으로 회전, rotateForceTomouseDirection = if(캐릭터 정면 기준으로 마우스 방향에 따라 + or - )
            if(JumpUp_currFrameCount <= JumpUp_FrameCount)
            {
                float temf = JumpUp_UpperPowerCurv.Evaluate(JumpUp_currFrameCount / JumpUp_FrameCount);
                //Debug.Log(temf);

                
                CharacterActor.Position = CharacterActor.Position + new Vector3(0, JumpUp_UpPower * temf, 0);
                JumpUp_currFrameCount++;
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
