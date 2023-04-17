using TMPro;
using UnityEngine;

namespace Urban_KimHyeonWoo 
{
    public interface IGetRisidualAmmo
    {
        void GetRisidualAmmo(ref int residualAmmo, ref int totalAmmo);
    }
    public class ResidualAmmoInfo : MonoBehaviour
    {
        [SerializeField] GameObject WeaponObject;
        [SerializeField] TMP_Text infoTextUI;

        int residualAmmo;
        int totalAmmo;
        #region public Method
        public void UpdateResidualInfo()
        {
            if (WeaponObject.TryGetComponent(out IGetRisidualAmmo getRisidualAmmo))
            {
                getRisidualAmmo.GetRisidualAmmo(ref residualAmmo, ref totalAmmo);
            }
            else Debug.LogError("무기 오브젝트에서 잔여 탄약 정보를 얻어오는 인터페이스를 찾지 못했습니다.");

            infoTextUI.text = residualAmmo.ToString() + " / " + totalAmmo.ToString();
        }
        #endregion
    }
}


