using System.Diagnostics;
using System.IO;
using UnityEngine;
public class Initial : MonoBehaviour

{
    public Material picture;
    // Start is called before the first frame update
    void Start()
    {
        Texture texture = picture.GetTexture("_BaseColorMap");
        print(texture.name);
        Texture2D texture2d = texture as Texture2D;
        byte [] image = texture2d.EncodeToJPG();

        using (MemoryStream mStream = new MemoryStream(image))
        {
            System.Drawing.Image.FromStream(mStream).Save("my_image.jpg");
        }

        print(texture2d.GetPixel(0, 0));

        //start ruby script
        Process.Start("cmd.exe", "/C ruby ruby_script.rb");

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
