using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public interface IEffectHitUI
    {
        void EffectHitUI();
    }
    public class HitEffect : MonoBehaviour, IEffectHitUI
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float fadeDuration = 1f;
        Coroutine CorFadeOutCanvasGroup;

        public void EffectHitUI()
        {
            if(CorFadeOutCanvasGroup != null) { StopCoroutine(CorFadeOutCanvasGroup);}
            CorFadeOutCanvasGroup = StartCoroutine(FadeOutCanvasGroup());
        }
        IEnumerator FadeOutCanvasGroup()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
    }
}

