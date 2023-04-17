using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Urban_KimHyeonWoo
{
    public class RobotActions : MonoBehaviour
    {
        //shake
        [Header("shake")]
        [SerializeField] private float shakeFrequency = 0.02f;
        [SerializeField] private float shakeAmplitude = 0.01f;
        WaitForSeconds waitForSeconds;

        [Header("look")]

        [SerializeField] private float lookDuration = 1f;
        [SerializeField] private float maxAngleSpeed = 180f;
        [SerializeField] private float minAngleSpeed = 60f;
        [SerializeField] private float jitterAngle = 15f;
        [SerializeField] private AnimationCurve lookCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


        [SerializeField] Transform targetGameObject;


        [Header("반동")]
        [SerializeField] private AnimationCurve recoilCurve;
        [SerializeField] private float recoilTime = 0.2f;
        [SerializeField] float recoilPower = 50f;
        [SerializeField] float rotateTime = 1;
        Vector3 recoilvector;
        Coroutine RecoilCor;
        public void TESTMEHTOD()
        {
            if (RecoilCor != null) StopCoroutine(RecoilCor);
            RecoilCor = StartCoroutine(RecoilCoroutine());
        }

        private Coroutine shakeCoroutine;

        [SerializeField] RobotComponenetManager robotComponenetManager;

        private void Start()
        {
            waitForSeconds = new WaitForSeconds(shakeFrequency);
            // 코루틴을 시작합니다.
            shakeCoroutine = StartCoroutine(ShakeRobotBody());
            if (robotComponenetManager == null) robotComponenetManager = GetComponent<RobotComponenetManager>();
        }

        public void LookTarget_ControllWithInputManager(float horizonInput, float verticalInput, float controllSpeed)
        {
            Vector3 currentAngle = robotComponenetManager.RobotBody.transform.rotation.eulerAngles;
            float newYAngle = currentAngle.y + horizonInput * controllSpeed;


            float newXAngle = currentAngle.x + verticalInput * controllSpeed; // -90
            if (89 < newXAngle && newXAngle < 271) newXAngle = currentAngle.x;

            Quaternion newRotation = Quaternion.Euler(newXAngle, newYAngle, 0f + 90);


            robotComponenetManager.RobotBody.transform.rotation = newRotation;
            //lookAngle(newRotation);
        }


        public void lookAngle(Quaternion rot)
        {
            Vector3 direction = rot * Vector3.forward;

            float distance = new Vector3(direction.x, 0f, direction.z).magnitude;
            distance = rot.eulerAngles.y < 180f ? distance : -distance;
            float high = Mathf.Abs(direction.y);

            float targetAngleX = high >= 0 ? Mathf.Atan(high / distance) * Mathf.Rad2Deg : (Mathf.Atan(high / distance) * Mathf.Rad2Deg);
            float targetAngleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            robotComponenetManager.RobotBody.transform.rotation = Quaternion.Euler(targetAngleX, targetAngleY + 180, 90);
        }

        //특정 포지션을 바라보는 함수
        public void looktarget(Vector3 pos)
        {
            Vector3 GunVec = robotComponenetManager.RobotBody.position;
            Vector3 direction = pos - GunVec;

            float distance = new Vector3(direction.x, 0f, direction.z).magnitude;
            distance = pos.y < GunVec.y ? -distance : distance;
            float high = Mathf.Abs(direction.y);

            float targetAngleX = Mathf.Atan(high / distance) * Mathf.Rad2Deg;
            float targetAngleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;


            robotComponenetManager.RobotBody.transform.rotation = Quaternion.Euler(targetAngleX, targetAngleY + 180, 90);            
        }









        [SerializeField] bool shakebody = true;
        IEnumerator ShakeRobotBody()
        {
            while (shakebody)
            {
                Vector3 newPosition = Random.insideUnitSphere * shakeAmplitude;
                if (RecoilCor != null) newPosition += recoilvector;
                robotComponenetManager.RobotBody.localPosition = newPosition;
                yield return waitForSeconds;
            }
        }


        private IEnumerator RobotLook(Vector3 pos)
        {

            // 덜컹거리는 움직임
            Quaternion originRotation = robotComponenetManager.RobotBody.rotation;
            float elapsed = 0f;
            while (elapsed < lookDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lookDuration;
                float angle1 = Mathf.Lerp(jitterAngle, 0f, t);
                float speed = Mathf.Lerp(maxAngleSpeed, minAngleSpeed, lookCurve.Evaluate(t));
                robotComponenetManager.RobotBody.rotation = originRotation * Quaternion.AngleAxis(angle1 * Mathf.Sin(speed * t), Vector3.right);
                yield return null;
            }


            Vector3 targetDirection = targetGameObject.position - robotComponenetManager.RobotEye.position;
            float targetAngleY = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float targetAngleX = Mathf.Atan2(targetDirection.y, targetDirection.z) * Mathf.Rad2Deg;

            Quaternion fromRotation = robotComponenetManager.RobotBody.rotation;
            Quaternion toRotation = Quaternion.Euler(-targetAngleX, targetAngleY, -90f);

            float elapsedTime = 0f;

            while (elapsedTime < rotateTime)
            {
                robotComponenetManager.RobotBody.rotation = Quaternion.Slerp(fromRotation, toRotation, (elapsedTime / rotateTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            robotComponenetManager.RobotBody.rotation = toRotation;




        }
        //반동
        private IEnumerator RecoilCoroutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < recoilTime)
            {
                // recoil backward
                float curveValue = recoilCurve.Evaluate(elapsedTime / recoilTime);
                recoilvector = robotComponenetManager.RobotBody.forward * curveValue * recoilPower;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

}
