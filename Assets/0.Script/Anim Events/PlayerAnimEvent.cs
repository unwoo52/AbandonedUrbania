using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField]UnityEvent m_Event;
    public void EndRoll()
    {
        m_Event?.Invoke();
    }
}
