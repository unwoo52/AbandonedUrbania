using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Urban_KimHyeonWoo
{
    public class InteractObject : MonoBehaviour
    {
        [SerializeField] float interactTime;
        public float InteractTime => interactTime;
        public GameObject InteractPlayer; // ��ȣ�ۿ����� �÷��̾�
        public GameObject InteractCam;

        //��ȣ�ۿ� ������ ��, ���� ��, �Ϸ��� �� �� ���� �������̽� ����
        [Header("��ȣ�ۿ� �� ����Ǵ� �Լ�")]
        public UnityEvent OnStartInteract;
        public UnityEvent OnBeingInteract;
        public UnityEvent OnCompleteInteract;
        public UnityEvent OnCancelInteract;
    }
}

