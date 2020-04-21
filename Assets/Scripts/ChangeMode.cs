using UnityEngine;
using UnityEngine.Rendering;

public class ChangeMode : MonoBehaviour
{
    // Start is called before the first frame update
    public  GameObject dayModeObj;
    public  GameObject nightModeObj;
    public  GameObject spotLight;
    public  GameObject lightOfPicture;
    public  GameObject directionalLight;
    public  GameObject directionalDayLightPrefab;
    public  GameObject directionalNightLightPrefab;

    GameObject dayDirecationalLight;
    GameObject nightDirectionalLight;

    VolumeProfile dayProfile;


    void Start()
    {
        //Instantiate new lights from prefab
        dayProfile = dayModeObj.GetComponent<Volume>().profile;
        dayDirecationalLight = Instantiate(directionalDayLightPrefab);
        nightDirectionalLight = Instantiate(directionalNightLightPrefab);
        dayDirecationalLight.name = "DayDirectionalLight";
        nightDirectionalLight.name = "NightDirectionalLight";   

        dayDirecationalLight.transform.SetParent(GameObject.Find("GameObject").transform);
        nightDirectionalLight.transform.SetParent(GameObject.Find("GameObject").transform);

        dayDirecationalLight.SetActive(true);
        nightDirectionalLight.SetActive(true);
        //directionalLight.SetActive(false);
        dayDirecationalLight.GetComponent<Light>().enabled = false;

        //nightDirectionalLight.SetActive(false);
        nightDirectionalLight.GetComponent<Light>().enabled = false;
        //dayDirecationalLight.SetActive(true);
        dayDirecationalLight.GetComponent<Light>().enabled = true;

    }

    public  void SetNightMode()
    {
        dayModeObj.GetComponent<Volume>().profile = nightModeObj.GetComponent<Volume>().profile;
        //nightDirectionalLight.SetActive(true);
        //dayDirecationalLight.SetActive(false);
        nightDirectionalLight.GetComponent<Light>().enabled = true;
        dayDirecationalLight.GetComponent<Light>().enabled = false;

        lightOfPicture.SetActive(true);
        spotLight.SetActive(false);
    }

    public  void SetDayMode()
    {
        dayModeObj.GetComponent<Volume>().profile = dayProfile;
        //nightDirectionalLight.SetActive(false);
        //dayDirecationalLight.SetActive(true);
        nightDirectionalLight.GetComponent<Light>().enabled = false;
        dayDirecationalLight.GetComponent<Light>().enabled = true;

        lightOfPicture.SetActive(false);
        spotLight.SetActive(true);
    }

    
}
