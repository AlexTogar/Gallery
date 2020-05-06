using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using static ColorCalculate;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

public class Initial : MonoBehaviour
{
    //external objects
    public GameObject pictureObject;
    public Material armchairMaterial;
    public Material badsideMaterialFirst;
    public Material badsideMaterialSecond;
    public Material badsideMaterialThird;
    public Material artMaterial;
    public Material pictureMaterial;
    public Material mainColorOne;
    public Material mainColorTwo;
    public Material mainColorThree;
    public GameObject directionalLight;
    public GameObject directionalDayLightPrefab;
    public GameObject directionalNightLightPrefab;
    public GameObject skyAndFogVolume;
    public GameObject canvas;
    public GameObject viewPortContent;
    public GameObject mainMenu;
    public GameObject selectPictureMenu;
    public GameObject openPicturesFolderMenu;
    public GameObject changeEnvironmentMenu;
    public GameObject searchMenu;
    public GameObject RowPrefab;
    public GameObject mainCamera;
    public InputField searchInputField;
    public bool lightUpdatingEnabled;
    public float sliderPosition = 1;
    protected System.Random rnd = new System.Random();
    protected List<GameObject> currentRows = new List<GameObject>();
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
    Picture currentPicture;
    List<Color> sliderColorList = new List<Color>();


    //VR BUTTONS
    [SerializeField] OVRInput.Button L;
    [SerializeField] OVRInput.Button J;
    [SerializeField] OVRInput.Button F1;
    [SerializeField] OVRInput.Controller controller;



    //===================== HELPFUL ADDITIONAL OWN METHODS ===========================


    public class Picture
    {
        public Texture2D texture { get; set; }
        public List<MainColor> mainColors { get; set; }
        public string name { get; set; }
        public string dataSaveFolderPath { get; set; }
        public int hash { get; set; }

        public Picture(Texture2D textureParam)
        {
            texture = textureParam;
            name = textureParam.name;
            dataSaveFolderPath = System.Environment.CurrentDirectory + "/Assets" + "/savedData";
            hash = GetTextureHash(textureParam);
            //if main colors of current picture not found => it will be calculated and saved in memory
            if (!LoadMainColors())
            {
                mainColors = GetMainColors(textureParam, 25, 4500);
                SaveMainColors();
                Debug.Log("metadata created and saved:" + name);
            }
            else
            {

            }

        }

        //Save information about current picture in memory
        public void SaveMainColors()
        {
            SerializableMainColors savingData = new SerializableMainColors(mainColors, GetTextureHash(texture));
            string savePath = dataSaveFolderPath + "/" + name + ".txt";

            var binaryFormatter = new BinaryFormatter();
            using(var fileStream = File.Create(savePath))
            {
                binaryFormatter.Serialize(fileStream, savingData);
            }

        }

        //Load information about current picture if is it possible (returns false if not)
        public bool LoadMainColors()
        {
            SerializableMainColors loadedMainColors;
            var files = new DirectoryInfo(dataSaveFolderPath).GetFiles("*.txt").ToList<FileInfo>();
            
            foreach (FileInfo fileInfo in files)
            {
                var binaryFormatter = new BinaryFormatter();
                using (var fileStream = File.Open( dataSaveFolderPath +"/"+ fileInfo.Name , FileMode.Open))
                {
                    loadedMainColors = (SerializableMainColors)binaryFormatter.Deserialize(fileStream);
                }

                if (loadedMainColors.hash == hash)
                {
                    mainColors = loadedMainColors.mainColors.ConvertAll(x => new MainColor(new Color(x.color[0], x.color[1], x.color[2]), x.capacity));
                    Debug.Log("metadata loaded:" + name);
                    return true;
                }

            }

            return false;
        }


        public Color GetLigthenColor()
        {
            return mainColors.Last().color;
        }

        public Color GetDarkenColor()
        {
            return mainColors.First().color;
        }

        public Color GetMediumColor()
        {
            return mainColors[1].color;
        }

        public Color GetMostFrequentColor()
        {
            return SortByCapacity(mainColors).Last().color;
        }

        public Color GetMediumFrequentColor()
        {
            return SortByCapacity(mainColors)[1].color;
        }

        public Color GetLeastFrequentColor()
        {
            return SortByCapacity(mainColors).First().color;
        }

        public int GetTextureHash(Texture2D texture)
        {

            //20 samples of pixels
            List<Color> pixels = new List<Color>();
            double compressionCoef = System.Math.Sqrt((double)texture.height * (double)texture.width / 20f);
            if (compressionCoef >= 1f)
            {
                pixels = TextureToColorsList(texture, (int)compressionCoef);
            }
            else
            {
                pixels = TextureToColorsList(texture, 1);
            }
            
            string hash = "";
            foreach (Color pixel in pixels)
            {
                hash += "(";
                hash += ((int)(pixel.r * 255f)).ToString() + ",";
                hash += ((int)(pixel.g * 255f)).ToString() + ",";
                hash += ((int)(pixel.b * 255f)).ToString();
                hash += ")";
                hash += " ";
            }

            return hash.GetHashCode();
        }

    }

    
    //Deactive all menu to show one from them after that
    public void DeactiveAllMenu()
    {
        mainMenu.SetActive(false);
        selectPictureMenu.SetActive(false);
        changeEnvironmentMenu.SetActive(false);
        openPicturesFolderMenu.SetActive(false);
        searchMenu.SetActive(false);
    }

    //Fill pictures list from /Assets/pictures/ folder with only .jpg and .png extensions
    void FillPicturesList()
    {
        pictures = new List<Picture>();
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/pictures/");
        List<FileInfo> jpgFiles = d.GetFiles("*.jpg").ToList<FileInfo>();
        List<FileInfo> pngFiles = d.GetFiles("*.png").ToList<FileInfo>();
        List<FileInfo> files = new List<FileInfo>();
        files.AddRange(pngFiles);
        files.AddRange(jpgFiles);
        foreach (FileInfo fileInfo in files)
        {
            Texture2D tex = LoadImage(fileInfo);
            if (CheckTextureRelation(tex, 1f))
            {
                pictures.Add(new Picture(tex));
            }
        }
        pictures = pictures.OrderBy(o => o.texture. name).ToList();
    }

    //Checking h/w relation with putted tolerance
    static bool CheckTextureRelation(Texture2D texture, float tolearnce = 0.1f)
    {
        float relation = (float)texture.width / (float)texture.height;
        float perfectRelation = 16 / 9f;
        if (relation <= perfectRelation + tolearnce && relation >= perfectRelation - tolearnce)
        {
            return true;
        } else
        {
            Debug.Log("Image with name" + texture.name + "doesn't fit with that width/height relation");
            return false;
        }
    }

    //Convertion from files to texture via path
    static Texture2D LoadImage(FileInfo filePath)
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
        //add next color to queue of directional light colors
        Color lightColor = ColorCalculate.LightUpdate(picture.GetLigthenColor(), lightUpdatingEnabled);
        setMainColorsPreview(picture.mainColors.ConvertAll<Color>(x => x.color));
        AddLightColorToQueue(lightColor);
        //set frame's color
        artMaterial.SetColor("_BaseColor", picture.GetLeastFrequentColor());
        //set additional armchair's color
        armchairMaterial.SetColor("_BaseColor", picture.GetDarkenColor());
        badsideMaterialFirst.SetColor("_BaseColor", picture.GetDarkenColor());
        badsideMaterialSecond.SetColor("_BaseColor", picture.GetDarkenColor());
        badsideMaterialThird.SetColor("_BaseColor", ColorCalculate.LightUpdate(picture.GetLeastFrequentColor(), true));
        Debug.Log(SortByCapacity(picture.mainColors).ConvertAll(x => x.color));
        currentPicture = picture;
        //FillColorListForSlider();

    }

    //Add color to the color of directional light queue
    public void AddLightColorToQueue(Color newColor, int iterations = 20)
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

    //Fill the additional pictures to show main colors in scene
    public void setMainColorsPreview(List<Color> colors)
    {
        mainColorOne.SetColor("_BaseColor", colors[0]);
        mainColorTwo.SetColor("_BaseColor", colors[1]);
        mainColorThree.SetColor("_BaseColor", colors[2]);
    }

    //Handle for the VR controller Start button pressing
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

    //Handle for the "Открыть папку" button in "Добавить/Удалить изобр." menu
    public void OpenPicturesFolder()
    {
        string path = System.Environment.CurrentDirectory + "\\Assets\\pictures";
        Process.Start("explorer.exe", path);
    }

    //Handle for the "Руководство" button in Main Menu
    public void OpenGuideFolder()
    {
        string path = System.Environment.CurrentDirectory + "\\Assets\\guide";
        Process.Start("explorer.exe", path);
    }

    //Set the texture of image to the picture's 3D model by name
    public void SetPictureByName(string pictureName)
    {
        // Texture2D newTexture = new Texture2D(1000, 1000);
        int index = 0;
        foreach (Picture picture in pictures)
        {

            if (picture.name == pictureName)
            {
                pictureMaterial.SetTexture("_BaseColorMap", picture.texture);
                setMainColorsPreview(picture.mainColors.ConvertAll<Color>(x => x.color));
                i = index;
                break;
            }
            index++;
        }
    }

    //Show next picutre in scene
    public void SetNextPicture()
    {
        i += 1;
        if (i > pictures.Count - 1) { i = 0; }
        pictureMaterial.SetTexture("_BaseColorMap", pictures[i].texture);
        ChangeEnvironmentByPicture(pictures[i]);
        setMainColorsPreview(pictures[i].mainColors.ConvertAll<Color>(x => x.color));
    }

    //Show previous picture in scene
    public void SetPreviousPicture()
    {
        i -= 1;
        if (i < 0) { i = pictures.Count - 1; }
        pictureMaterial.SetTexture("_BaseColorMap", pictures[i].texture);
        ChangeEnvironmentByPicture(pictures[i]);
        setMainColorsPreview(pictures[i].mainColors.ConvertAll<Color>(x => x.color));

    }

    public void QuitGame()
    {
        //for application
        Application.Quit();
        //for unity editor
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    //create pictures, savedData and guide folders if they are not exist
    public void CreateRequiredFolders()
    {
        System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "/Assets" + "/savedData");
        System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "/Assets" + "/pictures");
        System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "/Assets" + "/guide");
    }

    //Fill select list of pictures in menu "Выбрать изображение"
    public void FillSelectListOfPictures(List<Picture> pictures, string searchString = "")
    {
        //remove all old rows
        if (currentRows.Count() != 0)
        {
            foreach (GameObject row in currentRows)
            {
                Destroy(row);
            }
        }


        //add new Rows
        int j = 0;

        currentRows = new List<GameObject>();
        foreach (Picture picture in pictures)
        {
            if (picture.name.ToLower().Contains(searchString.ToLower()) || searchString == "")
            {
                GameObject Row = Instantiate(RowPrefab);
                Row.transform.SetParent(viewPortContent.transform);
                //set parent's position and rotation params
                Row.transform.rotation = viewPortContent.transform.rotation;
                Row.transform.position = viewPortContent.transform.position;
                //reserve space for header
                Row.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 - (100 * j));
                Row.name = "Row" + j.ToString();
                //set image name
                Row.GetComponentsInChildren<Text>()[0].text = picture.name;
                //set preview image
                Row.GetComponentsInChildren<Image>()[0].sprite = Sprite.Create(picture.texture, new Rect(0, 0, picture.texture.width, picture.texture.height), Vector2.zero);
                //set OnClick handler
                Row.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => { SetPictureByName(picture.name); ChangeEnvironmentByPicture(picture); });
                j += 1;
                Row.transform.localScale = new Vector3(1, 1, 1);
                currentRows.Add(Row);
            }

        }
    }

    //Set color of the Directional Light instantly
    public void SetLightColor(Color color)
    {
        GameObject day = GameObject.Find("DayDirectionalLight");
        GameObject night = GameObject.Find("NightDirectionalLight");
        Color nextColor = currentLightColorQueue.Dequeue();
        day.GetComponent<Light>().color = nextColor;
        night.GetComponent<Light>().color = nextColor;
    }


    //Handle for the button "Обновить список"
    public void ReloadPicturesFromFolder(string str)
    {
        FillPicturesList();
        pictureMaterial.SetTexture("_BaseColorMap", pictures[0].texture);
        ChangeEnvironmentByPicture(pictures[0]);
        if (str == "reload all") {
            FillSelectListOfPictures(pictures);

        } else
        {
            FillSelectListOfPictures(pictures, searchInputField.text);
        }

    }

    
    //===================== BUILT-IN UNITY METHODS ===========================

    // Start is called before the first frame update
    void Start()
    {
        CreateRequiredFolders();
        //disable main directional light (there is two other ones)
        directionalLight.GetComponent<Light>().enabled = false;
        //Hide main menu
        canvas.SetActive(false);

        FillPicturesList();

        //Set default picture and environment
        pictureMaterial.SetTexture("_BaseColorMap", pictures[0].texture);
        ChangeEnvironmentByPicture(pictures[0]);
        //allocate space in the Content object for header and rows
        viewPortContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * pictures.Count + 70);
        FillSelectListOfPictures(pictures);
        //FillColorListForSlider();

    }



    // Update is called once per frame
    void Update()
    {


        //set directional light color according to current picture
        if (currentLightColorQueue.Count() != 0)
        {
            SetLightColor(currentLightColorQueue.Dequeue());
        }


        //set directional light color according to UI slider bar
        if (sliderColorList.Count() != 0)
        {
            //UpdateLightColorAccordingToSlider(sliderPosition);
        }


        //handler of the l pressing (next picture)
        if ((Input.GetKeyDown("l") || OVRInput.GetDown(L,controller)) && lFlag == false )
        {
            lFlag = true;
            SetNextPicture();
        }
        else
        {
            lFlag = false;
        }

        //handler of the j pressing (prev picture)
        if ((Input.GetKeyDown("j") || OVRInput.GetDown(J, controller)) && jFlag == false)
        {
            jFlag = true;
            SetPreviousPicture();
        }
        else
        {
            jFlag = false;
        }

        //handler of the f1 pressing (Pause/Resume)
        if ((Input.GetKeyDown(KeyCode.F1) || OVRInput.GetDown(F1, controller)) && f1Flag == false)
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
