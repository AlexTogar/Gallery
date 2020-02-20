using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] float angle;
    public abstract class ClapanController : MonoBehaviour
    {
        public GameObject RotatingPart;
        public HingeJoint hingeJoint;

        public ClapanController(GameObject targetObj)
        {
            RotatingPart = targetObj;
            hingeJoint = RotatingPart.GetComponent<HingeJoint>();
        }
    }

    public class ClapanBase : ClapanController
    {
        Controller controller;
        public ClapanBase(GameObject targetObj, Controller Controller)
        : base(targetObj)
        {
            controller = Controller;
        }

        public void DisconnectClapan()
        {
            if (hingeJoint != null)
            {
                if (Mathf.Abs(hingeJoint.angle - hingeJoint.limits.min) < 5)
                {
                    Destroy(hingeJoint);
                    controller.ClapanDisconnected();
                    Hand.AttachmentFlags attachmentFlags = RotatingPart.GetComponent<ComplexThrowable>().attachmentFlags;
                    Destroy(RotatingPart.GetComponent<ComplexThrowable>());
                    RotatingPart.AddComponent<Throwable>().attachmentFlags = RotatingPart.GetComponent<ComplexThrowable>().attachmentFlags = attachmentFlags;
                }
            }            
        }
    }

    public class ControllerRotate : ClapanController
    {
        public ControllerRotate(GameObject targetObj)
        : base(targetObj)
        {

        }

        public float RotatePercent;

        public void CalculateFromAngle()
        {
            RotatePercent = Mathf.Abs(hingeJoint.angle - hingeJoint.limits.min)/145;
        }
    }

    public class ClapanRotate : ClapanController
    {
        public ClapanRotate(GameObject targetObj)
        : base(targetObj)
        {

        }
    }

    [HideInInspector] public ClapanBase clapanBase;
    [HideInInspector] public ControllerRotate controllerRotate;
    [HideInInspector] public ClapanRotate clapanRotate;
    [SerializeField] GameObject ControllerRotateObj;
    [SerializeField] GameObject ClapanBaseObj;
    [SerializeField] GameObject ClapanRotateObj;
    [SerializeField] GameObject ClapanCurrent;
    [SerializeField] GameObject ClapanCopy;
    [SerializeField] GameObject ClapanPlacer;




    // Start is called before the first frame update
    void Start()
    {
        ClapanPlacer.SetActive(false);
        clapanBase = new ClapanBase(ClapanBaseObj, this);
        controllerRotate = new ControllerRotate(ControllerRotateObj);
        clapanRotate = new ClapanRotate(ClapanRotateObj);

    }

    void ClapanDisconnected()
    {
        Hand.AttachmentFlags attachmentFlags = clapanRotate.RotatingPart.GetComponent<ComplexThrowable>().attachmentFlags;
        Destroy(clapanRotate.RotatingPart.GetComponent<ComplexThrowable>());
        clapanRotate.RotatingPart.AddComponent<Throwable>().attachmentFlags = clapanRotate.RotatingPart.GetComponent<ComplexThrowable>().attachmentFlags = attachmentFlags; Invoke("ActivateClapan", 2f);
        Invoke("ActivateClapan", 2f);
    }

    void ActivateClapan()
    {
        ClapanPlacer.SetActive(true);
    }

    public void SnapClapan()
    {        
        ClapanPlacer.SetActive(false);
        Destroy(ClapanCurrent);
        ClapanCurrent = ClapanCopy;
        ClapanCurrent.SetActive(true);
        ClapanBaseObj = ClapanCurrent.transform.GetChild(0).gameObject;
        clapanBase = new ClapanBase(ClapanBaseObj, this);
        ClapanCopy = Instantiate(ClapanCurrent, ClapanCurrent.transform.position, ClapanCurrent.transform.rotation, ClapanCurrent.transform.parent);
        ClapanPlacer.GetComponent<SnapOnPos>().targetObj = ClapanCurrent.transform.GetChild(0).gameObject;
        ClapanCopy.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        clapanBase.DisconnectClapan();
        controllerRotate.CalculateFromAngle();
        angle = controllerRotate.RotatePercent;
    }
}
