using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class WeaponStateController : MonoBehaviour
    {
        [Header("States")]
        public WeaponState initialState = null;
        public WeaponState CurrentState { get; protected set; }
        public WeaponState PreviousState { get; protected set; }


        [Header("Bullet Field")]
        //bullet fire system field
        [SerializeField][Tooltip("�߻� ��Ÿ��")] float bulletCooldown = 0.5f;
        private float currentBulletCooldown = 0f; // ���� ��Ÿ��
        [SerializeField] [Tooltip("�Ѿ� ��Ÿ�")]float maxDistance = 1000f;

        [SerializeField] GameObject BulletPrefab; // �Ѿ� ������
        AudioSource audioSource;
        [SerializeField] AudioClip FireSound;


        [Header("weapon and hand position")]
        //weapon and hand position
        [SerializeField] GameObject weaponObject;
        [SerializeField] GameObject handPosition;
        [SerializeField] GameObject backPosition;
        public GameObject WeaponObject => weaponObject;


        [Header("Cam")]
        [SerializeField] Camera3D camera3D;
        public Camera3D Camera3D => camera3D;
        public Camera Cam;

        [SerializeField] ControllCamera3D controllCamera3D;
        public ControllCamera3D ControllCamera3D => controllCamera3D;

        bool machineStarted = false;

        public void ChangeWeaponPos_Hand()
        {
            WeaponObject.transform.SetParent(handPosition.transform, false);
        }

        public void ChangeWeaponPos_Back()
        {
            WeaponObject.transform.SetParent(backPosition.transform, false);
        }

        Queue<WeaponState> transitionsQueue = new Queue<WeaponState>();

        #region unity Callbacks
        private void Awake()
        {
            camera3D = transform.parent.parent.GetChild(0).GetComponent<Camera3D>();
            controllCamera3D = transform.parent.parent.GetChild(0).GetComponent<ControllCamera3D>();
            Cam = camera3D.gameObject.GetComponent<Camera>();
            audioSource = this.GetComponentInBranch<CharacterActor,AudioSource>();

            //�⺻ ���°� ������,

            if (weaponObject == null)
            {
                weaponObject = transform.GetChild(0).gameObject;
            }

        }
        private void FixedUpdate()
        {
            if (!machineStarted)
            {
                if (initialState == null)
                {
                    enabled = false;
                    return;
                }

                CurrentState = initialState;


                if (CurrentState == null)
                    return;

                if (!CurrentState.isActiveAndEnabled)
                    return;

                machineStarted = true;
                CurrentState.EnterBehaviour(0f, CurrentState);
            }



            if (CurrentState == null)
                return;

            if (!CurrentState.isActiveAndEnabled)
                return;


            if (!machineStarted)
            {
                CurrentState.EnterBehaviour(0f, CurrentState);
                machineStarted = true;
            }


            float dt = Time.deltaTime;

            bool validTransition = CheckForTransitions();
            transitionsQueue.Clear();
            if (validTransition)
            {
                PreviousState.ExitBehaviour(dt, CurrentState);
                CurrentState.EnterBehaviour(dt, PreviousState);
            }

            CurrentState.PreUpdateBehaviour(dt);
            CurrentState.UpdateBehaviour(dt);
            CurrentState.PostUpdateBehaviour(dt);
        }

        private void Update()
        {
            // �Ѿ� ��Ÿ�� ����
            if (currentBulletCooldown > 0f)
            {
                currentBulletCooldown -= Time.deltaTime;
            }
        }
        #endregion

        #region Fire Weapon System
        [SerializeField] Transform muzzleTransform;
        [SerializeField] LayerMask hitableMask;
        public void DoFire()
        {
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
            // ��Ÿ���� ���� �������� �߻����� ����
            if (currentBulletCooldown > 0f) return;

            // �߻� ��Ÿ�� �ʱ�ȭ
            currentBulletCooldown = bulletCooldown;

            audioSource.PlayOneShot(FireSound);

            // BulletPrefab�� instantiate�Ͽ� ������ �Ѿ��� �����̼��� �Է¹��� ray�� �ٶ󺸰� ȸ����Ŵ
            GameObject bullet = Instantiate(BulletPrefab);
            bullet.transform.position = muzzleTransform.position;
            Vector3 direction = (hitPoint - muzzleTransform.position).normalized;
            bullet.transform.rotation = Quaternion.LookRotation(direction);
        }
        #endregion
        public GameObject t1;
        public GameObject t2;
        #region Events                  -----------------

        /// <summary>
        /// This event is called when a state transition occurs. 
        /// 
        /// The "from" and "to" states are passed as arguments.
        /// </summary>
        public event System.Action<WeaponState, WeaponState> OnStateChange;

        #endregion

        #region Transition state Method -----------------

        public void EnqueueTransition<T>() where T : WeaponState
        {
            WeaponState state = GetComponent<T>();

            if (state == null)
                return;

            transitionsQueue.Enqueue(state);
        }
        bool CheckForTransitions()
        {
            CurrentState.CheckExitTransition();

            while (transitionsQueue.Count != 0)
            {
                //�ٲ� state
                WeaponState thisState = transitionsQueue.Dequeue();

                if (thisState == null)
                    continue;

                if (!thisState.enabled)
                    continue;

                bool success = thisState.CheckEnterTransition(CurrentState);

                if (success)
                {
                    if (OnStateChange != null)
                        OnStateChange(CurrentState, thisState);

                    PreviousState = CurrentState;
                    CurrentState = thisState;

                    return true;
                }
            }

            return false;

        }
        #endregion 
    }
}
