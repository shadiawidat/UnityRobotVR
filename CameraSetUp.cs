using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraSetUp : MonoBehaviour
{
    public Camera mainCamera; 

    void Start()
    {
        if (mainCamera == null)  // we made sure that the camera starts with no rotations and transformations
        {
            mainCamera = Camera.main;
        }

        mainCamera.transform.position = new Vector3(0, 0, 0);

        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
 
}
