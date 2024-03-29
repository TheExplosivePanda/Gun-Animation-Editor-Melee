using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using SFB;
using System;
using Newtonsoft.Json;


public class OnImportImagesPressed : MonoBehaviour
{

    public static Vector2 readPoint;
    public GameObject ImgTabPrefab;
    public GameObject TabArea;
    public GameObject scrollthing;
    public Toggle IsTwoHanded;
    public RawImage handIMG;
    public RawImage handIMG2;
    public Image Image;
    public RawImage GreyBackgroundThangIdk;

    public Sprite DefaultTexture;
    string[] paths;
    HandController handController1;
    HandController handController2;
    public bool ControllingHand2 = false;
    public TabDisplay SelectedTab;

    void Start()
    {
        Image.sprite = DefaultTexture;
        Image.SetNativeSize();      
              //  Image.transform.localScale = new Vector3(20,20, 1);   
    }


    //loads up every image in the paths provided, creates matching frameInfo objects, and then creates an appropriate tab for each
    public void ImportSingleImages()
    {
        Image.sprite = DefaultTexture;
        paths = StandaloneFileBrowser.OpenFilePanel("gotta make them guns ey?", "", "png", true);

        if (paths.Length > 0)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                FileInfo info = new FileInfo(paths[i]);

                GaeAnimationInfo animationInfo = new GaeAnimationInfo();

                animationInfo.AnimationDirectory = info.DirectoryName;

                animationInfo.frames = new FrameInfo[1];
                
                animationInfo.frames[0] = LoadSingleFrame(info.FullName,animationInfo);

                animationInfo.animationName = Path.GetFileNameWithoutExtension(info.FullName);

                RectTransform content = StaticRefrences.Instance.TabArea.GetComponent<ScrollRect>().content;
                RectTransform tabArea = StaticRefrences.Instance.TabArea.GetComponent<RectTransform>();

                var newButton = Instantiate(ImgTabPrefab, content, false);
                newButton.name = animationInfo.animationName + " button";
                RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
                buttonRectTransform.localPosition = new Vector3(0, 0, 0);

                float ratio = tabArea.rect.width / buttonRectTransform.rect.width;
                newButton.transform.localScale = new Vector3(ratio, ratio, 1f);

                TabDisplay newButtonTabDisplay = newButton.GetComponent<TabDisplay>();
                newButtonTabDisplay.TMPtext.text = animationInfo.animationName;
                newButtonTabDisplay.animationInfo = animationInfo;

                StaticRefrences.Instance.spriteController.SetAnimation(animationInfo);
            }
            
         
            
        }
    }
    
    //similiar to importSingleImages, however it only has one path, and attempts to find other matching files based on that. loads all the frame info objects into a single tab
   public void ImportAnimation()
   {
        Image.sprite = DefaultTexture;
        paths = StandaloneFileBrowser.OpenFilePanel("gotta make them guns ey?", "", "png", false);

        if (paths.Length>0)
        {
            FileInfo info = new FileInfo(paths[0]);
            string fileName = Path.GetFileNameWithoutExtension(paths[0]);

            string animationName = RemoveTrailingDigits(fileName);
            FileInfo[] files = info.Directory.GetFiles(animationName + "_*.png");
           
            GaeAnimationInfo animationInfo = new GaeAnimationInfo();
            animationInfo.AnimationDirectory = info.DirectoryName;

            animationInfo.frames = new FrameInfo[files.Length];
            for (int i = 0; i < animationInfo.frames.Length; i++)
            {
                animationInfo.frames[i] = LoadSingleFrame(files[i].FullName,animationInfo);
            }
            animationInfo.animationName = RemoveTrailingDigits(Path.GetFileNameWithoutExtension(files[0].FullName));

  
            RectTransform viewPort = StaticRefrences.Instance.TabArea.GetComponent<ScrollRect>().content;
            RectTransform tabArea = StaticRefrences.Instance.TabArea.GetComponent<RectTransform>();

            var newButton = Instantiate(ImgTabPrefab, viewPort,false);
            newButton.name = animationInfo.animationName+" button";
            RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector3(0,0,0);

            float ratio = tabArea.rect.width / buttonRectTransform.rect.width;

            newButton.transform.localScale = new Vector3(ratio, ratio, 1f);
           
            TabDisplay newButtonTabDisplay = newButton.GetComponent<TabDisplay>();
            newButtonTabDisplay.TMPtext.text = animationInfo.animationName;
            newButtonTabDisplay.animationInfo = animationInfo;

            StaticRefrences.Instance.spriteController.SetAnimation(animationInfo);
        }


    }
    GameObject newbutton = null;
    private string RemoveTrailingDigits(string path)
    {

        int firstNumberIndex = path.Length-1;
        while (firstNumberIndex >= 0 && char.IsDigit(path[firstNumberIndex]))
        {
            firstNumberIndex--;
        }
        return path.Substring(0, firstNumberIndex);
    }
    //this method attempts to load the image as well as a corrosponding json file, and create a matching FrameInfo object based off of those
    private FrameInfo LoadSingleFrame(string path,GaeAnimationInfo animationInfo)
    {
        path = System.IO.Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
       
        string pngPath = path + ".png";
        string jsonPath = path + ".json";
        if (!File.Exists(pngPath))
        {
            return null;
        }
        byte[] bytes = File.ReadAllBytes(pngPath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        tex.filterMode = FilterMode.Point;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1);
        if (File.Exists(jsonPath))
        {
            try
            {
                string jsonInfo = File.ReadAllText(jsonPath);
                FrameJsonInfo frameJsonInfo = JsonConvert.DeserializeObject<FrameJsonInfo>(jsonInfo);
              
                for (int i = 1; i < frameJsonInfo.attachPoints.Length; i++)
                {
                    //this bit of code relies on conversion exceptions to make sure the data type is right. this only happens to class members are marked as [JsonRequiredAttribute]
                    //this is probably really bad practice. especially since it relies on loaded objects to be of " Newtonsoft.Json.Linq.JObject" type but i dont know any better as of writing this
                    try
                    {
                        frameJsonInfo.attachPoints[i] = (frameJsonInfo.attachPoints[i] as Newtonsoft.Json.Linq.JObject).ToObject<ArrayTypeUnkownAndSize>();
                    }
                    catch (Exception){}
                    try
                    {
                        frameJsonInfo.attachPoints[i] = (frameJsonInfo.attachPoints[i] as Newtonsoft.Json.Linq.JObject).ToObject<AttachPoint>();
                    }
                    catch (Exception){}
                }
                float convertedX1 = 0;
                float convertedX2 = 0;
                float convertedY1 = 0;
                float convertedY2 = 0;
                bool isTwoHanded = false;
                for (int i = 0; i < frameJsonInfo.attachPoints.Length; i++)
                {
                    if (frameJsonInfo.attachPoints[i] is AttachPoint)
                    {
                        if ((frameJsonInfo.attachPoints[i] as AttachPoint).name == "PrimaryHand")
                        {
                            AttachPoint hand1 = frameJsonInfo.attachPoints[i] as AttachPoint;
                            convertedX1 = hand1.position.x * 16 - frameJsonInfo.x;
                            convertedY1 = hand1.position.y * 16 - frameJsonInfo.y;
                        }
                        else if((frameJsonInfo.attachPoints[i] as AttachPoint).name == "SecondaryHand")
                        {
                            AttachPoint hand2 = frameJsonInfo.attachPoints[i] as AttachPoint;
                            convertedX2 = hand2.position.x * 16 - frameJsonInfo.x;
                            convertedY2 = hand2.position.y * 16 - frameJsonInfo.y;
                            isTwoHanded = true;
                        }
                    }
                }
                return new FrameInfo(tex, sprite, convertedX1, convertedY1, convertedX2, convertedY2, frameJsonInfo.x, frameJsonInfo.y, isTwoHanded, path,animationInfo);
            }
            catch (Exception)
            {
                throw new Exception("json seems to be invalid! or i dont know how to read jsons (prob the second one ;) )!");
            }
        }
        return new FrameInfo(tex, sprite , pngPath,animationInfo);
    }
   



    private void OnDrawGizmos()
    {
        Vector3[] corners = new Vector3[4];
        Image.GetComponent<RectTransform>().GetWorldCorners(corners);

        var center = (corners[0] + corners[2]) / 2;
        var size = corners[2] - corners[0];
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
    void Update()
    {
        Vector3[] corners = new Vector3[4];
        Image.GetComponent<RectTransform>().GetWorldCorners(corners);
        readPoint = corners[0];  
    }
}

