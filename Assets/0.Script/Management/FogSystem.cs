using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Urban_KimHyeonWoo
{
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
                SetValue(t);

                timer += Time.deltaTime;
                yield return null;
            }


            SetValue(1);
        }

        public void ResetFog()
        {
            if (ClearFog != null) StopCoroutine(ClearFog);


            SetValue(0);
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
            if (ClearFog != null) StopCoroutine(ClearFog);
            ClearFog = StartCoroutine(LowerEnvironmentLightingIntensity());
        }

        private void Update()
        {
        }
        [Header("Follow Player Speed")]
        [SerializeField] private float followSpeed = 10f;

        private void FixedUpdate()
        {
            if (Player == null) return;

            Vector3 newPos = Vector3.Lerp(transform.position, Player.transform.position, Time.fixedDeltaTime * followSpeed);
            transform.position = new Vector3(newPos.x, 0, newPos.z);


            if (Player.transform.position.y > CeilingHeightLimit)
            {
                Debug.Log("High!!");
            }
        }
    }

}
