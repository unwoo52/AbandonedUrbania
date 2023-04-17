using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��� ���� ���� ���¸� �����ϴ� ���¸ӽ�
public class WeaponController : MonoBehaviour
{
    [Header("Bullet Field")]
    //bullet fire system field
    [SerializeField][Tooltip("�߻� ��Ÿ��")] float bulletCooldown = 0.5f;
    private float currentBulletCooldown = 0f; // ���� ��Ÿ��
    [SerializeField][Tooltip("�Ѿ� ��Ÿ�")] float maxDistance = 1000f;

    [SerializeField] GameObject BulletPrefab; // �Ѿ� ������
    AudioSource audioSource;
    [SerializeField] AudioClip FireSound;

    [SerializeField] GameObject SkillEffectObject;

    [Header("���� ���� field")]
    [SerializeField] Transform muzzleTransform;
    [SerializeField] LayerMask hitableMask;

    [SerializeField] float skilltime = 1.2f;
    [SerializeField] int bulletcount = 30;

    [SerializeField]
    [Tooltip("������ �ð�")] float ReloadTime;

    [Header("��� �� �����ð� (������ �ð� ����)")]
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

        // �Ѿ� ��Ÿ�� ����
        if (currentBulletCooldown > 0f)
        {
            currentBulletCooldown -= dt;
        }

        if (CharacterActions.Reload.value == true)
        {
            //if() źâ�� �� ������ �ʴٸ�
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
                //layer weight ����
                //�ִϸ��̼� ������ ���� trigger ����
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
                //layer weight ����
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
            Debug.LogError("SetLayerWeight�Ϸ��� ���̾ �������ִϸ��̼��� �ִ� ��ݽ� ���̾ �ƴմϴ�.");
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

        // ��Ÿ���� ���� �������� �߻����� ����
        if (currentBulletCooldown > 0f) return;

        // �߻� ��Ÿ�� �ʱ�ȭ
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

    #region AnimMethod
    [Tooltip("ĳ������ �ִϸ����Ϳ��� ��ݽ��� �����ϴ� upper layer�� ��ȣ")]
    [SerializeField] int UpperAimRunLayerNum = 1;
    float curUpperLayerValue = 0;
    void SetAnimControllerSetLayerWeight(ref float curLayerValue, int LayerNum, float destValue, float dt)
    {
        curLayerValue = Mathf.Lerp(curLayerValue, destValue, 20 * dt);
        CharacterActor.Animator.SetLayerWeight(LayerNum, curLayerValue);
    }

    #endregion

    #region public method
    /// <summary> ���� �߻����� �ð��� �� ������ �ʾ����� true�� ��ȯ, ���� ���� ���� ���ÿ��� false�� ��ȯ </summary>
    public bool IsFiredRecontly()
    {
        return currBattleTime > 0;
    }
    #endregion
}
