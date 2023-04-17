using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using UnityEngine;
using Urban_KimHyeonWoo;

public class SubMachinegun_CloseView : WeaponState
{
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

    public override void CheckExitTransition()
    {
        
        if (Input.GetButtonDown("Fire2"))
        {
            WeaponStateController.EnqueueTransition<SubMachinegun_AimView>();
        }
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
        if (CharacterActions.Fire1.value == true)
        {
            WeaponStateController.DoFire();
        }
    }
}