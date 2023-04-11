using Lightbug.CharacterControllerPro.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public interface IBindPlayer
    {
        void BindPlayer(bool setvalue);
    }
    public class CharacterControll : MonoBehaviour, IBindPlayer
    {
        [SerializeField] Animator animator;
        [SerializeField] float bulletMaxDistance;
        [SerializeField] Camera3D camera3D;
        [SerializeField] GameObject Scope;

        public void BindPlayer(bool setvalue)
        {
            Transform actionsObject = transform.Find("Actions");
            if (actionsObject != null)
            {
                actionsObject.gameObject.SetActive(!setvalue);
            }
            else Debug.LogError("cannot find \"Actions\" Object!");
        }

        void Update()
        {
        }
    }
}

