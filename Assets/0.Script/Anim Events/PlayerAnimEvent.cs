using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField] UnityEvent m_Event;
    [SerializeField] UnityEvent ReloadEndEvent;
    [SerializeField] UnityEvent ReloadStartEvent;
    [SerializeField] Transform LeftHand;
    [SerializeField] Animator animator;
    public void EndRoll()
    {
        m_Event?.Invoke();
    }
    public void ReloadEnd()
    {
        ReloadEndEvent?.Invoke();
    }
    public void ReloadStart()
    {
        ReloadStartEvent?.Invoke();
    }

}
