using System.Collections;
using UnityEngine;

public class RobotActions : MonoBehaviour
{
    [SerializeField] private GameObject robotGun;
    [SerializeField] private GameObject robotEye;

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
        if(RecoilCor != null) StopCoroutine(RecoilCor);
        RecoilCor = StartCoroutine(RecoilCoroutine());
    }

    private Coroutine shakeCoroutine;


    private void Start()
    {
        waitForSeconds = new WaitForSeconds(shakeFrequency);
        // 코루틴을 시작합니다.
        shakeCoroutine = StartCoroutine(ShakeRobotBody());
    }
    public void looktarget(Vector3 pos)
    {
        Vector3 GunVec = robotGun.transform.position;
        Vector3 direction = pos - GunVec;

        float distance = new Vector3(direction.x, 0f, direction.z).magnitude;
        distance = pos.y < GunVec.y ? -distance : distance;
        float high = Mathf.Abs(direction.y);

        float targetAngleX = high >= 0 ? Mathf.Atan(high / distance) * Mathf.Rad2Deg : (Mathf.Atan(high / distance) * Mathf.Rad2Deg);
        float targetAngleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;


        //if (Mathf.Abs(targetAngleY) <= 90)
        if(true)
        {
            robotGun.transform.rotation = Quaternion.Euler(targetAngleX, targetAngleY + 180, 90);
        }
    }
    [ContextMenu("LookRandom")]
    public void RandomLook()
    {
        // 로봇을 기준으로 랜덤한 방향으로 10만큼 떨어진 거리의 포지션(lookposition)을 얻어옵니다.
        Vector3 lookposition = robotGun.transform.position + Random.insideUnitSphere * 1000f;
        Debug.DrawRay(robotGun.transform.position, robotGun.transform.position - lookposition, Color.red, 5f);
        // 얻은 포지션을 RobotLook 함수로 전달합니다.

        StartCoroutine(RobotLook(lookposition));
    }

    IEnumerator ShakeRobotBody()
    {
        while (true)
        {
            Vector3 newPosition = Random.insideUnitSphere * shakeAmplitude;
            if (RecoilCor != null) newPosition += recoilvector;
            robotGun.transform.localPosition = newPosition;
            yield return waitForSeconds;
        }
    }


    private IEnumerator RobotLook(Vector3 pos)
    {

        // 덜컹거리는 움직임
        Quaternion originRotation = robotGun.transform.rotation;
        float elapsed = 0f;
        while (elapsed < lookDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lookDuration;
            float angle1 = Mathf.Lerp(jitterAngle, 0f, t);
            float speed = Mathf.Lerp(maxAngleSpeed, minAngleSpeed, lookCurve.Evaluate(t));
            robotGun.transform.rotation = originRotation * Quaternion.AngleAxis(angle1 * Mathf.Sin(speed * t), Vector3.right);
            yield return null;
        }
        

        Vector3 targetDirection = targetGameObject.position - robotEye.transform.position;
        float targetAngleY = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
        float targetAngleX = Mathf.Atan2(targetDirection.y, targetDirection.z) * Mathf.Rad2Deg;

        Quaternion fromRotation = robotGun.transform.rotation;
        Quaternion toRotation = Quaternion.Euler(-targetAngleX, targetAngleY, -90f);

        float elapsedTime = 0f;

        while (elapsedTime < rotateTime)
        {
            robotGun.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, (elapsedTime / rotateTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        robotGun.transform.rotation = toRotation;




    }
    private IEnumerator RecoilCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < recoilTime)
        {
            // recoil backward
            float curveValue = recoilCurve.Evaluate(elapsedTime / recoilTime);
            recoilvector = robotGun.transform.forward * curveValue * recoilPower;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
