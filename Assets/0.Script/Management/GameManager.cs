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
                // ���콺�� �����ϸ�
                if (Input.mousePresent)
                {
                    // ���콺 �����
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else // ���콺�� �������� ������
                {
                    // ���콺 ���̱�
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
