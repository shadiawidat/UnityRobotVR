using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRPSController : MonoBehaviour // mixed reality play space controller 
{

    public Vector3 newPosition = new Vector3(0, 15, 0); //  new position for the Play Space
    public Vector3 newRotation = new Vector3(90, 0, 0); //  new rotation for the Play Space

    private void Start()
    {
        // Find the MixedRealityPlayspace in the scene
        GameObject playSpace = GameObject.Find("MixedRealityPlayspace");

        if (playSpace != null) // we apply some transformation and rotation in order to start the camera on the top of the plane in order to move the player easily and controll it better 
        {
            Transform playSpaceTransform = playSpace.transform;
            playSpaceTransform.position = newPosition;
            playSpaceTransform.rotation = Quaternion.Euler(newRotation);
        }
        else
        {
            Debug.LogError("MixedRealityPlayspace not found.");
        }
    }

}
