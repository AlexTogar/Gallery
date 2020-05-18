using UnityEngine;

//Applies to object which should to teleport to initial spot if it fall to the ground
public class DropTreatment : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject floor;
    Vector3 startPosition = new Vector3();
    Quaternion StartRotation = new Quaternion();

    void Start()
    {
        //Remember the initial position
        startPosition = gameObject.transform.position;
        StartRotation = gameObject.transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if the object thouches ground
        if (collision.gameObject.name == floor.name)
        {
            //Teleport object to initial spot
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.transform.position = startPosition;
            gameObject.transform.rotation = StartRotation;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

        }
    }
}
