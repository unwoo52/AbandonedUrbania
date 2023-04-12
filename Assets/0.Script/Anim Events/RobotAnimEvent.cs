using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Urban_KimHyeonWoo
{
    public class RobotAnimEvent : MonoBehaviour
    {
        [SerializeField]UnityEvent wakeupEvent;
        public void AwakeRobot()
        {
            Debug.Log("AWAKE!!");
            wakeupEvent?.Invoke();
        }
        public void TestAnimMethod()
        {

        }
    }
}
