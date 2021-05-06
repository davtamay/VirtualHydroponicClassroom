using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using WebXR;

public class StallAndFollowUI : MonoBehaviour
{

    [SerializeField] private Transform objectToRotate;
    [SerializeField] private int angleDistanceUntilRotateBack = 80;
    [SerializeField] private bool isUseCameraWorldUp = true;

    [Header("Camera To Orient With")]
    public Transform vrCamTransform;
    public Transform baseCamTransform;

    [Header("States")]
    [SerializeField] Bool_StateCheck_SO isInVR;

    private Vector3 offset;

    Quaternion rotation;

    bool isInitialLook = false;


    Vector3 oldViewingAngle;
    Vector3 curViewingAngle;

    bool isLerping = false;

    //used to hold movement when holding
    public bool hooldLerping;

    //used to hold movement by toggle on uipad
    public void SetFollowBehavior(bool follow)
    {
        if (follow)
            isUpdating = true;
        else
            isUpdating = false;
    }

    public Transform renderableObject;

    private void onVRChange(WebXRState state)
    {
        
        if (state == WebXRState.ENABLED)
        {
            curViewingAngle = vrCamTransform.forward;
            oldViewingAngle = vrCamTransform.forward;

            foreach (Canvas can in _uICanvasas)
                can.worldCamera = StateSwitchDependingOnVR.Instance._vrCamera;
        }
        else
        {
            curViewingAngle = baseCamTransform.forward;
            oldViewingAngle = baseCamTransform.forward;

            foreach (Canvas can in _uICanvasas)
                can.worldCamera = StateSwitchDependingOnVR.Instance._outOfVrCamera;
        }

        isLerping = true;

    }
    public Canvas[] _uICanvasas;
    private IEnumerator Start()
    {
        WebXRManager.Instance.OnXRChange += onVRChange;

        if (!renderableObject)
            renderableObject = transform.GetChild(0);

        renderableObject.gameObject.SetActive(false);

        yield return new WaitUntil(() => ClientSpawnManager.Instance.isURL_Loading_Finished);

        _uICanvasas = GetComponentsInChildren<Canvas>(true);
        foreach (Canvas can in _uICanvasas)
        {
            can.worldCamera = StateSwitchDependingOnVR.Instance._outOfVrCamera;
        }



        if (objectToRotate == null)
        {
            objectToRotate = transform;
        }
        if (vrCamTransform == null)
        {
            WebXRCamera webxrCameras = GameObject.FindWithTag("Player").GetComponent<WebXRCamera>();
            vrCamTransform = webxrCameras.cameraL.transform;
            baseCamTransform = webxrCameras.cameraMain.transform;
            
            objectToRotate.position = baseCamTransform.TransformPoint(Vector3.forward * 0.5f);

            if (isUseCameraWorldUp)
                objectToRotate.LookAt(2 * objectToRotate.position - baseCamTransform.position, baseCamTransform.up);
            else
                objectToRotate.LookAt(2 * objectToRotate.position - baseCamTransform.position, Vector3.up);
        }

    StartCoroutine(Initiate());

     //   onVRChange(WebXRState.ENABLED);
      

    }

    public bool isUpdating;
    IEnumerator Initiate()
    {
        yield return null;
        renderableObject.gameObject.SetActive(true);
      
        //WE SETUP EVERYTHING SO WE ARE READY TO GET THE CURRENT LOOK DIRECTION
        isInitialLook = true;
        isUpdating = true;
    }

    Vector3 locationToRotateTo;
    void LateUpdate()
    {
        if (hooldLerping || !isUpdating)
            return;

        //DETERMINE WHAT IS THE ORIGINAL LOOK DIRECTION IN CURRENT VIEWING MEDIUM
        if (isInitialLook)
        {
            isInitialLook = false;

            if (isInVR.value)
            {
                curViewingAngle = vrCamTransform.forward;
                oldViewingAngle = vrCamTransform.forward;
            }
            else
            {
                curViewingAngle = baseCamTransform.forward;
                oldViewingAngle = baseCamTransform.forward;
            }
     
            objectToRotate.position = baseCamTransform.TransformPoint(Vector3.forward * 0.5f);

        }
        else
        {
            if (isInVR.value)
                curViewingAngle =  vrCamTransform.forward; 
            else
                curViewingAngle = baseCamTransform.forward;

            //DETECTS IF ANGLE OF LOOK CROSSES THE THRESHOLD, TO MOVE UI PAD IN FRONT, OR TO KEEP PAD MOVING IN REACHING THE USERS FRONT LOOK DIRECTION
            if (Vector3.Angle(oldViewingAngle, curViewingAngle) > angleDistanceUntilRotateBack || isLerping)
            {
                locationToRotateTo = default;
                Vector3 cameraOriginPosition = default;

                float DistanceSqrd = default;

                //SWITCH VIEW CAMERA FOCUS //IF IT DOES NOT CONSIDER MAIN CAMERA PUT IT ON A LIST AN SEARCH CHILDREN
                if (isInVR.value)
                {
                    locationToRotateTo = vrCamTransform.TransformPoint(Vector3.forward * 0.5f);
                    cameraOriginPosition = vrCamTransform.position;
                    DistanceSqrd = Vector3.Distance(objectToRotate.position, vrCamTransform.TransformPoint(Vector3.forward * 0.5f));//camTransform.position - (camTransform.rotation * (offset * 1)));

                    if (isUseCameraWorldUp)
                        objectToRotate.LookAt(2 * objectToRotate.position - cameraOriginPosition, vrCamTransform.up);
                    else
                        objectToRotate.LookAt(2 * objectToRotate.position - cameraOriginPosition, Vector3.up);

                }
                else
                {
                    locationToRotateTo = baseCamTransform.TransformPoint(Vector3.forward * 0.5f);
                    cameraOriginPosition = baseCamTransform.position;
                    DistanceSqrd = Vector3.Distance(objectToRotate.position, baseCamTransform.TransformPoint(Vector3.forward * 0.5f));// camTransform.parent.position - (camTransform.parent.rotation * (offset * 1)));


                    if (isUseCameraWorldUp)
                        objectToRotate.LookAt(2 * objectToRotate.position - cameraOriginPosition, baseCamTransform.up);
                    else
                        objectToRotate.LookAt(2 * objectToRotate.position - cameraOriginPosition, Vector3.up);
              
                }

                //SET THE APPROPIATE MOVEMENT FOR USER
                objectToRotate.position = Vector3.Lerp(objectToRotate.position, locationToRotateTo, Time.unscaledDeltaTime * 3f);  //camTransform.position - (rotation * (offset * 1)), Time.unscaledDeltaTime * 3f);

                if (DistanceSqrd < 0.1f)
                {
                    isLerping = false;
                    oldViewingAngle = curViewingAngle;
                }
       
            }

        }


    }
}
