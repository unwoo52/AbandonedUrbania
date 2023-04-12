using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urban_KimHyeonWoo;

public class RobotHealthSystem : MonoBehaviour, ITestDamageSystem
{
    [SerializeField] float hp = 1000f;
    [SerializeField] GameObject Explosion;
    public void OnDam(float dmg)
    {
        if(dmg > hp)
        {
            SetGroggyState();
            Instantiate(Explosion);
        }
        hp -= dmg;
    }
    void SetGroggyState()
    {
        GetComponent<RobotBehavior>().ChangeFSM(RobotBehavior.RobotState.Destroy);
    }
}
