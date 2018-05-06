using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GBuster;

public class MainCameraController : MonoBehaviour {

    public Camera mainCamera;

	// Use this for initialization
	void Start ()
    {
        
        mainCamera = Camera.main;

        float angle = (mainCamera.fieldOfView / 2) * (Mathf.PI / 180);
        //mainCamera.fieldOfView=60

        float posX = Define.mapSizeWidth * Define.mapTileSize / 2;
        float posY = Mathf.Sqrt(2) * posX / Mathf.Sin(angle);
        float posZ = Define.mapSizeHeight * Define.mapTileSize / 2;

        transform.position = new Vector3(posX, posY, posZ);
        
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
