using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomBaseInput : BaseInput
{

    public Camera controllerCameraRay;
    Standalone_InputModule_Modifie_IndCameraRays standaloneInputModule;

    public void Set_EventCamera(Camera eventCamera)
    {
        controllerCameraRay = eventCamera;
    }

    protected override void Awake()
    {
        standaloneInputModule = GetComponent<Standalone_InputModule_Modifie_IndCameraRays>();
        if (standaloneInputModule) standaloneInputModule.inputOverride = this;
    }

    public override bool GetMouseButtonDown(int button)
    {
        return true;
    }

    public override Vector2 mouseScrollDelta => Vector2.one;
    

    public override bool mousePresent => true;
   
    public override Vector2 mousePosition => new Vector2(controllerCameraRay.pixelWidth / 2, controllerCameraRay.scaledPixelHeight / 2);

    //protected override void OnDisable()
    //{
    //    standaloneInputModule.SetTriggerForClick();
    //    //standaloneInputModule.OnDisable();
    //}
}
