using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class Sniper_CloseView : WeaponState
{
    [SerializeField]
    [Tooltip("캐릭터로부터 카메라의 거리")]
    Vector2 CloseZoomMinMax = new Vector2(0.7f, 0.8f);//<<줌minmax는 의미 없어졌으므로, 고정값으로 수정해야 함
    [SerializeField]
    [Tooltip("캐릭터를 기준으로 어깨 너머 카메라의 위치.")]
    Vector3 CloseViewOffsetValue = new Vector3(0.3f, -0.22f, 0.27f);


    public float MouseWheel;
    public bool Fire2;
    private void Update()
    {
        MouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("<color=red>빨간색 로그</color>");
            Fire2 = true;
        }
        else Fire2= false;
    }
    public override void CheckExitTransition()
    {
        if (Fire2 == true)
        {
            WeaponStateController.EnqueueTransition<Sniper_AimState>();
        }
        if (MouseWheel < 0)
        {
            WeaponStateController.EnqueueTransition<Sniper_FarView>();
        }
    }
    public override void EnterBehaviour(float dt, WeaponState fromState)
    {
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
        base.UpdateBehaviour(dt);
    }
}
