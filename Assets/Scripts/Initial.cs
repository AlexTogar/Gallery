using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplateProjects;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

public class Initial : MonoBehaviour
{
    //external objects
    public Material pictureMaterial;
    public Material mainColorOne;
    public Material mainColorTwo;
    public Material mainColorThree;
    public GameObject directionalLight;
    public GameObject skyAndFogVolume;
    public GameObject canvas;
    public GameObject viewPortContent;
    public GameObject mainMenu;
    public GameObject selectPictureMenu;
    public GameObject changeEnvironmentMenu;
    public GameObject RowPrefab;
    public GameObject mainCamera;
    protected System.Random rnd = new System.Random();
    //current index of showing picture
    protected int i = 0;
    //flags of buttons pressing
    protected bool lFlag = false;
    protected bool jFlag = false;
    protected bool f1Flag = false;
    //textures list, which will be filled by FillTexturesList() method, called in Start() method
    protected List<Picture> pictures = new List<Picture>();
    Queue<Color> currentLightColorQueue = new Queue<Color>();
    Color supportColorPoint = new Color();
    //===================== HELPFUL ADDITIONAL OWN METHODS ===========================

    public class Picture
    {
        public Texture2D texture { get; set; }
        public List<Color> mainColors { get; set; }
        public string envType { get; set; }
        public string name { get; set; }
        public Picture(Texture2D tex, List<Color> mainCol, string envT = "default")
        {
            texture = tex;
            mainColors = mainCol;
            envType = envT;
            name = tex.name;
        }

        public Picture setMainColors()
        {
            return new Picture(texture, ColorCalculate.GetMainColors(texture, 25), envType);
        }

        public Color GetLigthenColor()
        {
            return mainColors.Last();
        }

        public Color GetDarkenColor()
        {
            return mainColors.First();
        }

        public Color GetMediumColor()
        {
            return mainColors[1];
        }


    }

    //Deactive all menu to show one from them after that
    public void DeactiveAllMenu()
    {
        mainMenu.SetActive(false);
        selectPictureMenu.SetActive(false);
        changeEnvironmentMenu.SetActive(false);
    }
    //fill textures list from /Assets/pictures/ folder with .jpg extension
    void FillTexturesList()
    {
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/pictures/");
        FileInfo[] files = d.GetFiles("*.jpg");
        foreach (FileInfo fileInfo in files)
        {
            Texture2D tex = LoadJPG(fileInfo);
            pictures.Add(new Picture(tex, ColorCalculate.GetMainColors(tex, 25)));
        }
        pictures = pictures.OrderBy(o => o.texture.name).ToList();
    }

    //Convertion from .jgp to texture
    static Texture2D LoadJPG(FileInfo filePath)
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

    public void ChangeEnvironmentByPicture(Picture picture)
    {
        ChangeEnvironmentColor(picture.GetLigthenColor());
        SetDirectionalLightPosition(picture.GetLigthenColor());//emtpy function yet

    }


    public void SetLightColor(Color newColor, int iterations = 30)
    {
        Color oldColor = supportColorPoint;

        //Fill the colors Queue
        List<float> colorDiffs = new List<float>();
        colorDiffs.Add(newColor.r - oldColor.r);
        colorDiffs.Add(newColor.g - oldColor.g);
        colorDiffs.Add(newColor.b - oldColor.b);


        for (int i = 0; i < iterations; i++)
        {
            currentLightColorQueue.Enqueue(new Color(   oldColor.r + (colorDiffs[0] / iterations) * i,
                                                        oldColor.g + (colorDiffs[1] / iterations) * i,
                                                        oldColor.b + (colorDiffs[2] / iterations) * i));
        }

        supportColorPoint = newColor;

    }

    public void SetFogColor(Color color)
    {
        RenderSettings.fogColor = color;  //doesn't work yet
    }

    public void SetDirectionalLightPosition(Color color)
    {
        
    }


    public void ChangeEnvironmentColor(Color color)
    {
        SetLightColor(color);
        SetFogColor(color);
    }

    public void setMainColorsPreview(List<Color> colors)
    {
        mainColorOne.SetColor("_BaseColor", colors[0]);
        mainColorTwo.SetColor("_BaseColor", colors[1]);
        mainColorThree.SetColor("_BaseColor", colors[2]);

    }

    public void SetRandomEnvironmentColor()
    {
        ChangeEnvironmentColor(new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)));
    }

    public void ResumeOrPauseGame()
    {
        if (canvas.active)
        {
            canvas.SetActive(false);
        }
        else
        {
            DeactiveAllMenu();
            //show exactly main menu
            mainMenu.SetActive(true);
            canvas.SetActive(true);
        }
    }

    public void SetPictureByName(string pictureName)
    {
        // Texture2D newTexture = new Texture2D(1000, 1000);
        int index = 0;
        foreach (Picture picture in pictures)
        {

            if (picture.name == pictureName)
            {
                pictureMaterial.SetTexture("_BaseColorMap", picture.texture);
                setMainColorsPreview(picture.mainColors);
                i = index;
                break;
            }
            index++;
        }
    }

    public void SetNextPicture()
    {
        i += 1;
        if (i > pictures.Count - 1) { i = 0; }
        pictureMaterial.SetTexture("_BaseColorMap", pictures[i].texture);
        ChangeEnvironmentByPicture(pictures[i]);
        setMainColorsPreview(pictures[i].mainColors);
    }

    public void SetPreviousPicture()
    {
        i -= 1;
        if (i < 0) { i = pictures.Count - 1; }
        pictureMaterial.SetTexture("_BaseColorMap", pictures[i].texture);
        ChangeEnvironmentByPicture(pictures[i]);
        setMainColorsPreview(pictures[i].mainColors);

    }

    public void QuitGame()
    {
        //for application
        Application.Quit();
        //for unity editor
        UnityEditor.EditorApplication.isPlaying = false;
    }


    //===================== BUILT-IN METHODS ===========================

    // Start is called before the first frame update
    void Start()
    {
        //Hide main menu
        canvas.SetActive(false);

        FillTexturesList();

        //Set default picture and environment
        pictureMaterial.SetTexture("_BaseColorMap", pictures[0].texture);
        ChangeEnvironmentByPicture(pictures[0]);
        //allocate space in the Content object for header and rows
        viewPortContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * pictures.Count + 70);
        int j = 0;
        //create select list of pictures
        foreach (Picture picture in pictures)

        {
            
            GameObject Row = Instantiate(RowPrefab);
            Row.transform.SetParent(viewPortContent.transform);
            Row.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 - (100*j));
            Row.name = "Row" + i.ToString();
            //set image name
            Row.GetComponentsInChildren<Text>()[0].text = picture.name;
            //set preview image
            Row.GetComponentsInChildren<Image>()[0].sprite = Sprite.Create(picture.texture, new Rect(0, 0, picture.texture.width, picture.texture.height), Vector2.zero);
            //set OnClick handler
            Row.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => { SetPictureByName(picture.name); ChangeEnvironmentByPicture(picture); });
            Debug.Log("ksjdfljksa");
            j += 1;
        }

        
    }



    // Update is called once per frame
    void Update()
    {
        if (currentLightColorQueue.Count() != 0)
        {
            directionalLight.GetComponent<Light>().color = currentLightColorQueue.Dequeue();
        }


        //handler of the l pressing (next picture)
        if (Input.GetKeyDown("l") && lFlag == false)
        {
            lFlag = true;
            SetNextPicture();
        }
        else
        {
            lFlag = false;
        }

        //handler of the j pressing (prev picture)
        if (Input.GetKeyDown("j")&& jFlag == false)
        {
            jFlag = true;
            SetPreviousPicture();
        }
        else
        {
            jFlag = false;
        }

        //handler of the f1 pressing (Pause/Resume)
        if (Input.GetKeyDown(KeyCode.F1) && f1Flag == false)
        {
            f1Flag = true;
            ResumeOrPauseGame();
        }
        else
        {
            f1Flag = false;
        }


    }
}
