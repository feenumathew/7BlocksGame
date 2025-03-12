using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private float refAspectRatio = .56f;//for 1920*1080 size

    private GridManager gridManager;

    public Transform gridBg;

    void Start()
    {
        cam = GetComponent<Camera>();
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
       
        float camSize = refAspectRatio / cam.aspect * 1f * 8f;
        if (camSize > 8)
            cam.orthographicSize = camSize;

        


       cam.orthographicSize = Mathf.Max(gridManager.gridWidth / (2 * cam.aspect),gridManager.gridHeight / 2) + 2f;

       gridBg.transform.position = new Vector2(gridManager.gridHeight/2,gridManager.gridHeight/2);

        gridBg.localScale = new Vector3(gridManager.gridWidth * 1.05f, gridManager.gridHeight * 1.05f, 1);

       transform.position =   new Vector3(gridManager.gridHeight/2,gridManager.gridHeight/2+1,-10);


    }

}
