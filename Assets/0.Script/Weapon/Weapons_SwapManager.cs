using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoop;

namespace Urban_KimHyeonWoo
{
    public interface ISetWeaponDisable
    {
        WeaponStateType SetWeaponDisable();
    }
    public interface ISetWeaponsAble
    {
        void SetWeaponsAble(WeaponStateType weaponViews);
    }
    public class Weapons_SwapManager : MonoBehaviour, IActionStateChangeListener, IGetReloadTime
    {
        [SerializeField]
        [Tooltip("시작할 때 view 상태")] WeaponStateType InitViewType = WeaponStateType.Far;
        WeaponStateController currWeaponStateController;

        [SerializeField] List<GameObject> Slot;

        int currentSlot = 0; // 0 ~ 3
        [SerializeField] int InitSlot;

        private void Start()
        {
            currentSlot = InitSlot;
            if (InitViewType == default) InitViewType = WeaponStateType.Far;

            
            //get WeaponStateController
            if (Slot[currentSlot].TryGetComponent(out WeaponStateController weaponStateController))
            {
                currWeaponStateController = weaponStateController;
            }
        }


        void SwapWeapon(int index)
        {
            if (index == currentSlot) return;

            GameObject previousWeapon = Slot[currentSlot];
            GameObject currentWeapon = Slot[index];
            
            WeaponStateType views = default;

            if(previousWeapon.TryGetComponent(out ISetWeaponDisable setWeaponDisable))
            {
                views = setWeaponDisable.SetWeaponDisable();
            }

            if (views == WeaponStateType.Disable) views = WeaponStateType.Far;

            if (currentWeapon.TryGetComponent(out ISetWeaponsAble setWeaponsAble))
            {
                setWeaponsAble.SetWeaponsAble(views);
            }
            if(currentWeapon.TryGetComponent(out WeaponStateController weaponStateController))
            {
                currWeaponStateController = weaponStateController;
            }

            Adapter_NotifyActionStateChangeListeners(index);

            currentSlot = index;
        }
        bool isInitWeapon = false;
        private void Update()
        {
            if(!isInitWeapon)
            {
                if (Slot[currentSlot].TryGetComponent(out ISetWeaponsAble setWeaponsAble))
                {
                    setWeaponsAble.SetWeaponsAble(InitViewType);
                }
                Adapter_NotifyActionStateChangeListeners(currentSlot);
                isInitWeapon = true;
            }

            if (Input.GetButtonDown("WeaponSlot 1"))
            {
                SwapWeapon(0);
            }
            else if (Input.GetButtonDown("WeaponSlot 2"))
            {
                SwapWeapon(1);

            }
            else if (Input.GetButtonDown("WeaponSlot 3"))
            {
                SwapWeapon(2);

            }
            else if (Input.GetButtonDown("WeaponSlot 4"))
            {
                SwapWeapon(3);
            }
        }

        #region 

        public void CallOnExitReload()
        {
            if(currWeaponStateController.TryGetComponent(out WeaponController weaponController))
            {
                weaponController.OnExitReload();
            }
        }

        #endregion

        public void OnCharacterActionStateChanged(CharacterState state)
        {
            currWeaponStateController.Notify_CharacterActionHasChanged(state);
        }

        #region Adapter
        //WeaponState Adapter

        [Header("Adapter Field")]
        [SerializeField] GameObject WeaponInfoUI;

        void Adapter_NotifyActionStateChangeListeners(int Index)
        {
            if (WeaponInfoUI == null)
            {
                Debug.LogWarning("Weapon Slot이 변경된 사실을 Notify할 대상 오브젝트가 없습니다.");
                return;
            }

            //adapter code
            if (Slot[Index].TryGetComponent(out ISetWeaponInfoUI setWeaponInfoUI ))
            {
                //interface codes...
                setWeaponInfoUI.SetWeaponInfoUI(WeaponInfoUI);
            }
            else
            {
                Debug.LogWarning("Weapon Slot이 변경된 사실을 Notify할 대상 오브젝트에 Listener 인터페이스가 없습니다.");
                return;
            }
        }

        public float GetReloadTime()
        {
            if(Slot[currentSlot].TryGetComponent(out WeaponController weaponController))
            {
                return weaponController.WeaponInfo.ReloadTime;
            }

            return -1;
        }
        #endregion
    }
}

