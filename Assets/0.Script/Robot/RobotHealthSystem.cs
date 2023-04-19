using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Urban_KimHyeonWoo;

public class RobotHealthSystem : MonoBehaviour, IDamageSystem
{
    [SerializeField] float hp = 1000f;
    [SerializeField] GameObject Explosion;
    bool isDie= false;
    public void OnDam(float dmg)
    {
        if(dmg > hp && isDie == false)
        {
            isDie = true;
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
