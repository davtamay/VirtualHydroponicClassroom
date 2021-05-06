using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using WebXR;

public class StateSwitchDependingOnVR : SingletonComponent<StateSwitchDependingOnVR>
{

    public static StateSwitchDependingOnVR Instance
    {
        get { return ((StateSwitchDependingOnVR)_Instance); }
        set { _Instance = value; }
    }
   

    [Space]
    [Header("VR and Non VR Cameras")]

    public Camera _vrCamera;
    public UnityEvent _inVR_Event;

    public Camera _outOfVrCamera;
    public UnityEvent _outOfVR_Event;

    public bool isInVR;
    void Start()
    {
        //_vrCamera = Camera.main;
        //_outOfVrCamera = GameObject.FindGameObjectWithTag("Player").GetComponent<Camera>();

        _outOfVR_Event.Invoke();
        WebXRManager.Instance.OnXRChange += onXRChange;
    }
    private void onXRChange(WebXRState state)
    {

        if (state == WebXRState.ENABLED)
        {
            isInVR = true;
            _inVR_Event.Invoke();
        }
        else
        {
            isInVR = false;
            _outOfVR_Event.Invoke();
        }

    }




#if UNITY_EDITOR || !UNITY_WEBGL
    void Update()
    {
        //only toggle when the device active state doesn't match the internal state
        if (XRSettings.isDeviceActive && !isInVR)
        {
            Debug.Log("Entered Headset.");
            isInVR = true;
            //WebXRManager.Instance.setXrState(WebXRState.ENABLED);
            _inVR_Event.Invoke();
            return;
        }

        if (!XRSettings.isDeviceActive && isInVR)
        {
            Debug.Log("Exited Headset.");
            isInVR = false;
            //WebXRManager.Instance.setXrState(WebXRState.NORMAL);
            _outOfVR_Event.Invoke();
        }
    }

    // public void SwitchCameras(WebXRState state) {
    //     if (state == WebXRState.ENABLED) {

    //     }

    //     if (state == WebXRState.NORMAL) {

    //     }
    // }
#endif
}

