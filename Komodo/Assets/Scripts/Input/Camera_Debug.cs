using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Debug : MonoBehaviour
{
    Camera[] camerasInScene;
    // Start is called before the first frame update
    void Start()
    {
        camerasInScene = GameObject.FindObjectsOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in camerasInScene)
        {
            Debug.LogError("Cam Name:  " + item.name + " Enabled: " + item.enabled + "; FOV : " + item.fieldOfView);
        }
        
        
    }
}
