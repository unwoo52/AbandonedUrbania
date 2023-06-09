using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;
using Urban_KimHyeonWoo;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;

namespace Lightbug.CharacterControllerPro.Demo
{
    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Normal Movement")]
    public class NormalMovement : CharacterState
    {
        //====My Code====//

        [SerializeField] Camera3D cam3d;




        //-----------------------
        [Space(10)]

        public PlanarMovementParameters planarMovementParameters = new PlanarMovementParameters();

        public VerticalMovementParameters verticalMovementParameters = new VerticalMovementParameters();

        public CrouchParameters crouchParameters = new CrouchParameters();

        public LookingDirectionParameters lookingDirectionParameters = new LookingDirectionParameters();


        [Header("Animation")]

        [SerializeField]
        protected string groundedParameter = "Grounded";

        [SerializeField]
        protected string stableParameter = "Stable";

        [SerializeField]
        protected string verticalSpeedParameter = "VerticalSpeed";

        [SerializeField]
        protected string planarSpeedParameter = "PlanarSpeed";

        [SerializeField]
        protected string horizontalAxisParameter = "HorizontalAxis";

        [SerializeField]
        protected string verticalAxisParameter = "VerticalAxis";

        [SerializeField]
        protected string heightParameter = "Height";


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        #region Events	

        /// <summary>
        /// Event triggered when the character jumps.
        /// </summary>
        public event System.Action OnJumpPerformed;

        /// <summary>
        /// Event triggered when the character jumps from the ground.
        /// </summary>
        public event System.Action<bool> OnGroundedJumpPerformed;

        /// <summary>
        /// Event triggered when the character jumps while.
        /// </summary>
        public event System.Action<int> OnNotGroundedJumpPerformed;

        #endregion


        protected MaterialController materialController = null;
        protected int notGroundedJumpsLeft = 0;
        protected bool isAllowedToCancelJump = false;
        protected bool wantToRun = false;
        protected float currentPlanarSpeedLimit = 0f;

        protected bool groundedJumpAvailable = true;
        protected Vector3 jumpDirection = default(Vector3);

        protected Vector3 targetLookingDirection = default(Vector3);
        protected float targetHeight = 1f;

        protected bool wantToCrouch = false;
        protected bool isCrouched = false;

        protected PlanarMovementParameters.PlanarMovementProperties currentMotion = new PlanarMovementParameters.PlanarMovementProperties();
        bool reducedAirControlFlag = false;
        float reducedAirControlInitialTime = 0f;
        float reductionDuration = 0.5f;


        protected override void Awake()
        {
            base.Awake();

            notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;

            materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();
        }

        protected virtual void OnValidate()
        {
            verticalMovementParameters.OnValidate();
        }

        protected override void Start()
        {
            base.Start();

            targetHeight = CharacterActor.DefaultBodySize.y;

            float minCrouchHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
            crouchParameters.heightRatio = Mathf.Max(minCrouchHeightRatio, crouchParameters.heightRatio);
        }

        protected virtual void OnEnable()
        {
            CharacterActor.OnTeleport += OnTeleport;
        }

        protected virtual void OnDisable()
        {
            CharacterActor.OnTeleport -= OnTeleport;
        }

        public override string GetInfo()
        {
            return "This state serves as a multi purpose movement based state. It is responsible for handling gravity and jump, walk and run, crouch, " +
            "react to the different material properties, etc. Basically it covers all the common movements involved " +
            "in a typical game, from a 3D platformer to a first person walking simulator.";
        }

        void OnTeleport(Vector3 position, Quaternion rotation)
        {
            targetLookingDirection = CharacterActor.Forward;
            isAllowedToCancelJump = false;
        }

        /// <summary>
        /// Gets/Sets the useGravity toggle. Use this property to enable/disable the effect of gravity on the character.
        /// </summary>
        /// <value></value>
        public bool UseGravity
        {
            get => verticalMovementParameters.useGravity;
            set => verticalMovementParameters.useGravity = value;
        }

        public override void CheckExitTransition()
        {
            //rolling
            if (CharacterActions.jetPack.value)
            {
                CharacterStateController.EnqueueTransition<JetPack>();
            }
            //slide, wantrun이 트루이고 shift를 눌렀을 때,
            else if (wantToRun && CharacterActions.dash.Started)
            {
                CharacterStateController.EnqueueTransition<Slide>();
            }
            else if (CharacterActor.Triggers.Count != 0)
            {
                CharacterStateController.EnqueueTransition<LadderClimbing>();
                CharacterStateController.EnqueueTransition<RopeClimbing>();
            }
            else if (!CharacterActor.IsGrounded)
            {
                if (!CharacterActions.crouch.value)
                    CharacterStateController.EnqueueTransition<WallSlide>();

                CharacterStateController.EnqueueTransition<LedgeHanging>();
            }
        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            reducedAirControlFlag = false;
        }



        /// <summary>
        /// Reduces the amount of acceleration and deceleration (not grounded state) until the character reaches the apex of the jump 
        /// (vertical velocity close to zero). This can be useful to prevent the character from accelerating/decelerating too quickly (e.g. right after performing a wall jump).
        /// </summary>
        public void ReduceAirControl(float reductionDuration = 0.5f)
        {
            reducedAirControlFlag = true;
            reducedAirControlInitialTime = Time.time;
            this.reductionDuration = reductionDuration;
        }

        void SetMotionValues(Vector3 targetPlanarVelocity)
        {
            float angleCurrentTargetVelocity = Vector3.Angle(CharacterActor.PlanarVelocity, targetPlanarVelocity);

            switch (CharacterActor.CurrentState)
            {
                case CharacterActorState.StableGrounded:

                    currentMotion.acceleration = planarMovementParameters.stableGroundedAcceleration;
                    currentMotion.deceleration = planarMovementParameters.stableGroundedDeceleration;
                    currentMotion.angleAccelerationMultiplier = planarMovementParameters.stableGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                    break;

                case CharacterActorState.UnstableGrounded:
                    currentMotion.acceleration = planarMovementParameters.unstableGroundedAcceleration;
                    currentMotion.deceleration = planarMovementParameters.unstableGroundedDeceleration;
                    currentMotion.angleAccelerationMultiplier = planarMovementParameters.unstableGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                    break;

                case CharacterActorState.NotGrounded:

                    if (reducedAirControlFlag)
                    {
                        float time = Time.time - reducedAirControlInitialTime;
                        if (time <= reductionDuration)
                        {
                            currentMotion.acceleration = (planarMovementParameters.notGroundedAcceleration / reductionDuration) * time;
                            currentMotion.deceleration = (planarMovementParameters.notGroundedDeceleration / reductionDuration) * time;
                        }
                        else
                        {
                            reducedAirControlFlag = false;

                            currentMotion.acceleration = planarMovementParameters.notGroundedAcceleration;
                            currentMotion.deceleration = planarMovementParameters.notGroundedDeceleration;
                        }

                    }
                    else
                    {
                        currentMotion.acceleration = planarMovementParameters.notGroundedAcceleration;
                        currentMotion.deceleration = planarMovementParameters.notGroundedDeceleration;
                    }

                    currentMotion.angleAccelerationMultiplier = planarMovementParameters.notGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                    break;

            }


            // Material values
            if (materialController != null)
            {
                if (CharacterActor.IsGrounded)
                {
                    currentMotion.acceleration *= materialController.CurrentSurface.accelerationMultiplier * materialController.CurrentVolume.accelerationMultiplier;
                    currentMotion.deceleration *= materialController.CurrentSurface.decelerationMultiplier * materialController.CurrentVolume.decelerationMultiplier;
                }
                else
                {
                    currentMotion.acceleration *= materialController.CurrentVolume.accelerationMultiplier;
                    currentMotion.deceleration *= materialController.CurrentVolume.decelerationMultiplier;
                }
            }

        }

        private void FixedUpdate()
        {
            //mycode
            if (CharacterStateController.IsFixedLookdir)
            {
                Vector3 mouseDir = new Vector3(CharacterActor.CurCam.transform.forward.x, 0, CharacterActor.CurCam.transform.forward.z);
                //CharacterActor.SetRotation(mouseDir, CharacterActor.Up);
                CharacterActor.SetYaw(mouseDir);

                //Quaternion mouseRot = new Quaternion(CharacterActor.CurCam.transform.forward.x, 0, CharacterActor.CurCam.transform.forward.z,0);
                //CharacterActor.Rotation = mouseRot;
            }
            //====
        }

        /// <summary>
        /// Processes the lateral movement of the character (stable and unstable state), that is, walk, run, crouch, etc. 
        /// This movement is tied directly to the "movement" character action.
        /// </summary>
        protected virtual void ProcessPlanarMovement(float dt)
        {
            

            float speedMultiplier = materialController != null ?
            materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier : 1f;


            bool needToAccelerate = CustomUtilities.Multiply(
                CharacterStateController.InputMovementReference, currentPlanarSpeedLimit).sqrMagnitude
                    >= 
                CharacterActor.PlanarVelocity.sqrMagnitude;

            Vector3 targetPlanarVelocity = default;
            switch (CharacterActor.CurrentState)
            {
                case CharacterActorState.NotGrounded:

                    if (CharacterActor.WasGrounded)
                        currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);


                    //needToAccelerate = CustomUtilities.Multiply(CharacterStateController.InputMovementReference, currentPlanarSpeedLimit).sqrMagnitude >= CharacterActor.PlanarVelocity.sqrMagnitude;
                    targetPlanarVelocity = CustomUtilities.Multiply(CharacterStateController.InputMovementReference, speedMultiplier, currentPlanarSpeedLimit);

                    //GetAccelerationBoost(targetPlanarVelocity)
                    break;
                case CharacterActorState.StableGrounded:

                    
                    // Run ------------------------------------------------------------
                    if(cam3d != null)
                    {
                        if (cam3d.CurrentDistanceToTarget > 1)//카메라 줌이 멀면
                        {
                            wantToRun = true;                            
                        }
                        else
                        {
                            wantToRun = false;
                            //캐릭터 방향 고정 코드                                                   
                        }
                    }
                    
                    /*
                    if (planarMovementParameters.runInputMode == InputMode.Toggle)
                    {
                        if (CharacterActions.run.Started)
                            //if toggle mode && run button has down, run != run.
                            //누르면 고정되는 모드일 때 실행되는 코드
                            wantToRun = !wantToRun;
                    }
                    else
                    {
                        //계속 눌러야 하는 모드일 때 실행되는 코드
                        //if hold mode
                            //run == 
                            wantToRun = CharacterActions.run.value;
                    }
                    */
                    if (wantToCrouch || !planarMovementParameters.canRun)
                        wantToRun = false;


                    if (isCrouched)
                    {
                        currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit * crouchParameters.speedMultiplier;
                    }
                    else
                    {
                        currentPlanarSpeedLimit = wantToRun ? planarMovementParameters.boostSpeedLimit : planarMovementParameters.baseSpeedLimit;
                    }

                    targetPlanarVelocity = CustomUtilities.Multiply(CharacterStateController.InputMovementReference, speedMultiplier, currentPlanarSpeedLimit);

                    //needToAccelerate = CharacterStateController.InputMovementReference != Vector3.zero;


                    break;
                case CharacterActorState.UnstableGrounded:

                    currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit;

                    //needToAccelerate = CustomUtilities.Multiply(CharacterStateController.InputMovementReference, currentPlanarSpeedLimit).sqrMagnitude >= CharacterActor.PlanarVelocity.sqrMagnitude;
                    targetPlanarVelocity = CustomUtilities.Multiply(CharacterStateController.InputMovementReference, speedMultiplier, currentPlanarSpeedLimit);

                    break;
            }

            SetMotionValues(targetPlanarVelocity);

            

            float acceleration = currentMotion.acceleration;
            if (needToAccelerate) //가속할 필요가 있다면
            {
                acceleration *= currentMotion.angleAccelerationMultiplier;
                //angleAccelerationMultiplier는
                //캐릭터의 현재 방향과 목표 방향 사이의 각도를 고려한 가속도 강화 요소입니다.
                //즉, 캐릭터가 현재 이동 방향과 목표 이동 방향이 같은 방향으로
                //있을 때 가속도를 더 강하게 해주는 요소입니다.

                // Affect acceleration based on the angle between target velocity and current velocity
                //float angleCurrentTargetVelocity = Vector3.Angle(CharacterActor.PlanarVelocity, targetPlanarVelocity);
                //float accelerationBoost = 20f * (angleCurrentTargetVelocity / 180f);
                //acceleration += accelerationBoost;
            }
            else
            {
                acceleration = currentMotion.deceleration;
                //deceleration는
                //캐릭터가 이전에 가속해야 했던 상황에서 현재는 감속해야 하는 상황이
                //되었을 때, 즉 이동 방향이 변경되는 상황에서 사용됩니다.
            }

            //최종 계산된 값을 PlanarVelocity에 대입
            CharacterActor.PlanarVelocity = Vector3.MoveTowards(
                CharacterActor.PlanarVelocity,
                targetPlanarVelocity,
                acceleration * dt
            );
        }
        [Header("IK")]
        [SerializeField]Transform LeftHand;
        public override void UpdateIK(int layerIndex)
        {
            /*
            if (CharacterActor.IsGrounded)
            {
                // Set the weight
                CharacterActor.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

                // Set the position
                CharacterActor.Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
            }
            */
        }

        protected virtual void ProcessGravity(float dt)
        {
            //useGravity가 fail이면 리턴
            if (!verticalMovementParameters.useGravity)
                return;

            verticalMovementParameters.UpdateParameters();


            float gravityMultiplier = 1f;
            
            //질감(얼음 진흙 등 위이면) 값 계산
            if (materialController != null)
                gravityMultiplier = CharacterActor.LocalVelocity.y >= 0 ?
                    materialController.CurrentVolume.gravityAscendingMultiplier :
                    materialController.CurrentVolume.gravityDescendingMultiplier;

            float gravity = gravityMultiplier * verticalMovementParameters.gravity;


            if (!CharacterActor.IsStable)
                CharacterActor.VerticalVelocity += CustomUtilities.Multiply(-CharacterActor.Up, gravity, dt);


        }


        protected bool UnstableGroundedJumpAvailable => !verticalMovementParameters.canJumpOnUnstableGround && CharacterActor.CurrentState == CharacterActorState.UnstableGrounded;



        public enum JumpResult
        {
            Invalid,
            Grounded,
            NotGrounded
        }

        JumpResult CanJump()
        {
            JumpResult jumpResult = JumpResult.Invalid;

            //canJump가 활성화 되어있으면              점프 X
            if (!verticalMovementParameters.canJump)
                return jumpResult;

            //웅크리고 있으면                          점프 X
            if (isCrouched)
                return jumpResult;


            switch (CharacterActor.CurrentState)
            {
                case CharacterActorState.StableGrounded:
                    //캐릭터가 지면에 닿은 뒤 바로 점프할 수는 없고 preGroundedJumpTime가 지나야 점프할 수 있음
                    //preGroundedJumpTime가 지났는지를 체크해서, 지났으면 JumpResult          점프 O
                    if (CharacterActions.jump.StartedElapsedTime <= verticalMovementParameters.preGroundedJumpTime && groundedJumpAvailable)
                        jumpResult = JumpResult.Grounded;

                    break;
                case CharacterActorState.NotGrounded:

                    if (CharacterActions.jump.Started)
                    {
                        // First check if the "grounded jump" is available. If so, execute a "coyote jump".
                        if (CharacterActor.NotGroundedTime <= verticalMovementParameters.postGroundedJumpTime && groundedJumpAvailable)
                        {
                            jumpResult = JumpResult.Grounded;
                        }
                        else if (notGroundedJumpsLeft != 0)  // Do a not grounded jump
                        {
                            jumpResult = JumpResult.NotGrounded;
                        }
                    }

                    break;
                case CharacterActorState.UnstableGrounded:

                    if (CharacterActions.jump.StartedElapsedTime <= verticalMovementParameters.preGroundedJumpTime && verticalMovementParameters.canJumpOnUnstableGround)
                        jumpResult = JumpResult.Grounded;

                    break;
            }

            return jumpResult;
        }



        protected virtual void ProcessJump(float dt)
        {
            ProcessRegularJump(dt);
            ProcessJumpDown(dt);
        }

        #region JumpDown

        protected virtual bool ProcessJumpDown(float dt)
        {
            if (!verticalMovementParameters.canJumpDown)
                return false;

            if (!CharacterActor.IsStable)
                return false;

            if (!CharacterActor.IsGroundAOneWayPlatform)
                return false;

            if (verticalMovementParameters.filterByTag)
            {
                if (!CharacterActor.GroundObject.CompareTag(verticalMovementParameters.jumpDownTag))
                    return false;
            }

            if (!ProcessJumpDownAction())
                return false;

            JumpDown(dt);

            return true;
        }


        protected virtual bool ProcessJumpDownAction()
        {
            return isCrouched && CharacterActions.jump.Started;
        }


        protected virtual void JumpDown(float dt)
        {

            float groundDisplacementExtraDistance = 0f;

            Vector3 groundDisplacement = CustomUtilities.Multiply(CharacterActor.GroundVelocity, dt);
            // bool  CharacterActor.transform.InverseTransformVectorUnscaled( Vector3.Project( groundDisplacement , CharacterActor.Up ) ).y

            if (!CharacterActor.IsGroundAscending)
                groundDisplacementExtraDistance = groundDisplacement.magnitude;

            CharacterActor.ForceNotGrounded();

            CharacterActor.Position -=
                CustomUtilities.Multiply(
                    CharacterActor.Up,
                    CharacterConstants.ColliderMinBottomOffset + verticalMovementParameters.jumpDownDistance + groundDisplacementExtraDistance
                );

            CharacterActor.VerticalVelocity -= CustomUtilities.Multiply(CharacterActor.Up, verticalMovementParameters.jumpDownVerticalVelocity);
        }

        #endregion

        #region Jump

        protected virtual void ProcessRegularJump(float dt)
        {
            if (CharacterActor.IsGrounded)
            {
                notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;

                groundedJumpAvailable = true;
            }


            if (isAllowedToCancelJump)
            {
                if (verticalMovementParameters.cancelJumpOnRelease)
                {
                    if (CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMaxTime || CharacterActor.IsFalling)
                    {
                        isAllowedToCancelJump = false;
                    }
                    else if (!CharacterActions.jump.value && CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMinTime)
                    {
                        // Get the velocity mapped onto the current jump direction
                        Vector3 projectedJumpVelocity = Vector3.Project(CharacterActor.Velocity, jumpDirection);

                        CharacterActor.Velocity -= CustomUtilities.Multiply(projectedJumpVelocity, 1f - verticalMovementParameters.cancelJumpMultiplier);

                        isAllowedToCancelJump = false;
                    }
                }
            }
            else
            {
                JumpResult jumpResult = CanJump();

                switch (jumpResult)
                {
                    case JumpResult.Grounded:
                        groundedJumpAvailable = false;

                        break;
                    case JumpResult.NotGrounded:
                        notGroundedJumpsLeft--;

                        break;

                    case JumpResult.Invalid:
                        return;
                }

                // Events ---------------------------------------------------
                if (CharacterActor.IsGrounded)
                {

                    if (OnGroundedJumpPerformed != null)
                        OnGroundedJumpPerformed(true);
                }
                else
                {
                    if (OnNotGroundedJumpPerformed != null)
                        OnNotGroundedJumpPerformed(notGroundedJumpsLeft);
                }

                if (OnJumpPerformed != null)
                    OnJumpPerformed();


                // Force "not grounded" state.     
                if (CharacterActor.IsGrounded)
                    CharacterActor.ForceNotGrounded();

                // First remove any velocity associated with the jump direction.
                CharacterActor.Velocity -= Vector3.Project(CharacterActor.Velocity, CharacterActor.Up);
                CharacterActor.Velocity += CustomUtilities.Multiply(CharacterActor.Up, verticalMovementParameters.jumpSpeed);

                if (verticalMovementParameters.cancelJumpOnRelease)
                    isAllowedToCancelJump = true;

            }


        }

        #endregion


        void ProcessVerticalMovement(float dt)
        {
            ProcessGravity(dt);
            ProcessJump(dt);
        }


        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            CharacterActor.alwaysNotGrounded = false;

            targetLookingDirection = CharacterActor.Forward;

            if (fromState == CharacterStateController.GetState<WallSlide>())
            {
                // "availableNotGroundedJumps + 1" because the update code will consume one jump!
                notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps + 1;

                // Reduce the amount of air control (acceleration and deceleration) for 0.5 seconds.
                ReduceAirControl(0.5f);
            }

            currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);

            CharacterActor.UseRootMotion = false;
        }

        protected virtual void HandleRotation(float dt)
        {
            HandleLookingDirection(dt);
        }

        void HandleLookingDirection(float dt)
        {
            if (!lookingDirectionParameters.changeLookingDirection)
                return;

            switch (lookingDirectionParameters.lookingDirectionMode)
            {
                case LookingDirectionParameters.LookingDirectionMode.Movement:

                    switch (CharacterActor.CurrentState)
                    {
                        case CharacterActorState.NotGrounded:

                            SetTargetLookingDirection(lookingDirectionParameters.notGroundedLookingDirectionMode);

                            break;
                        case CharacterActorState.StableGrounded:

                            SetTargetLookingDirection(lookingDirectionParameters.stableGroundedLookingDirectionMode);

                            break;
                        case CharacterActorState.UnstableGrounded:

                            SetTargetLookingDirection(lookingDirectionParameters.unstableGroundedLookingDirectionMode);

                            break;
                    }

                    break;

                case LookingDirectionParameters.LookingDirectionMode.ExternalReference:

                    if (!CharacterActor.CharacterBody.Is2D)
                        targetLookingDirection = CharacterStateController.MovementReferenceForward;

                    break;

                case LookingDirectionParameters.LookingDirectionMode.Target:

                    targetLookingDirection = (lookingDirectionParameters.target.position - CharacterActor.Position);
                    targetLookingDirection.Normalize();

                    break;
            }

            Quaternion targetDeltaRotation = Quaternion.FromToRotation(CharacterActor.Forward, targetLookingDirection);
            Quaternion currentDeltaRotation = Quaternion.Slerp(Quaternion.identity, targetDeltaRotation, lookingDirectionParameters.speed * dt);

            if (CharacterActor.CharacterBody.Is2D)
                CharacterActor.SetYaw(targetLookingDirection);
            else
                CharacterActor.SetYaw(currentDeltaRotation * CharacterActor.Forward);
        }

        void SetTargetLookingDirection(LookingDirectionParameters.LookingDirectionMovementSource lookingDirectionMode)
        {
            if (lookingDirectionMode == LookingDirectionParameters.LookingDirectionMovementSource.Input)
            {
                if (CharacterStateController.InputMovementReference != Vector3.zero)
                    targetLookingDirection = CharacterStateController.InputMovementReference;
                else
                    targetLookingDirection = CharacterActor.Forward;
            }
            else
            {
                if (CharacterActor.PlanarVelocity != Vector3.zero)
                    targetLookingDirection = Vector3.ProjectOnPlane(CharacterActor.PlanarVelocity, CharacterActor.Up);
                else
                    targetLookingDirection = CharacterActor.Forward;
            }
        }

        public override void UpdateBehaviour(float dt)
        {
            HandleSize(dt);
            HandleVelocity(dt);
            HandleRotation(dt);
        }


        public override void PreCharacterSimulation(float dt)
        {
            // Pre/PostCharacterSimulation methods are useful to update all the Animator parameters. 
            // Why? Because the CharacterActor component will end up modifying the velocity of the actor.
            if (!CharacterActor.IsAnimatorValid())
                return;

            CharacterStateController.Animator.SetBool(groundedParameter, CharacterActor.IsGrounded);
            CharacterStateController.Animator.SetBool(stableParameter, CharacterActor.IsStable);
            CharacterStateController.Animator.SetFloat(horizontalAxisParameter, CharacterActions.movement.value.x);
            CharacterStateController.Animator.SetFloat(verticalAxisParameter, CharacterActions.movement.value.y);
            CharacterStateController.Animator.SetFloat(heightParameter, CharacterActor.BodySize.y);
        }

        public override void PostCharacterSimulation(float dt)
        {
            // Pre/PostCharacterSimulation methods are useful to update all the Animator parameters. 
            // Why? Because the CharacterActor component will end up modifying the velocity of the actor.
            if (!CharacterActor.IsAnimatorValid())
                return;

            // Parameters associated with velocity are sent after the simulation.
            // The PostSimulationUpdate (CharacterActor) might update velocity once more (e.g. if a "bad step" has been detected).
            CharacterStateController.Animator.SetFloat(verticalSpeedParameter, CharacterActor.LocalVelocity.y);
            CharacterStateController.Animator.SetFloat(planarSpeedParameter, CharacterActor.PlanarVelocity.magnitude);
        }

        protected virtual void HandleSize(float dt)
        {
            // Get the crouch input state 
            if (crouchParameters.enableCrouch)
            {
                if (crouchParameters.inputMode == InputMode.Toggle)
                {
                    if (CharacterActions.crouch.Started)
                        wantToCrouch = !wantToCrouch;
                }
                else
                {
                    wantToCrouch = CharacterActions.crouch.value;
                }

                if (!crouchParameters.notGroundedCrouch && !CharacterActor.IsGrounded)
                    wantToCrouch = false;

                /*
                if (CharacterActor.IsGrounded && wantToRun)
                    wantToCrouch = false;
                */
            }
            else
            {
                wantToCrouch = false;
            }

            if (wantToCrouch)
                Crouch(dt);
            else
                StandUp(dt);
        }

        void Crouch(float dt)
        {
            CharacterActor.SizeReferenceType sizeReferenceType = CharacterActor.IsGrounded ?
                CharacterActor.SizeReferenceType.Bottom : crouchParameters.notGroundedReference;

            bool validSize = CharacterActor.CheckAndInterpolateHeight(
                CharacterActor.DefaultBodySize.y * crouchParameters.heightRatio,
                crouchParameters.sizeLerpSpeed * dt, sizeReferenceType);

            if (validSize)
                isCrouched = true;
        }

        void StandUp(float dt)
        {
            CharacterActor.SizeReferenceType sizeReferenceType = CharacterActor.IsGrounded ?
                CharacterActor.SizeReferenceType.Bottom : crouchParameters.notGroundedReference;

            bool validSize = CharacterActor.CheckAndInterpolateHeight(
                CharacterActor.DefaultBodySize.y,
                crouchParameters.sizeLerpSpeed * dt, sizeReferenceType);

            if (validSize)
                isCrouched = false;
        }


        protected virtual void HandleVelocity(float dt)
        {
            ProcessVerticalMovement(dt);
            ProcessPlanarMovement(dt);
        }
    }
}






