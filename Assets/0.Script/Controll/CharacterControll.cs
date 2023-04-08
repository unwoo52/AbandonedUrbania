using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public class CharacterControll : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] float bulletMaxDistance;

        bool EnterRunView = false;
        void Update()
        {
            //EnterRunView = Input.GetKeyDown("EnterRunView");
        }

    }
}

