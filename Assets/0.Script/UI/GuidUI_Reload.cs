using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Urban_KimHyeonWoo
{
    public interface IGetReloadTime
    {
        float GetReloadTime();
    }
    public interface ICancelReload
    {
        void CancelReload();
    }
    public class GuidUI_Reload : MonoBehaviour, ICancelReload
    {
        [SerializeField] Image _filledIcon;
        [SerializeField] TMP_Text _guidText;
        [SerializeField] GameObject guidUIParent;
        Coroutine corRecord;
        float curReloadTiem;

        private void Start()
        {
            if(guidUIParent == null)
            {
                guidUIParent = transform.GetChild(0).gameObject;
            }
            if(_filledIcon == null)
            {
                _filledIcon = guidUIParent.GetComponentInChildren<Image>();
            }
            if(_guidText == null)
            {
                _guidText = guidUIParent.GetComponentInChildren<TMP_Text>();
            }
            guidUIParent.SetActive(false);
        }

        IEnumerator RecordTime(float totalReloadTime)
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime;
                _guidText.text = string.Format("{0:F2} sec \n Reload...", Mathf.Abs(time - totalReloadTime));
                _filledIcon.fillAmount = time/ totalReloadTime;
                yield return null;
            }
        }

        public void StartRecord()
        {
            if (!GetReloadTime())
            {
                Debug.LogError("Fail to Show Reload Guid UI !!");
            }


            guidUIParent.SetActive(true);
            if (corRecord != null) { StopCoroutine(corRecord); }
            corRecord = StartCoroutine(RecordTime(curReloadTiem));
        }

        public void EndRecode()
        {
            guidUIParent.SetActive(false);
            StopCoroutine(corRecord);
        }


        #region Adapter                         -----------
        [Header("Adapter Field")]
        [SerializeField] GameObject WeaponSwap;

        bool GetReloadTime()
        {
            if (WeaponSwap == null)
            {
                Debug.LogWarning("totalAmmo information을 받아올 WeaponSwap 오브젝트가 없습니다.");
                return false;
            }

            //adapter code
            if (WeaponSwap.TryGetComponent(out IGetReloadTime getReloadTime))
            {
                curReloadTiem = getReloadTime.GetReloadTime();
                if(curReloadTiem == -1)
                {
                    Debug.LogWarning("Reload Time을 받아오는데 실패했습니다.");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("WeaponSwap 오브젝트에 GetTotalAmmo 인터페이스가 없습니다.");
                return false;
            }

            return true;
        }

        public void CancelReload()
        {
            guidUIParent.SetActive(false);
            StopCoroutine(corRecord);
        }
        #endregion
    }
}

