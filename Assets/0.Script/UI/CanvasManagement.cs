using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class CanvasManagement : MonoBehaviour
{
    #region singleton
    private static CanvasManagement _instance = null;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static CanvasManagement Instance
    {
        get
        {
            if (null == _instance)
            {
                return null;
            }
            return _instance;
        }
    }
    #endregion
    [SerializeField] GameObject interactUI;
    [SerializeField] GameObject healthDisplay;
    [SerializeField] GameObject hitEffectImage;
    [SerializeField] GameObject reloadGuidUI;

    public GameObject InteractUI => interactUI;
    public GameObject HealthDisplay => healthDisplay;
    public GameObject HitEffectImage => hitEffectImage;
    public GameObject ReloadGuidUI => reloadGuidUI;
}
