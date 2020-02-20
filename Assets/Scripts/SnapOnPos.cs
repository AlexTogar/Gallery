using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SnapOnPos : MonoBehaviour
{
    public GameObject targetObj;
    [SerializeField] Controller Controller;

    private void Start()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        targetObj.GetComponent<Interactable>().onAttachedToHand += SnapOnPos_onAttachedToHand;
        targetObj.GetComponent<Interactable>().onDetachedFromHand += SnapOnPos_onDetachedFromHand;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(targetObj == other.gameObject)
        {
            Controller.SnapClapan();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SnapOnPos_onDetachedFromHand(Hand hand)
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void SnapOnPos_onAttachedToHand(Hand hand)
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
