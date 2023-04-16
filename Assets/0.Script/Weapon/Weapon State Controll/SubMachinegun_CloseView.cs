using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using UnityEngine;
using Urban_KimHyeonWoo;

public class SubMachinegun_CloseView : WeaponState
{
    [SerializeField]
    [Tooltip("캐릭터로부터 카메라의 거리")]
    Vector2 CloseZoomMinMax = new Vector2(0.7f, 0.8f);//<<줌minmax는 의미 없어졌으므로, 고정값으로 수정해야 함
    [SerializeField]
    [Tooltip("캐릭터를 기준으로 어깨 너머 카메라의 위치.")]
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
        /*
            if (CharacterActions.Fire2.value == true)
            {
                WeaponStateController.EnqueueTransition<Sniper_AimState>();
            }*/
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
        CharacterActor.Animator.SetLayerWeight(1, 1);

        WeaponStateController.ChangeWeaponPos_Hand();

        WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;
        WeaponStateController.Camera3D.OffsetFromHead = CloseViewOffsetValue;
        WeaponStateController.Camera3D.minZoom = CloseZoomMinMax.x;
        WeaponStateController.Camera3D.maxZoom = CloseZoomMinMax.y;
    }
    public override void ExitBehaviour(float dt, WeaponState toState)
    {
        base.ExitBehaviour(dt, toState);
    }
    public override void UpdateBehaviour(float dt)
    {
        if (CharacterActions.Fire1.value == true)
        {
            WeaponStateController.DoFire();
        }
    }
}
