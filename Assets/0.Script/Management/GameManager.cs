using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public class GameManager : MonoBehaviour
    {
        #region singleton
        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        public List<GameObject> Players;

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
    }

}
