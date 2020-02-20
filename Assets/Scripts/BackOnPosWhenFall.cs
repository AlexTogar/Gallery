using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackOnPosWhenFall : MonoBehaviour
{
    [SerializeField] Controller controller;
    [SerializeField] Transform position;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.name == "Floor" || collision.gameObject.name == "Floor_Unwrapped")
        {

            if (controller == null)
            {
                gameObject.transform.position = position.position;
                gameObject.transform.rotation = position.rotation;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                controller.SnapClapan();
            }
        }
    }

}
