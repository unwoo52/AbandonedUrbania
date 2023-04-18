using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public interface ISetWeaponDisable
    {
        WeaponViews SetWeaponDisable();
    }
    public interface ISetWeaponsAble
    {
        void SetWeaponsAble(WeaponViews weaponViews);
    }
    public class Weapons_SwapManager : MonoBehaviour, IActionStateChangeListener
    {
        WeaponStateController currWeaponStateController;

        [SerializeField] List<GameObject> Slot;

        int currentSlot = 0; // 0 ~ 3
        [SerializeField] int InitSlot;

        private void Start()
        {
            currentSlot = InitSlot;
            //SetAbleSlot(InitSlot);
            if (Slot[currentSlot].TryGetComponent(out WeaponStateController weaponStateController))
            {
                currWeaponStateController = weaponStateController;
            }
        }

        void SetAbleSlot(int index)
        {
            if (index == currentSlot) return;
            
            WeaponViews views = default;
            if(Slot[currentSlot].TryGetComponent(out ISetWeaponDisable setWeaponDisable))
            {
                views = setWeaponDisable.SetWeaponDisable();
            }

            currentSlot = index;

            if (Slot[index].TryGetComponent(out ISetWeaponsAble setWeaponsAble))
            {
                setWeaponsAble.SetWeaponsAble(views);
            }
            if(Slot[index].TryGetComponent(out WeaponStateController weaponStateController))
            {
                currWeaponStateController = weaponStateController;
            }
        }

        private void Update()
        {
            /*
            if (Input.GetButtonDown("Slot0"))
            {
                SetAbleSlot(0);
            }
            else if (Input.GetButtonDown("slot1"))
            {
                SetAbleSlot(1);

            }
            else if (Input.GetButtonDown("slot2"))
            {
                SetAbleSlot(2);

            }
            else if (Input.GetButtonDown("slot3"))
            {
                SetAbleSlot(3);
            }*/
        }

        public void OnCharacterActionStateChanged(CharacterState state)
        {
            currWeaponStateController.Notify_CharacterActionHasChanged(state);
        }
    }
}

