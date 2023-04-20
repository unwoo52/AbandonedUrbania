using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Urban_KimHyeonWoop
{
    public interface ISetResidualAmmoUI
    {
        void SetResidualAmmoUI(int curammo);
    }
    public interface IGetWeaponInformation
    {
        void GetWeaponinformation(Urban_KimHyeonWoo.WeaponInfo weaponInfo, int currentAmmo);
    }
    public class WeaponInfo : MonoBehaviour, ISetResidualAmmoUI, IGetWeaponInformation
    {
        [SerializeField] RawImage weaponImage;
        [SerializeField] TMP_Text ammoCount;
        [SerializeField] TMP_Text totalAmmoCount;

        public void GetWeaponinformation(Urban_KimHyeonWoo.WeaponInfo weaponInfo, int currentAmmo)
        {
            weaponImage.texture = weaponInfo.WeaponImage;
            totalAmmoCount.text = weaponInfo.TotalAmmo.ToString();
            ammoCount.text = currentAmmo.ToString();
        }

        public void SetResidualAmmoUI(int curammo)
        {            
            ammoCount.text = curammo.ToString();
            if(curammo != 0)
            {
                ammoCount.color = Color.white;
            }
            else if(curammo == 0) 
            {                
                ammoCount.color = Color.red;
            }
        }

        private void Start()
        {
            if(weaponImage == null)
            {
                weaponImage = GetComponentInChildren<RawImage>();
            }
            if(ammoCount == null)
            {
                TMP_Text[] tMP_Texts = GetComponentsInChildren<TMP_Text>();
                foreach (var tmp in tMP_Texts)
                {
                    if (tmp.name == "CurrentAmmo")
                    {
                        ammoCount = tmp; 
                        break;
                    }
                        
                }
            }
            if(totalAmmoCount == null)
            {
                TMP_Text[] tMP_Texts = GetComponentsInChildren<TMP_Text>();
                foreach(var tmp in tMP_Texts) 
                {
                    if (tmp.name == "TotalAmmo")
                    {
                        totalAmmoCount = tmp; 
                        break;
                    }
                        
                }
            }
        }
    }
}

