using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
public class Initial : MonoBehaviour

{
    public Material picture;
    //current index of showing picture
    public int i = 1;
    //flag of space pressing
    public bool space_flag = false;
    List<Texture2D> textures = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        
        Texture texture = picture.GetTexture("_BaseColorMap");
        Texture2D texture2d = texture as Texture2D;
        FillImageList();
        picture.SetTexture("_BaseColorMap", textures[0]);

        //texture2d.Resize(200, 200);
        //texture2d.Apply();

        //start ruby script
        //Process.Start("cmd.exe", "/C ruby ruby_script.rb");

    }

    void FillImageList()
    {
        string currentFolderPath = System.Environment.CurrentDirectory;
        DirectoryInfo d = new DirectoryInfo(currentFolderPath + "/Assets/" + "/pictures/");
        FileInfo[] files = d.GetFiles("*.jpg");
        foreach (FileInfo fileInfo in files)
        {
            textures.Add(LoadJPG(fileInfo));
        }
    }

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

    // Update is called once per frame
    void Update()
    {
        // change picture by pressing space
        if (Input.GetKeyDown("space") && space_flag == false)
        {
            space_flag = true;
            i = i + 1;
            if (i > 3)
            {
                i = 1;
            }
            //handler of space pressing
            /*
            string picture_path = Application.dataPath + "/pictures/picture" + i.ToString();
            Texture2D new_texture2d = Resources.Load(picture_path, typeof(Texture2D)) as Texture2D;
            picture.SetTexture("_MainTex", new_texture2d);
            */
            
            picture.SetTexture("_BaseColorMap", textures[i-1]);
        }
        else
        {

        }
        {
            space_flag = false;
        }
    }
}
