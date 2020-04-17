using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revert : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject floor;
    Vector3 startPosition = new Vector3();
    Quaternion StartRotation = new Quaternion();

    void Start()
    {
        startPosition = gameObject.transform.position;
        StartRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("fsdf");
        if (collision.gameObject.name == floor.name)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.transform.position = startPosition;
            gameObject.transform.rotation = StartRotation;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

        }
    }
}
