using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class Sniper_FarView : WeaponState
{
    [SerializeField]
    [Tooltip("캐릭터로부터 카메라의 거리")]
    Vector2 ZoomMinMax = new Vector2(5, 12);//<<줌minmax는 의미 없어졌으므로, 고정값으로 수정해야 함
    [SerializeField]
    [Tooltip("캐릭터를 기준으로 어깨 너머 카메라의 위치.")]
    Vector3 ViewOffsetValue = new Vector3(0.4f, -0.1f, 0);


    public float MouseWheel;
    public bool Fire2;
    private void Update()
    {
        MouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        Fire2 = Input.GetButtonDown("Fire2");
    }
    public override void CheckExitTransition()
    {
        if (Fire2 == true)
        {
            WeaponStateController.EnqueueTransition<Sniper_AimState>();
        }
        if (MouseWheel > 0)
        {
            WeaponStateController.EnqueueTransition<Sniper_CloseView>();
        }
    }
    public override void EnterBehaviour(float dt, WeaponState fromState)
    {
        WeaponStateController.Camera3D.cameraMode = Camera3D.CameraMode.ThirdPerson;
        WeaponStateController.Camera3D.OffsetFromHead = ViewOffsetValue;
        WeaponStateController.Camera3D.minZoom = ZoomMinMax.x;
        WeaponStateController.Camera3D.maxZoom = ZoomMinMax.y;
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
