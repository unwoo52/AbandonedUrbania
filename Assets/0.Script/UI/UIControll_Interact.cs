using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ISetInteractUI
{
    void SetInteractImageFillamount(float fillvalue);
}
public class UIControll_Interact : MonoBehaviour, ISetInteractUI
{
    [SerializeField] GameObject InteractFillBackground;
    [SerializeField] Image InteractFillFront;
    public GameObject Playerinfo;

    public void SetInteractImageFillamount(float fillvalue)
    {
        SetInteractFillValue(fillvalue);
    }

    void SetActive_InteractUI(bool b)
    {
        InteractFillBackground.SetActive(b);
        InteractFillFront.gameObject.SetActive(b);
    }
    void SetInteractFillValue(float fillvalue)
    {
        if(fillvalue < 0)
            SetActive_InteractUI(false);
        else
            SetActive_InteractUI(true);

        InteractFillFront.fillAmount = fillvalue;
    }
}
