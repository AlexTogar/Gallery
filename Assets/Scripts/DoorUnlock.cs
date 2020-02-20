using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    JointLimits jointLimits;
    HingeJoint hinge;
    [SerializeField] FixedJoint fixedJoint;

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        jointLimits = hinge.limits;
    }

    // Update is called once per frame
    void Update()
    {
        //if(hinge.angle <= jointLimits.min || hinge.angle >= jointLimits.max)
        //{
         //   Destroy(fixedJoint);
        //}
        if (Mathf.Abs(hinge.angle - jointLimits.min) < 1 || Mathf.Abs(hinge.angle - jointLimits.max) < 1)
        {
            Destroy(fixedJoint);
        }
    }
}
