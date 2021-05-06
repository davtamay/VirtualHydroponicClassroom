using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_UI_Placement : MonoBehaviour
{

    public Vector3 scaleToChangeTo; // = new Vector3(0.001f, 0.001f, 0.001f); // UI > Rect Transform > Scale
    public Vector3 rotationToChangeTo;
    public Vector3 positionToChangeTo;


    [Header("HAND SELECTION CAMERA")]
    public Camera left_Selection_lazer_camera;
    public List<Canvas> selectionCanvas = new List<Canvas>();

    //public IEnumerator Start()
    //{

    //    yield return new WaitForSeconds(2f);
    //    GetComponent<StateSwitchDependingOnVR>()._inVR_Event.Invoke();

    //    foreach (var item in selectionCanvas)
    //    {
    //        item.worldCamera = left_Selection_lazer_camera;
    //    }

    //    SetVRViewPort();
    //}

    public void SetVRViewPort()
    {   //todo(Brandon): refactor this so that the world camera can also get set for the right hand.
        var canvasTransform = selectionCanvas[0].GetComponent<RectTransform>();

        if (canvasTransform == null) {
            throw new Exception("selection canvas must have a RectTransform component");
        }
        
        canvasTransform.localRotation = Quaternion.Euler(rotationToChangeTo); //0, 180, 180 //UI > Rect Trans > Rotation -123, -0.75, 0.16
        canvasTransform.localScale = scaleToChangeTo;
        canvasTransform.anchoredPosition3D = positionToChangeTo; //new Vector3(0.0f,-0.35f,0f); //UI > R T > Position 0.25, -0.15, 0.1
        selectionCanvas[0].renderMode = RenderMode.WorldSpace;
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500); // sizeDelta.x =  500;

        foreach (var item in selectionCanvas)
        {
            item.worldCamera = left_Selection_lazer_camera;
        }
      //  selectionCanvas.worldCamera = left_Selection_lazer_camera;
       
    }

    public void SetDesktopViewport()
    {
        selectionCanvas[0].renderMode = RenderMode.ScreenSpaceOverlay;

    }
}
