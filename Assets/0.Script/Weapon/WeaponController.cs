using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사격 장전 등의 상태를 조절하는 상태머신
public class WeaponController : MonoBehaviour
{
    [Header("Bullet Field")]
    //bullet fire system field
    [SerializeField][Tooltip("발사 쿨타임")] float bulletCooldown = 0.5f;
    private float currentBulletCooldown = 0f; // 현재 쿨타임
    [SerializeField][Tooltip("총알 사거리")] float maxDistance = 1000f;

    [SerializeField] GameObject BulletPrefab; // 총알 프리팹
    AudioSource audioSource;
    [SerializeField] AudioClip FireSound;

    [SerializeField] GameObject SkillEffectObject;

    [Header("무기 관련 field")]
    [SerializeField] Transform muzzleTransform;
    [SerializeField] LayerMask hitableMask;

    [SerializeField] float skilltime = 1.2f;
    [SerializeField] int bulletcount = 30;

    [SerializeField]
    [Tooltip("재장전 시간")] float ReloadTime;

    [Header("사격 후 전투시간 (숨고르기 시간 개념)")]
    [SerializeField] float battletime = 0.2f;
    public float currBattleTime;

    Camera Cam = null;
    //reload
    bool isReload = false;
    public bool IsReload => isReload;
    Coroutine CorReload;
    public bool FireLock = false;


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

        if (CharacterActions.Reload.value == true)
        {
            //if() 탄창이 꽉 차있지 않다면
            ChangeWeaponFireState(WeaponFireState.Reload);
        }

        

        
        
    }
    #endregion

    #region Weapon State Machine
    public enum WeaponFireState
    {
        CanFire, Reload, OnSkill
    }
    public WeaponFireState currentState = WeaponFireState.CanFire;
    public void ChangeWeaponFireState(WeaponFireState state)
    {
        if(currentState == state) return;
        ExitStateBehavior(currentState);
        currentState = state;
        EnterStateBehavior(currentState);
    }
    void EnterStateBehavior(WeaponFireState state)
    {
        switch (state)
        {
            case WeaponFireState.CanFire:
                break;
            case WeaponFireState.Reload:
                CharacterActor.Animator.SetTrigger("Trigger_Reload");
                isReload = true;
                break;
            case WeaponFireState.OnSkill:
                break;
            default:
                break;
        }
    }
    void ExitStateBehavior(WeaponFireState state)
    {
        switch (state)
        {
            case WeaponFireState.CanFire:
                break;
            case WeaponFireState.Reload:
                //layer weight 설정
                //애니메이션 실행을 위한 trigger 실행
                isReload = false;
                break;
            case WeaponFireState.OnSkill:
                break;
            default:
                break;
        }
    }
    void StateMachine(float dt)
    {
        switch (currentState)
        {
            case WeaponFireState.CanFire:
                //layer weight 설정
                if (IsFiredRecontly())
                {
                    SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);
                }
                else
                {
                    SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 0, dt);
                }
                //fire
                if (CharacterActions.Fire1.value == true)                
                    OrderFire();                
                break;
            case WeaponFireState.Reload:
                SetAnimControllerSetLayerWeight(ref curUpperLayerValue, UpperAimRunLayerNum, 1, dt);

                break;
            case WeaponFireState.OnSkill:
                break;
            default:
                break;
        }
    }
    #endregion

    #region Anim Event
    public void OnExitReload()
    {
        ChangeWeaponFireState(WeaponFireState.CanFire);
    }
    #endregion

    #region Weapon System
    IEnumerator corReload(float duration)
    {
        isReload = true;
        float curdurationTime = duration;

        //check layer
        if (CharacterActor.Animator.GetLayerName(1) != "Upper Layer")
        {
            Debug.LogError("SetLayerWeight하려는 레이어가 재장전애니메이션이 있는 상반신 레이어가 아닙니다.");
            StopCoroutine(CorReload);
        }
        CharacterActor.Animator.SetTrigger("Trigger_Reload");


        while (curdurationTime > 0)
        {
            CharacterActor.Animator.SetLayerWeight(1, 1);
            float dt = Time.deltaTime;
            if (curdurationTime <= dt)
            {
                dt = curdurationTime;
            }
            curdurationTime -= dt;
            yield return null;
        }

        CharacterActor.Animator.SetLayerWeight(1, 0);

        isReload = false;
    }

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

        // 발사 쿨타임 초기화
        currentBulletCooldown = bulletCooldown;
        Bullet();
    }
    void Bullet()
    {
        currBattleTime = battletime;

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

    #region AnimMethod
    [Tooltip("캐릭터의 애니메이터에서 상반신을 통제하는 upper layer의 번호")]
    [SerializeField] int UpperAimRunLayerNum = 1;
    float curUpperLayerValue = 0;
    void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum, float destValue, float dt)
    {
        curLayerValue = Mathf.Lerp(curLayerValue, destValue, 20 * dt);
        CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
    }

    #endregion

    #region public method
    /// <summary> 총을 발사한지 시간이 얼마 지나지 않았으면 true를 반환, 총을 쏘지 않은 평상시에는 false를 반환 </summary>
    public bool IsFiredRecontly()
    {
        return currBattleTime > 0;
    }
    #endregion
}
