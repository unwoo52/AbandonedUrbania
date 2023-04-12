using Lightbug.CharacterControllerPro.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Urban_KimHyeonWoo
{
    public interface IBindPlayer
    {
        void BindPlayer(bool setvalue);
    }
    public class CharacterControll : MonoBehaviour, IBindPlayer, ITestDamageSystem
    {
        private void Start()
        {
            if (CanvasManagement.Instance.HealthDisplay.TryGetComponent(out ISetHealthDisplay setHealthDisplay))
            {
                setHealthDisplay.SetHealthDisplay(hp);
            }
        }
        [SerializeField] float hp = 1000;
        public void BindPlayer(bool setvalue)
        {
            Transform actionsObject = transform.Find("Actions");
            if (actionsObject != null)
            {
                actionsObject.gameObject.SetActive(!setvalue);
            }
            else Debug.LogError("cannot find \"Actions\" Object!");
        }

        public void OnDam(float dmg)
        {
            hp -= dmg;
            PlayDamageEffect();
        }
        
        void PlayDamageEffect()
        {
            if(CanvasManagement.Instance.HitEffectImage.TryGetComponent(out IEffectHitUI effectHitUI))
            {
                effectHitUI.EffectHitUI();
            }
            if (CanvasManagement.Instance.HealthDisplay.TryGetComponent(out ISetHealthDisplay setHealthDisplay))
            {
                setHealthDisplay.SetHealthDisplay(hp);
            }
        }
    }
}

