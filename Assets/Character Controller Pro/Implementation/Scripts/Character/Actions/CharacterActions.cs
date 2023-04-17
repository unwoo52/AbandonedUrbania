namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// This struct contains all the inputs actions available for the character to interact with.
    /// </summary>
    [System.Serializable]
    public struct CharacterActions
    {

        // Bool actions
        public BoolAction @jump;
        public BoolAction @run;
        public BoolAction @interact;
        public BoolAction @jetPack;
        public BoolAction @dash;
        public BoolAction @crouch;
        //add
        public BoolAction Fire1;
        public BoolAction Fire2;
        public BoolAction Reload;


        // Float actions
        public FloatAction @pitch;
        public FloatAction @roll;
        //add
        public FloatAction Wheelupdown;


        // Vector2 actions
        public Vector2Action @movement;



        /// <summary>
        /// Reset all the actions.
        /// </summary>
        public void Reset()
        {
            @jump.Reset();
            @run.Reset();
            @interact.Reset();
            @jetPack.Reset();
            @dash.Reset();
            @crouch.Reset();
            //add
            Fire1.Reset();
            Fire2.Reset();
            Reload.Reset();

            @pitch.Reset();
            @roll.Reset();
            //add
            Wheelupdown.Reset();

            @movement.Reset();

        }

        /// <summary>
        /// Initializes all the actions by instantiate them. Each action will be instantiated with its specific type (Bool, Float or Vector2).
        /// </summary>
        public void InitializeActions()
        {
            @jump = new BoolAction();
            @jump.Initialize();

            @run = new BoolAction();
            @run.Initialize();

            @interact = new BoolAction();
            @interact.Initialize();

            @jetPack = new BoolAction();
            @jetPack.Initialize();

            @dash = new BoolAction();
            @dash.Initialize();

            @crouch = new BoolAction();
            @crouch.Initialize();

            //add
            Fire1 = new BoolAction();
            Fire1.Initialize();

            Fire2 = new BoolAction();
            Fire2.Initialize();

            Reload = new BoolAction();
            Reload.Initialize();


            @pitch = new FloatAction();
            @roll = new FloatAction();
            //add
            Wheelupdown = new FloatAction();

            @movement = new Vector2Action();

        }

        /// <summary>
        /// Updates the values of all the actions based on the current input handler (human).
        /// </summary>
        public void SetValues(InputHandler inputHandler)
        {
            if (inputHandler == null)
                return;

            @jump.value = inputHandler.GetBool("Jump");
            @run.value = inputHandler.GetBool("Run");
            @interact.value = inputHandler.GetBool("Interact");
            @jetPack.value = inputHandler.GetBool("Jet Pack");
            @dash.value = inputHandler.GetBool("Dash");
            @crouch.value = inputHandler.GetBool("Crouch");
            //add
            Fire1.value = inputHandler.GetBool("Fire1");
            Fire2.value = inputHandler.GetBool("Fire2");
            Reload.value = inputHandler.GetBool("Reload");


            @pitch.value = inputHandler.GetFloat("Pitch");
            @roll.value = inputHandler.GetFloat("Roll");
            //add
            Wheelupdown.value = inputHandler.GetFloat("Mouse ScrollWheel");

            @movement.value = inputHandler.GetVector2("Movement");

        }

        /// <summary>
        /// Copies the values of all the actions from an existing set of actions.
        /// </summary>
        public void SetValues(CharacterActions characterActions)
        {
            @jump.value = characterActions.jump.value;
            @run.value = characterActions.run.value;
            @interact.value = characterActions.interact.value;
            @jetPack.value = characterActions.jetPack.value;
            @dash.value = characterActions.dash.value;
            @crouch.value = characterActions.crouch.value;
            //add
            Fire1.value = characterActions.Fire1.value;
            Fire2.value = characterActions.Fire2.value;
            Reload.value = characterActions.Reload.value;

            @pitch.value = characterActions.pitch.value;
            @roll.value = characterActions.roll.value;
            //add
            Wheelupdown.value = characterActions.Wheelupdown.value;

            @pitch.value = characterActions.pitch.value;
            @roll.value = characterActions.roll.value;
            @movement.value = characterActions.movement.value;

        }

        /// <summary>
        /// Update all the actions internal states.
        /// </summary>
        public void Update(float dt)
        {
            @jump.Update(dt);
            @run.Update(dt);
            @interact.Update(dt);
            @jetPack.Update(dt);
            @dash.Update(dt);
            @crouch.Update(dt);
            //add
            Fire1.Update(dt);
            Fire2.Update(dt);
            Reload.Update(dt);
        }


    }


}