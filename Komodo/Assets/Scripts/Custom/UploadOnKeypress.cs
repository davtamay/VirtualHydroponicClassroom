using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadOnKeypress : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.A) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) )
        {
            print("ok!");
        }
    }
}
