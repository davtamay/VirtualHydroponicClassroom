using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading.Tasks;

[System.Serializable]
public class Send_Camera_UnityEvent : UnityEvent<Camera> { }

public class Trigger_Select : MonoBehaviour
{
    public Send_Camera_UnityEvent set_Raycast_Camera_Selection;
    public UnityEvent onDisable_UnityEvent;

    private Camera eventCamera;
    private Transform thisTransform;
    LineRenderer lr;

    public Trigger_Select other_handTriggerSelect;
    // public Trigger_Select r_handTriggerSelect;
    
    //to check if we should emit lazers or not
    public GameObject ui_cursor;

    public bool is_this_LR_ProducingEvents;
    public void Set_Current_LR_Event(bool isOn)
    {
        is_this_LR_ProducingEvents = isOn;
    }

    public void Awake()
    {
        thisTransform = transform;
        lr = GetComponent<LineRenderer>();
        eventCamera = GetComponent<Camera>();
    }
    public void OnEnable()
    {
        //set to use this particular line renderer for events
        if (standalone != null)
        {
            standalone.isUpdating = true;
            standalone.isCurrentLineRendererOn = true ;
            set_Raycast_Camera_Selection.Invoke(eventCamera);
        }
        
    }

    public void Update()
    {
     
        //only use this if lr is not producing events to allow to avoid visual errors when using both lasers for possible selection 
        if (!is_this_LR_ProducingEvents)
        {
            //if (ui_cursor.activeInHierarchy)
            //{
                lr.SetPosition(0, thisTransform.position);
                lr.SetPosition(1, thisTransform.position + thisTransform.forward * 15f);
            //}
            //else
            //{
            //    lr.SetPosition(0, Vector3.zero);
            //    lr.SetPosition(1, Vector3.zero);
            //}
        }
    }

   




    public Standalone_InputModule_Modifie_IndCameraRays standalone;
    // public EventSystem es;
   // THIS IS WHERE FUNCTIONS ARE INVOKED(ON RELEASE OF TRIGGER BUTTON WHICH DEACTIVATES PARENT OBJECT
    public void OnDisable()
    {
    

        if (standalone != null)
        {
            standalone.SetTriggerForClick();

          //only change input when other lazer is on, if not keep it within this hand to track cursor
          if(other_handTriggerSelect.gameObject.activeInHierarchy)
            onDisable_UnityEvent.Invoke();
        }

    }

    
}
