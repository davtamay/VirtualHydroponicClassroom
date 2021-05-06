﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;

public class Color_Picker : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,  IPointerExitHandler//,IDragHandler,IEndDragHandler,IBeginDragHandler
{
    public LineRenderer[] targets;
    public List<Trigger_Draw> drawTargets;
   // public LineRenderer target;
    //public Material materialTarget;
    public Transform colorTargetLocation;
    public Image colorDisplay;
   // public Camera deskTopCamera;
    /*  
    /!\ Don't forget to make the texture readable
    (Select your texture : in Inspector
    [Texture Import Setting] > Texture Type > Advanced > Read/Write enabled > True  then Apply).
    */
    public Texture2D colorPicker;

    public Rect colorPanelRect = new Rect(0, 0, 200, 200);

    //public GameObject drawingTool;
    //public Vector3 drawOriginalLocation;
    private void Awake()
    {
        
        foreach (var item in targets)
        {
            var triggerDraw = item.GetComponent<Trigger_Draw>();

            if (triggerDraw == null)
            {
                Debug.LogError("There is no TriggerDraw.cs in Color Tool LineRenderer ", this.gameObject);
                continue ;
            }
            drawTargets.Add(triggerDraw);
        }
       // drawOriginalLocation = drawingTool.transform.localPosition;
        colorPicker = (Texture2D) GetComponent<RawImage>().texture;

        if (!colorPicker)
            Debug.LogError("Color_Picker.cs is missing RawTexture", gameObject);
     //   target = target.GetComponent<LineRenderer>();

        colorPanelRect.width = colorPicker.width;
        colorPanelRect.height = colorPicker.height;
        
    }

  
    void Update()
    {
        Vector2 pickpos = new Vector2(colorTargetLocation.localPosition.x, colorTargetLocation.localPosition.z);// Event.current.mousePosition;//new Vector2(colorTargetLocation.position.x, colorTargetLocation.position.y);

        float aaa = 0.1f * Mathf.Abs((pickpos.x - colorPanelRect.x) / 10f * 10f - 10f) * 512f;

        float bbb = 0.1f * Mathf.Abs((pickpos.y - colorPanelRect.y) / 10f * 10f) * 512f;

        int aaa2 = (int)(aaa * (colorPicker.width / (colorPanelRect.width + 0.0f)));

        int bbb2 = (int)((colorPanelRect.height - bbb) * (colorPicker.height / (colorPanelRect.height + 0.0f)));

        Color col = colorPicker.GetPixel(aaa2, bbb2);

        foreach (var item in targets)
        {
            item.startColor = col;
            item.endColor = col;

        }
        //target.sharedMaterial.color = col;
        //foreach (var item in targets)
        //{
        //    item.material.color = col;
        //}
        
        colorDisplay.color = col;
    }

    //change color marker depending on image selection location
    public void OnPointerClick(PointerEventData eventData) => colorTargetLocation.position = Input.mousePosition;


    //Do not allow for drawing to continue to avoid having issues of line stroke jumping from color picker to outside draw
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var item in drawTargets)
        {
            item.isSelectingColorPicker = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var item in drawTargets)
        {
            item.isSelectingColorPicker = false;
        }
    }
    //on pointerexit does not get called when turning off UI so also do behavior when its disabled aswell
    public void OnDisable()
    {
        foreach (var item in drawTargets)
        {
            item.isSelectingColorPicker = false;
        }
    }
}
