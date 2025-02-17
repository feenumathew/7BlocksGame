using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    float refAspectRatio = .56f;//for 1920*1080 size

    // Start is called before the first frame update
    void Update()
    {
        cam = GetComponent<Camera>();
        float camSize = refAspectRatio/cam.aspect*1f*8f;
        if(camSize>8)
            cam.orthographicSize = camSize;
    
    }

}
