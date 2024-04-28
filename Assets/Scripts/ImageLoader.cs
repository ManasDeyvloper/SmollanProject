


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
//using UnityEditor.PackageManager.Requests;

public class ImageLoader : MonoBehaviour
{
    RawImage rawImage;
    FirebaseStorage storage;
    StorageReference storageReference;
    [SerializeField] Text text;
    [SerializeField] int ImageIndex = 0;
    [SerializeField] GameObject Content;
    [SerializeField] RawImage[] RawImages;
    [SerializeField] GameObject RawImagePrefab;
    [SerializeField] RectTransform Canvas;
    [SerializeField]private float targetWidth = 864;
   [SerializeField] bool imageFound = true;

    //WebRequest to download and apply texture on RawImage
    IEnumerator LoadImage(string MediaUrl, RawImage rawImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {

            Debug.Log(MediaUrl);
            Debug.Log(request.error);
           
        }
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            ResizeImage(rawImage);

        }
    }

    void Start()
    {
        RawImages = Content.GetComponentsInChildren<RawImage>();
        //Initial batch
        StartCoroutine(LoadImages(0));
    }

    private void Update()
    {
        RawImages = Content.GetComponentsInChildren<RawImage>();
        targetWidth = Canvas.rect.width;

    }
    //Checks if RawImage has been created, if not then Instantiates a new one 
    public void MakeImageSpace(int index)
    {

        try
        {
            RawImage rawImage = RawImages[index];
        }
        catch (Exception e) 
        {
            StartCoroutine(InstantiateAndWait(rawImage,index));
            Debug.Log("loading"+ index);
            Debug.LogError(e);
            
        }
        }


    //Loads Batches of 3
    IEnumerator LoadImages(int startIndex)
    {
       //  startIndex = 0 ;
        int endIndex = startIndex +2;


        for (int i = startIndex; i < endIndex; i++)
        {
            MakeImageSpace(i);
            yield return new WaitForEndOfFrame();
        }
        ImageIndex = endIndex;
        yield return null;
    }

    
    //Resizing RawImage component to fit the texture without distortion
    void ResizeImage(RawImage rawImage) 
    {
        Texture texture = rawImage.texture;
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        float aspectRatio = (float)texture.width / texture.height;
        float targetHeight = targetWidth / aspectRatio;

        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }

    //Instantiation of RawImage and Apply Texture
    IEnumerator InstantiateAndWait(RawImage rawImage, int index)
    {

        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://smollanproject.appspot.com");

        StorageReference image = storageReference.Child("Images/" + (index + 1) + ".jpg");
        if (image != null && imageFound)
        {
            GameObject instantiatedObject = Instantiate(RawImagePrefab, Content.transform);
            yield return new WaitForEndOfFrame();

            rawImage = instantiatedObject.GetComponentInChildren<RawImage>();
            RawImages[index] = rawImage;

            image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    StartCoroutine(LoadImage(Convert.ToString(task.Result), rawImage));
                }
                else
                {
                    
                    imageFound = false;
                    text.text = "Try again later";
                    Destroy(instantiatedObject);
                    Debug.Log(task.Exception);
                }
            });
        }
    }


    //Function for button 

    public void LoadMoreImages() 
    {
        StartCoroutine(LoadImages(ImageIndex));
    }
}

























