using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, ICursorHover
{
    //public Standalone_InputModule_Modifie_IndCameraRays inputModule;
    public GameObject cursor;

    private Image cursorImage;
    public Color hoverColor;
    private Color originalColor;

//    public LineRenderer[] selectionRays;
[Header("Add both Draw objects to avoid using draw features when interacting with UI")]
    public Trigger_Draw[] objectsToDeactivateOnHover;



    //public UnityEvent onMenu_Focus;
    //public UnityEvent onMenu_UnFocus;
    public void Awake()
    {
        cursorImage = GetComponent<Image>();
    }
    void Start () {


        //if (!inputModule) {
        //    throw new Exception("You must set an input module");
        //}

        if (!cursor) {
            throw new Exception("You must set a cursor");
        }
   

        if (!cursorImage) {
            throw new Exception("You must have an Image component on your cursor");
        }
        cursor.SetActive(true);

    }
    

    public void OnPointerEnter(PointerEventData eventData) {

        //foreach (var item in selectionRays)
        //{
        //    item.enabled = true;
        //}
        foreach (var item in objectsToDeactivateOnHover)
        {
            item.enabled = false;
        }
        //if (!StateSwitchDependingOnVR.Instance.isInVR)
        //    return;

     //   onMenu_Focus.Invoke();
        originalColor = cursorImage.color;
        cursorImage.color = hoverColor;
    
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        foreach (var item in objectsToDeactivateOnHover)
        {
            item.enabled = true;
        }

        cursorImage.color = originalColor;
    //    cursor.SetActive(false);
    }

    //on pointerexit does not get called when turning off UI so also do behavior when its disabled aswell
    public void OnDisable()
    {
        foreach (var item in objectsToDeactivateOnHover)
        {
            item.enabled = true;
        }

        if (!cursorImage)
            cursorImage = cursor.GetComponent<Image>();

        cursorImage.color = originalColor;
       // cursor.SetActive(false);
    }


}
