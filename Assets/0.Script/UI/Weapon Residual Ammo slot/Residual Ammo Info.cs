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
            else Debug.LogError("���� ������Ʈ���� �ܿ� ź�� ������ ������ �������̽��� ã�� ���߽��ϴ�.");

            infoTextUI.text = residualAmmo.ToString() + " / " + totalAmmo.ToString();
        }
        #endregion
    }
}


