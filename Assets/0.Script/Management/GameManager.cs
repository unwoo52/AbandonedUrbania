using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class GameManager : MonoBehaviour
    {


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 마우스가 존재하면
                if (Input.mousePresent)
                {
                    // 마우스 숨기기
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else // 마우스가 존재하지 않으면
                {
                    // 마우스 보이기
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
        [SerializeField] private float lightIntensityMultiplier = 1.0f;
        void setFog()
        {
            RenderSettings.ambientIntensity = lightIntensityMultiplier;

        }
    }

}
