using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("The intensity of the camera shake.")]
    public float shakeIntensity = 0.1f;

    [Tooltip("The duration of the camera shake.")]
    public float shakeDuration = 0.5f;

    [Tooltip("The animation curve that controls the shaking intensity over time.")]
    public AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }
    public float inten;
    public float dura;
    public AnimationCurve intenCurve;
    [ContextMenu("TEST")]
    void TSET()
    {
        Shake(inten, dura, intenCurve);
    }
    public void Shake(float intensity, float duration, AnimationCurve curve)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(intensity, duration, curve));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration, AnimationCurve curve)
    {
        float time = 0;

        while (time < duration)
        {
            float shakeAmount = curve.Evaluate(time / duration) * intensity;
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            transform.localPosition = originalPosition + randomOffset;

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
