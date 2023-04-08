using Lightbug.CharacterControllerPro.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public class CharacterControll : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] float bulletMaxDistance;
        [SerializeField] Camera3D camera3D;
        [SerializeField] GameObject Scope;
        bool EnterRunView = false;
        void Update()
        {
            //GetInput();
            //InPutBehaviour();
        }
        public void Zoom()
        {
            camera3D.ToggleCameraMode();
            EnterRunView = !EnterRunView;
            Scope.SetActive(EnterRunView);
        }
        void GetInput()
        {
            EnterRunView = Input.GetKey(KeyCode.Mouse1);
        }
        void InPutBehaviour()
        {
            EnterRun(EnterRunView);
        }
        void EnterRun(bool EnterRunView)
        {
            camera3D.cameraMode = EnterRunView ? Camera3D.CameraMode.FirstPerson : Camera3D.CameraMode.ThirdPerson;
            Scope.SetActive(EnterRunView);
        }


    }
}

