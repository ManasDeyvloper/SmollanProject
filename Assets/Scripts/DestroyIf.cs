
using UnityEngine;
using UnityEngine.UI;

public class DestroyIf : MonoBehaviour
{
    RawImage rawImage = null;
    
    // Update is called once per frame
    void Update()
    {
        
        rawImage= GetComponent<RawImage>(); 
        if(rawImage == null) 
        {
           Destroy(gameObject);
        }

    }

    public void DestroyThis()
    { Destroy(gameObject); }
}
