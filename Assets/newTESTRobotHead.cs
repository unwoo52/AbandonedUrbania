using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newTESTRobotHead : MonoBehaviour
{
    public Transform targetTransform;
    public float rotationSpeed = 1f;

    private Quaternion targetRotation;

    private void Update()
    {
        Vector3 lookDir = targetTransform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);

        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

