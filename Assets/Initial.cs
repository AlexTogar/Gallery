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

public class Initial : MonoBehaviour
{
    public Material picture;
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
    protected List<Texture2D> textures = new List<Texture2D>();
    //protected List<System.Drawing.Bitmap> bitmaps = new List<Bitmap>();
    //===================== HELPFUL ADDITIONAL OWN METHODS ===========================

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
            textures.Add(LoadJPG(fileInfo));
        }
        textures = textures.OrderBy(o => o.name).ToList();
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

    public void ChangeEnvironmentByTexture(Texture2D texture)
    {
        Color mainColor = GetMainColor(texture);
        ChangeEnvironmentColor(mainColor);
        SetDirectionalLightPosition(mainColor);//emtpy function yet
    }

    //rewrite it
    public Color GetMainColor(Texture2D texture)
    {
        return texture.GetPixel((int)(Random.Range(0, 1f) * texture.height), (int)(Random.Range(0, 1f) * texture.width));
    }

    public void SetLightColor(Color color)
    {
        directionalLight.GetComponent<Light>().color = color;

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
        foreach (Texture2D texture in textures)
        {

            if (texture.name == pictureName)
            {
                picture.SetTexture("_BaseColorMap", texture);
                i = index;
                break;
            }
            index++;
        }
    }

    public void SetNextPicture()
    {
        i += 1;
        if (i > textures.Count - 1) { i = 0; }
        picture.SetTexture("_BaseColorMap", textures[i]);
        ChangeEnvironmentByTexture(textures[i]);
    }

    public void SetPreviousPicture()
    {
        i -= 1;
        if (i < 0) { i = textures.Count - 1; }
        picture.SetTexture("_BaseColorMap", textures[i]);
        ChangeEnvironmentByTexture(textures[i]);
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
        picture.SetTexture("_BaseColorMap", textures[0]);
        ChangeEnvironmentByTexture(textures[0]);
        //allocate space in the Content object for header and rows
        viewPortContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * textures.Count + 70);
        int j = 0;
        //create select list of pictures
        foreach (Texture2D texture in textures)

        {
            
            GameObject Row = Instantiate(RowPrefab);
            Row.transform.SetParent(viewPortContent.transform);
            Row.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 - (100*j));
            Row.name = "Row" + i.ToString();
            //set image name
            Row.GetComponentsInChildren<Text>()[0].text = texture.name;
            //set preview image
            Row.GetComponentsInChildren<Image>()[0].sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            //set OnClick handler
            Row.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => { SetPictureByName(texture.name); ChangeEnvironmentByTexture(texture); });
            j += 1;
        }

        
    }



    // Update is called once per frame
    void Update()
    {
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
