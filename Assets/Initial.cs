using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initial : MonoBehaviour
{
    public GameObject picture;
    // Start is called before the first frame update
    void Start()
    {
        picture = GameObject.Find("picture");
        picture = picture.GetComponent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
