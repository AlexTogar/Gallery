using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;


public class IpadControll : MonoBehaviour
{
    List<Texture2D> textures = new List<Texture2D>();
    [SerializeField] Material Screen;
    [SerializeField] VideoPlayer videoPlayer;
    List<FileInfo> videoClipsUrl = new List<FileInfo>();
    [SerializeField] Transform ButtonPlaces;
    [SerializeField] GameObject IPadButton;
    [SerializeField] GameObject WifiButton;
    [SerializeField] List<Controller> controllers;
    [SerializeField] List<Material> materials;
    [SerializeField] Transform arrow;
    int index;
    int indexController;
    bool PumpSetting;
    //[SerializeField] float percents;
    
    
    // Start is called before the first frame update
    void Start()
    {
        PumpSetting = false;
        arrow.gameObject.SetActive(false);
        IPadButton.SetActive(true);
        WifiButton.SetActive(false);
        videoPlayer.enabled = false;
        index = 1;
        indexController = 0;
        FillImageList();
        FillVideoArray();
        PlaceButton();
        Screen.SetTexture("_BaseColorMap", textures[0]);
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        IPadButton.SetActive(true);
    }

    void FillImageList()
    {
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/IpadMedia/" + "/images/");
        FileInfo[] files = d.GetFiles("*.png");
        foreach (FileInfo fileInfo in files)
        {
            textures.Add(LoadPNG(fileInfo));
        }
    }

    static Texture2D LoadPNG(FileInfo filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath.FullName))
        {
            fileData = File.ReadAllBytes(filePath.FullName);
            tex = new Texture2D(2, 2);
            tex.name = Path.GetFileNameWithoutExtension(filePath.Name);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }


    void FillVideoArray()
    {
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/IpadMedia/" + "/video/");
        FileInfo[] files = d.GetFiles("*.mp4");
        //videoClipsUrl.AddRange(d.GetFiles("*.mp4"));
        videoClipsUrl.AddRange(files);
    }



    public void NextFile()
    {
        arrow.gameObject.SetActive(false);
        if (Screen.GetTexture("_BaseColorMap").name == index.ToString() + "--")
        {
            arrow.gameObject.SetActive(true);
            indexController++;
        }
        index++;
        PumpSetting = false;
        IPadButton.SetActive(true);
        WifiButton.SetActive(false);
        videoPlayer.enabled = false;
        foreach (Texture texture in textures)
        {
            if(texture.name == index.ToString())
            {
                Screen.SetTexture("_BaseColorMap", texture);
                PlaceButton();
                break;
            }
            if (texture.name == index.ToString() + "+")
            {
                Screen.SetTexture("_BaseColorMap", texture);
                IPadButton.SetActive(false);
                WifiButton.SetActive(true);
                break;
            }
            if (texture.name == index.ToString() + "-")
            {
                Screen.SetTexture("_BaseColorMap", texture);
                PumpSetting = true;
                arrow.gameObject.SetActive(true);
                IPadButton.SetActive(false);
                break;
            }
        }

        if (Screen.GetTexture("_BaseColorMap").name != index.ToString())
        {
            foreach (FileInfo fileInfo in videoClipsUrl)
            {
                if (Path.GetFileNameWithoutExtension(fileInfo.Name) == index.ToString())
                {
                    PlaceButton();
                    IPadButton.SetActive(false);
                    videoPlayer.enabled = true;
                    LoadVideo(fileInfo.FullName.Replace("\\", "/"));
                    break;
                }
            }
        }
    }

    void PlaceButton()
    {
        Vector3 vector3 = IPadButton.transform.GetChild(0).transform.position - ButtonPlaces.transform.Find(index.ToString()).transform.position;
        IPadButton.transform.position -= vector3;
        IPadButton.transform.GetChild(0).transform.localScale = ButtonPlaces.transform.Find(index.ToString()).transform.localScale;
    }

    public void LoadVideo(string Url)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Url;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    {
        videoPlayer.Play();
    }

    void ArrowControll()
    {
        float rotationAngle = controllers[indexController].controllerRotate.RotatePercent;
        Vector3 vector3 = new Vector3(0,-110 + rotationAngle * 220,0);
        arrow.localEulerAngles = vector3;
        if (arrow.localRotation.y > 0.71 || arrow.localRotation.y < -0.72)
        {
            foreach(Renderer renderer in arrow.GetComponentsInChildren<Renderer>())
            {
                renderer.material = materials[0];
            }
        }
        else
        {
            if (arrow.localRotation.y > 0.45 || arrow.localRotation.y < -0.5)
            {
                foreach (Renderer renderer in arrow.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = materials[1];
                }
            }
            else
            {
                foreach (Renderer renderer in arrow.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = materials[2];
                }
            }
        }
        if (rotationAngle > 0.4 && rotationAngle < 0.6)
        {
            foreach (Texture texture in textures)
            {
                if (texture.name == index.ToString() + "--")
                {
                    if (Screen.GetTexture("_BaseColorMap") != texture)
                    {
                        Screen.SetTexture("_BaseColorMap", texture);
                        IPadButton.SetActive(true);
                    }
                }
            }
        }
        else
        {
            foreach (Texture texture in textures)
            {
                if (texture.name == index.ToString() + "-")
                {
                    if (Screen.GetTexture("_BaseColorMap") != texture)
                    {
                        Screen.SetTexture("_BaseColorMap", texture);
                        IPadButton.SetActive(true);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PumpSetting)
        {
            ArrowControll();
        }
        ArrowControll();

    }
}
