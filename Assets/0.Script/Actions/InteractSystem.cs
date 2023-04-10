using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class InteractSystem : MonoBehaviour
{
    [Header("3d ī�޶�")]
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
    void InputProcsee_InteractKey_Down()//��ȣ�ۿ� ��Ÿ��� ī�޶� ������. ���߿� �÷��̾� �������� �ٲ� ��.
    {
        //���̸� �߻��ؼ� Ž���� ��ġ ������ ã��
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

        //Ž���� ������ �������� OverlapSphere�� �̿��� ��ȣ�ۿ� ������Ʈ���� Ž��
        Collider[] colliders = Physics.OverlapSphere(hit.point, hitSphereRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.transform.gameObject.TryGetComponent(out InteractObject interactObject))
            {
                //��ȣ�ۿ� ������ ������Ʈ�� ������ ��ȣ�ۿ� �̺�Ʈ ����(�ڷ�ƾ ����)
                if (interactObject == curInteractObject) continue;

                curInteractObject = interactObject;
                StartInteract(collider.transform.gameObject);
                break;
            }
            else
            {
                Debug.LogError("��ȣ�ۿ� ������Ʈ�� ��ȣ�ۿ� ��ũ��Ʈ�� �����ϴ�");
            }
        }
    }

    void StartInteract(GameObject gameObject)
    {
        //��ȣ�ۿ� ���� �Լ�

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
            //curInteractObject.OnCompleteInteract?.Invoke(); << InteractBehavior �ڷ�ƾ�� ����. �ڷ�ƾ�� while�� ���ᰡ ���� �� �� ����.
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

            curInteractTime -= Time.deltaTime; // �ð� ����
            temp = curInteractTime / InteractTime; // interactFillAmountTime�� 0 ~ 1�� ǥ���� ��

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
