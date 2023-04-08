using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FogSystem : MonoBehaviour
{
    [Header("안개가 사라지는 시간")]
    [SerializeField]
    float fogTime = 3f;

    [Header("안개가 있을 때, 플레이어의 높이 제한")]
    [SerializeField]
    float CeilingHeightLimit = 45f;

    [Header("Fog Meterial")]
    [SerializeField] Material fogMaterial;
    [SerializeField] AnimationCurve fogAlphaCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);


    [Header("Window Meterial")]
    [SerializeField] Material windowMaterial;
    [SerializeField] AnimationCurve windowSmoothnessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    [Header("Direction Light")]
    [SerializeField] Light directionalLight;
    [SerializeField] AnimationCurve directionalLightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] AnimationCurve ShadowStrengthCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);


    [Header("Light Intensity Multiplier")]
    [SerializeField] AnimationCurve lightIntensityMultiplier = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] AnimationCurve ReflectionIntensityMultiplierMultiplier = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

    [Header("Ground Fog Meterial")]
    [SerializeField] Material groundFogMeterial;
    [SerializeField] AnimationCurve groundFogAlphaCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [SerializeField] GameObject Player;

    IEnumerator LowerEnvironmentLightingIntensity()
    {
        float timer = 0f;

        while (timer < fogTime)
        {
            float t = timer / fogTime;

            // Set Lighting++
            UnityEngine.RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(t);

            UnityEngine.RenderSettings.reflectionIntensity = ReflectionIntensityMultiplierMultiplier.Evaluate(t);

            //set direction light++
            directionalLight.shadowStrength = ShadowStrengthCurve.Evaluate(t);
            float flot = directionalLightCurve.Evaluate(t);
            directionalLight.color = new Color(flot, flot, flot, 1);



            // Set fog alpha
            Color fogColor = fogMaterial.color;
            fogColor.a = fogAlphaCurve.Evaluate(t);
            fogMaterial.color = fogColor;

            // Set window smoothness
            float windowSmoothness = windowSmoothnessCurve.Evaluate(t);
            windowMaterial.SetFloat("_Smoothness", windowSmoothness);

            // Set ground fog alpha
            Color groundFogColor = groundFogMeterial.color;
            groundFogColor.a = groundFogAlphaCurve.Evaluate(t);
            groundFogMeterial.color = groundFogColor;

            timer += Time.deltaTime;
            yield return null;
        }


        UnityEngine.RenderSettings.reflectionIntensity = ReflectionIntensityMultiplierMultiplier.Evaluate(1);

        //set direction light++
        directionalLight.shadowStrength = ShadowStrengthCurve.Evaluate(1);
        float value = directionalLightCurve.Evaluate(1);
        directionalLight.color = new Color(value, value, value, 1);

        // Set final values
        Color fogFinalColor = fogMaterial.color;
        fogFinalColor.a = fogAlphaCurve.Evaluate(1f);
        fogMaterial.color = fogFinalColor;

        windowMaterial.SetFloat("_Smoothness", windowSmoothnessCurve.Evaluate(1f));

        Color groundFogFinalColor = groundFogMeterial.color;
        groundFogFinalColor.a = groundFogAlphaCurve.Evaluate(1f);
        groundFogMeterial.color = groundFogFinalColor;

        UnityEngine.RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(1);
    }

    public void ResetFog()
    {
        if (ClearFog != null) StopCoroutine(ClearFog);


        UnityEngine.RenderSettings.reflectionIntensity = ReflectionIntensityMultiplierMultiplier.Evaluate(0);

        //set direction light++
        directionalLight.shadowStrength = ShadowStrengthCurve.Evaluate(0);
        float value = directionalLightCurve.Evaluate(0);
        directionalLight.color = new Color(value, value, value, 1);


        Color fogColor = fogMaterial.color;
        fogColor.a = fogAlphaCurve.Evaluate(0f);
        fogMaterial.color = fogColor;

        windowMaterial.SetFloat("_Smoothness", windowSmoothnessCurve.Evaluate(0f));

        UnityEngine.RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(0);

        Color groundFogColor = groundFogMeterial.color;
        groundFogColor.a = groundFogAlphaCurve.Evaluate(0f);
        groundFogMeterial.color = groundFogColor;
    }
    void SetValue(float v)
    {
        //light reflectionIntensity
        UnityEngine.RenderSettings.reflectionIntensity = ReflectionIntensityMultiplierMultiplier.Evaluate(v);
        //light Intensity
        UnityEngine.RenderSettings.ambientIntensity = lightIntensityMultiplier.Evaluate(v);

        //direction light shadow
        directionalLight.shadowStrength = ShadowStrengthCurve.Evaluate(v);
        //direction light color
        float flo = directionalLightCurve.Evaluate(v);
        directionalLight.color = new Color(flo, flo, flo, 1);


        //window
        windowMaterial.SetFloat("_Smoothness", windowSmoothnessCurve.Evaluate(v));

        //fog
        Color fogColor = fogMaterial.color;
        fogColor.a = fogAlphaCurve.Evaluate(v);
        fogMaterial.color = fogColor;

        //ground fog
        Color groundFogColor = groundFogMeterial.color;
        groundFogColor.a = groundFogAlphaCurve.Evaluate(v);
        groundFogMeterial.color = groundFogColor;
    }

    Coroutine ClearFog;
    public void SkyMakeClear()
    {
        if(ClearFog != null) StopCoroutine(ClearFog);
        ClearFog = StartCoroutine(LowerEnvironmentLightingIntensity());
    }

    private void Update()
    {
        if(Player.transform.position.y > CeilingHeightLimit)
        {
            // -hp and effect
        }
    }
}
