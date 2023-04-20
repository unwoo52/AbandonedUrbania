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
    public void OnDam(float dmg, GameObject gameObject)
    {
        if (gameObject == this.gameObject) return;
        if(dmg > hp && isDie == false)
        {
            isDie = true;
            SetGroggyState();
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }
        hp -= dmg;
    }
    void SetGroggyState()
    {
        GetComponent<RobotBehavior>().ChangeFSM(RobotBehavior.RobotState.Destroy);
    }
}
