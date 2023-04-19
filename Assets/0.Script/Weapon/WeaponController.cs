using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using Urban_KimHyeonWoop;

namespace Urban_KimHyeonWoo
{
    public interface ISetWeaponInfoUI
    {
        void SetWeaponInfoUI(GameObject uiObject);
    }
    [Serializable]
    public class WeaponInfo
    {
        [SerializeField] Texture weaponImage;
        public Texture WeaponImage => weaponImage;

        [SerializeField] float reloadSpeed;
        public float ReloadTime => reloadSpeed;

        [SerializeField] int totalAmmo;
        public int TotalAmmo => totalAmmo;
    }
    //사격 장전 등의 상태를 조절하는 상태머신
    public class WeaponController : MonoBehaviour, ISetWeaponInfoUI
    {
        [Header("Weapon Info")]
        [SerializeField] WeaponInfo weaponInfo = new WeaponInfo();
        public WeaponInfo WeaponInfo => weaponInfo;

        int currentAmmo;
        public int CurrentAmmo => currentAmmo;

        [Header("Bullet Field")]
        //bullet fire system field
        [SerializeField][Tooltip("발사 쿨타임")] float bulletCooldown = 0.5f;
        private float currentBulletCooldown = 0f; // 현재 쿨타임
        [SerializeField][Tooltip("레이 조준 사거리")] float maxDistance = 1000f;

        [SerializeField] GameObject BulletPrefab; // 총알 프리팹
        AudioSource audioSource;
        [SerializeField] AudioClip FireSound;

        [SerializeField] GameObject SkillEffectObject;

        [Header("무기 관련 field")]
        [SerializeField] Transform muzzleTransform;
        [SerializeField] LayerMask hitableMask;

        [SerializeField] float skilltime = 1.2f;
        [SerializeField] int bulletcount = 30;


        [Header("사격 후 전투시간 (숨고르기 시간 개념)")]
        [SerializeField] float battletime = 0.2f;
        public float currBattleTime;

        Camera Cam = null;
        //reload
        bool isReload = false;
        public bool IsReload => isReload;
        Coroutine CorReload;
        public bool FireLock = false;
        float curReloadTime = 0;


        #region Character Controller Pro        -----------
        public CharacterActor CharacterActor { get; private set; }
        CharacterBrain CharacterBrain = null;
        public CharacterActions CharacterActions
        {
            get
            {
                return CharacterBrain == null ? new CharacterActions() : CharacterBrain.CharacterActions;
            }
        }
        #endregion
        
        #region Unity Callbacks                 -----------

        private void Awake()
        {
            audioSource = this.GetComponentInBranch<CharacterActor, AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("audioSource is null!!");
            }
            Cam = transform.parent.parent.parent.GetChild(0).GetComponent<Camera>();
            if (Cam == null)
            {
                Debug.LogError("Transform rig is Changed!!");
            }
            CharacterActor = this.GetComponentInBranch<CharacterActor>();
            CharacterBrain = this.GetComponentInBranch<CharacterActor, CharacterBrain>();
        }
        private void Start()
        {
            currentAmmo = weaponInfo.TotalAmmo;
        }
        private void Update()
        {
            float dt = Time.deltaTime;
            if (currBattleTime >= 0)
            {
                currBattleTime -= dt;
            }

            StateMachine(dt);

            // 총알 쿨타임 감소
            if (currentBulletCooldown > 0f)
            {
                currentBulletCooldown -= dt;
            }            
        }
        #endregion

        #region Weapon FSM                      -----------
        public enum WeaponFireState
        {
            CanFire, Reload, OnSkill, Disable
        }
        public WeaponFireState previousState = WeaponFireState.CanFire;
        public void ChangeWeaponFireState(WeaponFireState DestinyState)
        {
            if (previousState == DestinyState) return;

            if (!ExitStateBehavior(previousState, DestinyState)) return;

            if (!EnterStateBehavior(DestinyState, previousState)) return;

            previousState = DestinyState;
        }
        bool ExitStateBehavior(WeaponFireState previusState, WeaponFireState destinyState )
        {
            switch (previusState)
            {
                case WeaponFireState.CanFire:
                    break;
                case WeaponFireState.Reload:
                    //layer weight 설정
                    //애니메이션 실행을 위한 trigger 실행
                    isReload = false;
                    Adapter_SetResidualAmmo();
                    break;
                case WeaponFireState.OnSkill:
                    break;
                case WeaponFireState.Disable:
                    //이전 state가 disable이고, 총알이 0일 때 (총을 변경했는데, 변경한 총의 잔탄이 0이라면 changerState를 취소하고, reload로 changeState 실행
                    if(currentAmmo < 1 && destinyState != WeaponFireState.Reload)
                    {
                        ChangeWeaponFireState(WeaponFireState.Reload);
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
        bool EnterStateBehavior(WeaponFireState state, WeaponFireState previousState)
        {
            switch (state)
            {
                case WeaponFireState.CanFire:
                    break;
                case WeaponFireState.Reload:
                    CorAddReloadTimeField = StartCoroutine(AddReloadTimeField());
                    CharacterActor.Animator.SetFloat("Multipl_ReloadSpeed", 3.57f / weaponInfo.ReloadTime);
                    CharacterActor.Animator.SetTrigger("Trigger_Reload");
                    isReload = true;
                    break;
                case WeaponFireState.OnSkill:
                    break;
                case WeaponFireState.Disable:
                    if (previousState == WeaponFireState.Reload)
                    {
                        CancelReload();
                    }
                    //adatercode
                    Adapter_EmptyWeaponInfoUI();
                    break;
                default:
                    break;
            }
            return true;
        }
        void StateMachine(float dt)
        {
            switch (previousState)
            {
                case WeaponFireState.CanFire:
                    //fire
                    if (CharacterActions.Fire1.value == true)
                    {
                        OrderFire();
                    }                        
                    if (CharacterActions.Reload.value == true)
                    {
                        //if() 탄창이 꽉 차있지 않다면
                        ChangeWeaponFireState(WeaponFireState.Reload);
                    }
                    break;
                case WeaponFireState.Reload:
                    SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);
                    break;
                case WeaponFireState.OnSkill:
                    break;
                case WeaponFireState.Disable:
                    break;
                default:
                    break;
            }
        }

        void CancelReload()
        {
            //cancel animation
            CharacterActor.Animator.SetTrigger("Trigger_CancelReload");
            //cancel reload guid gui
            CencelReloadGuidUI();
        }
        Coroutine CorAddReloadTimeField;
        IEnumerator AddReloadTimeField()
        {
            float time = 0f;

            while (time < weaponInfo.ReloadTime)
            {
                time += Time.deltaTime;
                curReloadTime = time;
                yield return null;
            }
        }
        #endregion

        #region Anim Event                      -----------
        public void OnExitReload()
        {
            ChangeWeaponFireState(WeaponFireState.CanFire);
            StopCoroutine(CorAddReloadTimeField);
            if(curReloadTime > weaponInfo.ReloadTime * 0.8f)
                currentAmmo = weaponInfo.TotalAmmo;

            Adapter_SetResidualAmmo();
        }
        #endregion

        #region Weapon System                   -----------

        bool isSkillOn = false;
        public void SkillOn()
        {
            isSkillOn = true;
        }
        [SerializeField] float skillCooltime = 2f;
        public void OrderFire()
        {
            if (FireLock) return;
            //Bullet();
            if (isSkillOn)
            {
                StartCoroutine(ShootSkill(skilltime, bulletcount));
                currentBulletCooldown = skillCooltime;
                isSkillOn = false;
                return;
            }

            // 쿨타임이 아직 남았으면 발사하지 않음
            if (currentBulletCooldown > 0f) return;
            if (currentAmmo < 1) return;

            // 발사 쿨타임 초기화
            currentBulletCooldown = bulletCooldown;
            Bullet();
        }
        void Bullet()
        {
            currBattleTime = battletime;

            //
            currentAmmo--;
            //
            Adapter_SetResidualAmmo();

            //get ray
            Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitInfo;
            Vector3 hitPoint;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, hitableMask))
            {
                hitPoint = hitInfo.point;
                // 충돌한 지점의 위치를 사용하는 코드
            }
            else
            {
                hitPoint = ray.origin + ray.direction * maxDistance;
                // 레이의 사거리 끝 지점을 사용하는 코드
            }



            //audioSource.clip = FireSound;
            audioSource.Stop();
            audioSource.PlayOneShot(FireSound);

            // BulletPrefab을 instantiate하여 생성된 총알의 로테이션을 입력받은 ray를 바라보게 회전시킴
            GameObject bullet = Instantiate(BulletPrefab);
            bullet.transform.position = muzzleTransform.position;
            Vector3 direction = (hitPoint - muzzleTransform.position).normalized;
            bullet.transform.rotation = Quaternion.LookRotation(direction);


            //skill Effect
            GameObject effect = Instantiate(SkillEffectObject);
            effect.transform.position = hitPoint;
            effect.transform.rotation = Quaternion.LookRotation(direction) * new Quaternion(0, 270, 0, 0);

            if (currentAmmo < 1) ChangeWeaponFireState(WeaponFireState.Reload);
        }

        IEnumerator ShootSkill(float shootTime, int bulletCount)
        {
            // 발사 시간을 총알 개수로 나누어 코루틴 동안 발사 주기를 계산한다
            float interval = shootTime / bulletCount;

            // 총알 발사를 위해 bulletCount만큼 Bullet() 코루틴 함수를 실행한다
            for (int i = 0; i < bulletCount; i++)
            {
                Bullet(); // 총알 발사
                yield return new WaitForSeconds(interval); // 발사 주기만큼 대기
            }
        }
        #endregion

        #region set Animator LayerWeight Method                      -----------
        [Tooltip("캐릭터의 애니메이터에서 상반신을 통제하는 upper layer의 번호")]
        [SerializeField] int UpperAimRunLayerNum = 1;
        float curUpperLayerValue = 0;
        void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum, float destValue, float dt)
        {
            curLayerValue = Mathf.Lerp(curLayerValue, destValue, 20 * dt);
            CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
        }


        #endregion

        #region public method                   -----------
        /// <summary> 총을 발사한지 시간이 얼마 지나지 않았으면 true를 반환, 총을 쏘지 않은 평상시에는 false를 반환 </summary>
        public bool IsFiredRecontly()
        {
            return currBattleTime > 0;
        }
        #endregion

        #region Adapter                         -----------
        //WeaponState Adapter

        [Header("Adapter Field")]
        //IActionStateChangeListener 인터페이스를 갖고 있는 오브젝트
        [SerializeField] GameObject WeaponInfoUI;
        void Adapter_EmptyWeaponInfoUI()
        {
            WeaponInfoUI = null;
        }

        void Adapter_SetResidualAmmo()
        {            
            if (WeaponInfoUI == null)
            {
                Debug.LogWarning("Weapon Slot이 변경된 사실을 Notify할 대상 오브젝트가 없습니다.");
                return;
            }
                
            //adapter code
            if (WeaponInfoUI.TryGetComponent(out ISetResidualAmmoUI interfc))
            {
                interfc.SetResidualAmmoUI(currentAmmo);
            }
            else
            {
                Debug.LogWarning("Weapon Slot이 변경된 사실을 Notify할 대상 오브젝트에 Listener 인터페이스가 없습니다.");
                return;
            } 
        }

        public void SetWeaponInfoUI(GameObject uiObject)
        {
            WeaponInfoUI = uiObject;
            if(uiObject.TryGetComponent(out IGetWeaponInformation getWeaponInformation))
            {
                getWeaponInformation.GetWeaponinformation(weaponInfo, currentAmmo);
            }
        }


        GameObject ReloadGuidUI;
        void CencelReloadGuidUI()
        {
            ReloadGuidUI = CanvasManagement.Instance.ReloadGuidUI;
            if (ReloadGuidUI == null)
            {
                Debug.LogWarning("ui 표시를 취소할 대상 ui 오브젝트가 없습니다.");
                return;
            }

            //adapter code
            if (ReloadGuidUI.TryGetComponent(out ICancelReload cancelReload))
            {
                cancelReload.CancelReload();
            }
            else
            {
                Debug.LogWarning("ui 표시를 취소할 대상 ui 오브젝트에 ICancelReload 인터페이스가 없습니다.");
                return;
            }
        }
        #endregion        
    }
}
