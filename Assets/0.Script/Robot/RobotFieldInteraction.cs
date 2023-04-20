using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urban_KimHyeonWoo
{
    public interface IDrawAttention
    {
        void OnDrawAttention(GameObject gameObject);
    }
    public class RobotFieldInteraction : MonoBehaviour, IDrawAttention
    {
        public void OnDrawAttention(GameObject gameObject)
        {
            Debug.Log("Attents Robot");
            Debug.Log(this.gameObject.name);
        }
    }

}
