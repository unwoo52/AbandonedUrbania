using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using UnityEngine;
using Urban_KimHyeonWoo;

public class SubMachinegun_CloseView : WeaponState
{
    [Header("�ִϸ��̼� ���̾ ���õ� field")]
    [SerializeField] int UpperAimRunLayerNum = 1;
    float curUpperLayerValue = 0;

    [SerializeField]
    [Tooltip("ĳ���ͷκ��� ī�޶��� �Ÿ�")]
    Vector2 CloseZoomMinMax = new Vector2(0.7f, 0.8f);//<<��minmax�� �ǹ� ���������Ƿ�, ���������� �����ؾ� ��
    [SerializeField]
    [Tooltip("ĳ���͸� �������� ��� �ʸ� ī�޶��� ��ġ.")]
    Vector3 CloseViewOffsetValue = new Vector3(0.3f, -0.22f, 0.27f);

    #region EventTrigger
    bool setFarStateTrigger = false;
    public void SetFatState()
    {
        setFarStateTrigger = true;
    }
    #endregion

    #region unity Callbacks
    private void Start()
    {
        weaponViews = WeaponViews.Close;
    }
    #endregion

    public override void CheckExitTransition()
    {
        //adapter code
        if (isViewChangeLock()) return;

        if (CharacterActions.Wheelupdown.value < 0f)
        {
            WeaponStateController.EnqueueTransition<SubMachinegun_FarView>();
        }
        if(setFarStateTrigger == true)
        {
            setFarStateTrigger= false;
            WeaponStateController.EnqueueTransition<SubMachinegun_FarView>();
        }
    }
    public override void EnterBehaviour(float dt, WeaponState fromState)
    {
        CharacterActor.Animator.SetFloat("BlendFloat_WeaponType", 0);
        CharacterStateController.IsFixedLookdir = true;
        CharacterActor.Animator.SetLayerWeight(1, 1);

        WeaponStateController.ChangeWeaponPos_Hand();

        WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;
        WeaponStateController.Camera3D.OffsetFromHead = CloseViewOffsetValue;
        WeaponStateController.Camera3D.minZoom = CloseZoomMinMax.x;
        WeaponStateController.Camera3D.maxZoom = CloseZoomMinMax.y;
    }
    public override void ExitBehaviour(float dt, WeaponState toState)
    {
        CharacterStateController.IsFixedLookdir = false;
    }

    
    public override void UpdateBehaviour(float dt)
    {
        SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);
        if (CharacterStateController.CurrentState == CharacterStateController.GetState<SuperJump>())
        {
            weaponController.currBattleTime = 0.2f;
        }
    }


    #region Adapter
    //CharacterState Adapter
    bool SuperJumpLock = false;
    bool isViewChangeLock()
    {
        return SuperJumpLock;
    }
    public override void ActionStateChangeListener(CharacterState state)
    {
        if (state == CharacterStateController.GetState<SuperJump>())
        {
            SuperJumpLock = true;

            WeaponStateController.ForceState(this);
        }
        else if (state != CharacterStateController.GetState<SuperJump>() && SuperJumpLock == true)
        {//������������ ���������ٸ�,
            SuperJumpLock = false;

            WeaponStateController.ForceState(WeaponStateController.PreviousState);
        }
    }
    #endregion
}
