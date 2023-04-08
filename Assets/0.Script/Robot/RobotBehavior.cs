using Lightbug.CharacterControllerPro.Demo;
using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    [SerializeField] float WeaponArrange = 100;
    [SerializeField] GameObject Target;
    [SerializeField] AIFollowBehaviour aIFollowBehaviour;
    RobotActions robotActions;
    robotWeaponSystem robotWeaponSystem; 

    private void Start()
    {
        robotActions = GetComponent<RobotActions>();
        robotWeaponSystem = GetComponent<robotWeaponSystem>();
    }
    private void Update()
    {
        robotActions.looktarget(Target.transform.position);

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance < WeaponArrange)
        {
            AttackTarget();
        }
        else
        {
            TrackingTarget();
        }
    }

    private void TrackingTarget()
    {
        aIFollowBehaviour.IsTrackingState = true;
    }

    private void AttackTarget()
    {
        aIFollowBehaviour.IsTrackingState = false;
        robotWeaponSystem.AllWeaponFire(Target.transform.position);
    }
}
