using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class InteractSystem : MonoBehaviour
{
    [Header("3d 카메라")]
    [SerializeField] GameObject cam;

    #region Unity Callbacks
    private void Update()
    {
        GetInput();
        InputProcsee();
    }
    #endregion

    #region Input
    //Input Fields
    bool Interact = false;
    bool InteractUp = false;
    void GetInput()
    {
        Interact = Input.GetButtonDown("Interact");
        InteractUp = Input.GetButtonUp("Interact");
    }
    void InputProcsee()
    {
        if (Interact) InputProcsee_InteractKey_Down();
        if (InteractUp) InputProcsee_InteractKey_Up();
    }
    #endregion

    #region Interact System

    //interact system fields

    [Header("Interact")]
    [SerializeField] float interactDistance;
    [SerializeField] float hitSphereRadius;

    float InteractTime;
    float curInteractTime;
    InteractObject curInteractObject;
    ISetInteractUI curInteractUI;
    Coroutine CorInteract;
    void InputProcsee_InteractKey_Down()//상호작용 사거리가 카메라 기준임. 나중에 플레이어 기준으로 바꿀 것.
    {
        //레이를 발사해서 탐색할 위치 원점을 찾음
        Ray ray = default;
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Interactable");

        //get ray from camera3d
        if (cam.TryGetComponent(out IGetRayAtCamera getRayAtCamera))
        {
            if (!getRayAtCamera.GetRayAtCamera(out ray)) return;
        }
        

        if (!Physics.Raycast(ray, out hit, interactDistance, layerMask))
            return;

        //탐색할 원점을 기준으로 OverlapSphere를 이용해 상호작용 오브젝트들을 탐색
        Collider[] colliders = Physics.OverlapSphere(hit.point, hitSphereRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.transform.gameObject.TryGetComponent(out InteractObject interactObject))
            {
                //상호작용 가능한 오브젝트가 있으면 상호작용 이벤트 시작(코루틴 시작)
                if (interactObject == curInteractObject) continue;

                curInteractObject = interactObject;
                StartInteract(collider.transform.gameObject);
                break;
            }
            else
            {
                Debug.LogError("상호작용 오브젝트에 상호작용 스크립트가 없습니다");
            }
        }
    }

    void StartInteract(GameObject gameObject)
    {
        //상호작용 시작 함수

        //set ISetInteractUI
        if (CanvasManagement.Instance.InteractUI.TryGetComponent(out ISetInteractUI setInteractUI))
        {
            if (CorInteract != null) StopCoroutine(CorInteract);
            curInteractUI = setInteractUI;
            CorInteract = StartCoroutine(InteractBehavior(curInteractObject.InteractTime));
        }

        //init this objects, 1.Player 2.Cam
        curInteractObject.InteractPlayer = transform.parent.gameObject;
        curInteractObject.InteractCam = cam;
    }

    void InputProcsee_InteractKey_Up()
    {
        if (curInteractTime >= 0) // cancel Interact.
        {
            curInteractObject.OnCancelInteract?.Invoke();
            Debug.Log("Cancel!");
        }
        else // complete interact
        {
            //curInteractObject.OnCompleteInteract?.Invoke(); << InteractBehavior 코루틴에 있음. 코루틴의 while문 종료가 끝날 때 만 실행.
        }

        if (CorInteract != null) StopCoroutine(CorInteract);
        if (CanvasManagement.Instance.InteractUI.TryGetComponent(out ISetInteractUI setInteractUI))
        {
            SetInActive_InteractUI();
        }

        curInteractObject = null;
        curInteractUI = null;
    }
    IEnumerator InteractBehavior(float InteractTime)
    {
        curInteractTime = InteractTime;

        while (curInteractTime >= 0)
        {
            float temp = 0; // 0 ~ 1

            curInteractTime -= Time.deltaTime; // 시간 감소
            temp = curInteractTime / InteractTime; // interactFillAmountTime을 0 ~ 1로 표현한 값

            curInteractUI.SetInteractImageFillamount(temp);

            yield return null;
        }
        curInteractObject.OnCompleteInteract?.Invoke();
        curInteractObject = null;
        SetInActive_InteractUI();
        Debug.Log("End!");
        yield return null;
    }

    void SetInActive_InteractUI()
    {
        curInteractUI.SetInteractImageFillamount(-1);
    }
    #endregion
}
