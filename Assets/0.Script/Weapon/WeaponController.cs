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
    //��� ���� ���� ���¸� �����ϴ� ���¸ӽ�
    public class WeaponController : MonoBehaviour, ISetWeaponInfoUI
    {
        [Header("Weapon Info")]
        [SerializeField] WeaponInfo weaponInfo = new WeaponInfo();
        public WeaponInfo WeaponInfo => weaponInfo;

        int currentAmmo;
        public int CurrentAmmo => currentAmmo;

        [Header("Bullet Field")]
        //bullet fire system field
        [SerializeField][Tooltip("�߻� ��Ÿ��")] float bulletCooldown = 0.5f;
        private float currentBulletCooldown = 0f; // ���� ��Ÿ��
        [SerializeField][Tooltip("���� ���� ��Ÿ�")] float maxDistance = 1000f;

        [SerializeField] GameObject BulletPrefab; // �Ѿ� ������
        AudioSource audioSource;
        [SerializeField] AudioClip FireSound;

        [SerializeField] GameObject SkillEffectObject;

        [Header("���� ���� field")]
        [SerializeField] Transform muzzleTransform;
        [SerializeField] LayerMask hitableMask;

        [SerializeField] float skilltime = 1.2f;
        [SerializeField] int bulletcount = 30;


        [Header("��� �� �����ð� (������ �ð� ����)")]
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

            // �Ѿ� ��Ÿ�� ����
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
                    //layer weight ����
                    //�ִϸ��̼� ������ ���� trigger ����
                    isReload = false;
                    Adapter_SetResidualAmmo();
                    break;
                case WeaponFireState.OnSkill:
                    break;
                case WeaponFireState.Disable:
                    //���� state�� disable�̰�, �Ѿ��� 0�� �� (���� �����ߴµ�, ������ ���� ��ź�� 0�̶�� changerState�� ����ϰ�, reload�� changeState ����
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
                        //if() źâ�� �� ������ �ʴٸ�
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

            // ��Ÿ���� ���� �������� �߻����� ����
            if (currentBulletCooldown > 0f) return;
            if (currentAmmo < 1) return;

            // �߻� ��Ÿ�� �ʱ�ȭ
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
                // �浹�� ������ ��ġ�� ����ϴ� �ڵ�
            }
            else
            {
                hitPoint = ray.origin + ray.direction * maxDistance;
                // ������ ��Ÿ� �� ������ ����ϴ� �ڵ�
            }



            //audioSource.clip = FireSound;
            audioSource.Stop();
            audioSource.PlayOneShot(FireSound);

            // BulletPrefab�� instantiate�Ͽ� ������ �Ѿ��� �����̼��� �Է¹��� ray�� �ٶ󺸰� ȸ����Ŵ
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
            // �߻� �ð��� �Ѿ� ������ ������ �ڷ�ƾ ���� �߻� �ֱ⸦ ����Ѵ�
            float interval = shootTime / bulletCount;

            // �Ѿ� �߻縦 ���� bulletCount��ŭ Bullet() �ڷ�ƾ �Լ��� �����Ѵ�
            for (int i = 0; i < bulletCount; i++)
            {
                Bullet(); // �Ѿ� �߻�
                yield return new WaitForSeconds(interval); // �߻� �ֱ⸸ŭ ���
            }
        }
        #endregion

        #region set Animator LayerWeight Method                      -----------
        [Tooltip("ĳ������ �ִϸ����Ϳ��� ��ݽ��� �����ϴ� upper layer�� ��ȣ")]
        [SerializeField] int UpperAimRunLayerNum = 1;
        float curUpperLayerValue = 0;
        void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum, float destValue, float dt)
        {
            curLayerValue = Mathf.Lerp(curLayerValue, destValue, 20 * dt);
            CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
        }


        #endregion

        #region public method                   -----------
        /// <summary> ���� �߻����� �ð��� �� ������ �ʾ����� true�� ��ȯ, ���� ���� ���� ���ÿ��� false�� ��ȯ </summary>
        public bool IsFiredRecontly()
        {
            return currBattleTime > 0;
        }
        #endregion

        #region Adapter                         -----------
        //WeaponState Adapter

        [Header("Adapter Field")]
        //IActionStateChangeListener �������̽��� ���� �ִ� ������Ʈ
        [SerializeField] GameObject WeaponInfoUI;
        void Adapter_EmptyWeaponInfoUI()
        {
            WeaponInfoUI = null;
        }

        void Adapter_SetResidualAmmo()
        {            
            if (WeaponInfoUI == null)
            {
                Debug.LogWarning("Weapon Slot�� ����� ����� Notify�� ��� ������Ʈ�� �����ϴ�.");
                return;
            }
                
            //adapter code
            if (WeaponInfoUI.TryGetComponent(out ISetResidualAmmoUI interfc))
            {
                interfc.SetResidualAmmoUI(currentAmmo);
            }
            else
            {
                Debug.LogWarning("Weapon Slot�� ����� ����� Notify�� ��� ������Ʈ�� Listener �������̽��� �����ϴ�.");
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
                Debug.LogWarning("ui ǥ�ø� ����� ��� ui ������Ʈ�� �����ϴ�.");
                return;
            }

            //adapter code
            if (ReloadGuidUI.TryGetComponent(out ICancelReload cancelReload))
            {
                cancelReload.CancelReload();
            }
            else
            {
                Debug.LogWarning("ui ǥ�ø� ����� ��� ui ������Ʈ�� ICancelReload �������̽��� �����ϴ�.");
                return;
            }
        }
        #endregion        
    }
}
