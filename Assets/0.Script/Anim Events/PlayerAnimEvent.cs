using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField] UnityEvent m_Event;
    [SerializeField] UnityEvent reloadEvent;
    [SerializeField] Transform LeftHand;
    [SerializeField] Animator animator;
    public void EndRoll()
    {
        m_Event?.Invoke();
    }
    public void ReloadEvent()
    {
        reloadEvent?.Invoke();
    }
    private void Update()
    {

    }
    private void OnAnimatorIK(int layerIndex)
    {
        
    }
}
