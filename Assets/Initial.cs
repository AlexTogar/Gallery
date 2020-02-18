using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UnityEngine;
using Color = UnityEngine.Color;

public class Initial : MonoBehaviour
{
    public Material picture;
    public GameObject directionalLight;
    public GameObject skyAndFogVolume;

    protected System.Random rnd = new System.Random();
    //current index of showing picture
    protected int i = 0;
    //flag of space pressing
    protected bool space_flag = false;
    //textures list, which will be filled by FillTexturesList() method, called in Start() method
    protected List<Texture2D> textures = new List<Texture2D>();
    //protected List<System.Drawing.Bitmap> bitmaps = new List<Bitmap>();
    //===================== HELPFUL ADDITIONAL OWN METHODS ===========================

    /* void FillBitmapsList()
    {
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/pictures/");
        FileInfo[] files = d.GetFiles("*.jpg");
        foreach (FileInfo fileInfo in files)
        {
            bitmaps.Add(ConvertToBitmap(fileInfo.FullName));
        }
    }
    */

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

    //Convert file via FileName to bitmap
    /*public Bitmap ConvertToBitmap(string fileName)
    {
        Bitmap bitmap;
        using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
        {
            Image image = Image.FromStream(bmpStream);

            bitmap = new Bitmap(image);

        }
        return bitmap;
    }
    */

    public void SetLightColor(Color color)
    {
        directionalLight.GetComponent<Light>().color = color;

    }

    public void SetFogColor(Color color)
    {
        RenderSettings.fogColor = color;
    }


    public void ChangeEnvironmentColor(Color color)
    {
        SetLightColor(color);
        //SetFogColor(color);
    }

    //===================== UNITY'S BUILT-IN METHODS ===========================

    // Start is called before the first frame update
    void Start()
    {
        FillTexturesList();
        //FillBitmapsList();

        //print(bitmaps);
        /*set first texture to picture object
          textures sorted by name in pictures folder */

        picture.SetTexture("_BaseColorMap", textures[0]);
        
        //Process.Start("cmd.exe", "/C python python_script.py");

    }



    // Update is called once per frame
    void Update()
    {
        //handler of space pressing
        if (Input.GetKeyDown("l") && space_flag == false)
        {
            space_flag = true;

            picture.SetTexture("_BaseColorMap", textures[i]);
            ChangeEnvironmentColor(textures[i].GetPixel((int)(rnd.Next()*textures[i].height), (int)(rnd.Next()*textures[i].width)));
            
            i += 1;
            if (i > textures.Count - 1) { i = 0; }
        }
        else
        {
            space_flag = false;
        }


    }
}
