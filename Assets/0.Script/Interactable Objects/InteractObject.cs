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
        public GameObject InteractPlayer; // 상호작용중인 플레이어
        public GameObject InteractCam;

        //상호작용 시작할 때, 중일 때, 완료일 때 에 대한 인터페이스 구현
        [Header("상호작용 중 실행되는 함수")]
        public UnityEvent OnStartInteract;
        public UnityEvent OnBeingInteract;
        public UnityEvent OnCompleteInteract;
        public UnityEvent OnCancelInteract;
    }
}

