using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GlowNearGrab : MonoBehaviour
{
    [SerializeField] Material GlowMaterial;
    Material StartMaterial;

    private void Start()
    {
        StartMaterial = GetComponent<Renderer>().material;
    }

    public void GlowControll(bool isGlowing)
    {
        if (isGlowing)
        {
            Material material = new Material(GlowMaterial);
            material.SetTexture("_Albedo", StartMaterial.GetTexture("_BaseColorMap"));
            //material.SetTexture("_Metallic", StartMaterial.GetTexture("_Metallic"));
            material.SetTexture("_Normal", StartMaterial.GetTexture("_NormalMap"));
            GetComponent<MeshRenderer>().material = material;

        }
        else
        {
            GetComponent<Renderer>().material = StartMaterial;
        }
    }

}
