using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using WebXR;

public class FreeFlightController : MonoBehaviour //IDragHandler, IBeginDragHandler, IEndDragHandler 
{
    [Tooltip("Enable/disable rotation control. For use in Unity editor only.")]
    public bool rotationEnabled = true;

    [Tooltip("Enable/disable translation control. For use in Unity editor only.")]
    public bool translationEnabled = true;

    private WebXRDisplayCapabilities capabilities;

    [Tooltip("Mouse sensitivity")]
    public float mouseSensitivity = 1f;

    [Tooltip("Pan Sensitivity sensitivity + middle mouse hold")]
    public float panSensitivity = 0.65f;

    [Tooltip("Straffe Speed")]
    public float straffeSpeed = 5f;

    private float minimumX = -360f;
    private float maximumX = 360f;

    private float minimumY = -90f;
    private float maximumY = 90f;

    public int turningDegrees = 30;

    Quaternion originalRotation;

    private Transform thisTransform;
    private Transform vrPlayer;

    public TeleportPlayer teleportPlayer;


    public bool pinDesktopTransformToVR;

    // private float moveSpeed = 0.5f;
    public float scrollSpeed = 10f;

    //to check on ui over objects to disable mouse drag while clicking buttons
    public StandaloneInputModule_Desktop standaloneInputModule_Desktop;

    //set state of desktop
    public bool isUpdating = true;

  

    bool inDesktopLike
    {
        get
        {
            return capabilities.supportsInline;
        }
    }

    void Start()
    {
        //vr_Cameras_parent
        if(!vrPlayer)
        vrPlayer = GameObject.FindWithTag("XRCamera").transform;

        //desktop_Camera
        thisTransform = transform;
        WebXRManager.Instance.OnXRChange += onXRChange;

        if(!standaloneInputModule_Desktop) {
            var eventSystem = GameObject.FindGameObjectWithTag("EventSystemDesktop");
            if (!eventSystem) {
                throw new System.Exception("No desktop event system found.");
            }

            standaloneInputModule_Desktop = eventSystem.GetComponent<StandaloneInputModule_Desktop>();
            if (!standaloneInputModule_Desktop) {
                throw new System.Exception("No input module found on desktop event system object.");
            }
        }

        WebXRManager.Instance.OnXRCapabilitiesUpdate += onXRCapabilitiesUpdate;// onVRCapabilitiesUpdate;
        originalRotation = thisTransform.localRotation;


    }

    private void onXRChange(WebXRState state)
    {
        if (state == WebXRState.ENABLED)
        {
            //  DisableEverything();
            isUpdating = false;
            //set allignment x for VR to avoid leaving weird rotations from desktop mode
            curRotationX = 0f;
            var result = Quaternion.Euler(new Vector3(0, curRotationY, 0));

            teleportPlayer.UpdatePlayerRotation(new Position { rot = result });

        }
        else
        {

            //commented to avoid setting rotation back on which causes rotational issues when switching cameras
            //  EnableAccordingToPlatform();

            //set desktop camera the same as the xr camera on xr exit
            curRotationX = 0f;
            thisTransform.position = vrPlayer.position;
            thisTransform.localRotation = Quaternion.Euler(new Vector3(0, curRotationY, 0));
            teleportPlayer.UpdatePlayerPositionWithoutHeight(new Position { pos = thisTransform.position, rot = thisTransform.localRotation });//vrPlayer.localRotation;
            isUpdating = true;
        }
    }

    private void onXRCapabilitiesUpdate(WebXRDisplayCapabilities vrCapabilities)
    {
        capabilities = vrCapabilities;
        EnableAccordingToPlatform();
    }


    #region  Buttton event linking funcions (editor UnityEvent accessible)
    Quaternion xQuaternion;
    Quaternion yQuaternion;
    public void RotatePlayer(int rotateDirection)
    {


        switch (rotateDirection)
        {

            case 3:
                //LEFT
                curRotationX -= turningDegrees;
                break;

            case 4:
                //RIGHT
                curRotationX += turningDegrees;
                break;

            case 2:
                //UP
                curRotationY -= turningDegrees;
                break;

            case 1:
                //DOWN
                curRotationY += turningDegrees;
                break;
        }

        curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
        curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

        thisTransform.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
    }

    public void RotateXR_Player(int rotateDirection)
    {
        switch (rotateDirection)
        {

            case 3:
                //LEFT
                curRotationX -= turningDegrees;
                break;

            case 4:
                //RIGHT
                curRotationX += turningDegrees;
                break;

            case 2:
                //UP
                curRotationY -= turningDegrees;
                break;

            case 1:
                //DOWN
                curRotationY += turningDegrees;
                break;
        }

        // thisTransform.localRotation 

        curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
        curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

        var result = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));

        teleportPlayer.UpdatePlayerRotation(new Position { rot = result });
        //   originalRotation = result;
    }
    float curRotationX = 0f;
    float curRotationY = 0f;
    public void RotatePlayerWithDelta(int rotateDirection)
    {
        var delta = Time.deltaTime * straffeSpeed;

        switch (rotateDirection)
        {

            case 3:
                //LEFT
                curRotationX -= 45F * delta;
                break;

            case 4:
                //RIGHT
                curRotationX += 45F * delta;
                break;

            case 2:
                //UP
                curRotationY -= 45 * delta;
                break;

            case 1:
                //DOWN
                curRotationY += 45 * delta;
                break;
        }
        curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
        curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

        thisTransform.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
    }


    public void MovePlayer(int moveDirection)
    {
        switch (moveDirection)
        {

            case 1:

                float x = 1;
                var movement = new Vector3(x, 0, 0);
                movement = thisTransform.TransformDirection(movement);
                thisTransform.position += movement;
                break;

            case 2:

                x = -1;
                movement = new Vector3(x, 0, 0);
                movement = thisTransform.TransformDirection(movement);
                thisTransform.position += movement;
                break;

            case 3:

                float z = 1;// Input.GetAxis("Vertical") * Time.deltaTime * straffeSpeed;
                movement = new Vector3(0, 0, z);
                movement = thisTransform.TransformDirection(movement);
                thisTransform.position += movement;
                break;

            case 4:

                z = -1;// Input.GetAxis("Vertical") * Time.deltaTime * straffeSpeed;
                movement = new Vector3(0, 0, z);
                movement = thisTransform.TransformDirection(movement);
                thisTransform.position += movement;
                break;
        }
    }

    #endregion


    //#if UNITY_EDITOR
    void Update()
    {
        if (!isUpdating)
            return;

       
        //float rotationX = 0f;
        //float rotationY = 0f;

        if (translationEnabled)
        {
            var accumulatedImpactMul = Time.deltaTime * straffeSpeed;
            float x = Input.GetAxis("Horizontal") * accumulatedImpactMul;
            float z = Input.GetAxis("Vertical") * accumulatedImpactMul;

            if (Input.GetKey(KeyCode.Q)) RotatePlayerWithDelta(2);
            if (Input.GetKey(KeyCode.E)) RotatePlayerWithDelta(1);
            if (Input.GetKey(KeyCode.Alpha2)) RotatePlayerWithDelta(3);
            if (Input.GetKey(KeyCode.Alpha3)) RotatePlayerWithDelta(4);


            var movement = new Vector3(x, 0, z);
            movement = thisTransform.TransformDirection(movement);
            thisTransform.position += movement;
        }

        //if event system picks up button selection -> skip mouse drag events
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (standaloneInputModule_Desktop.GetCurrentFocusedObject_Desktop())
            {
                if (standaloneInputModule_Desktop.GetCurrentFocusedObject_Desktop().layer == LayerMask.NameToLayer("UI"))
                    return;
            }
        }

        if (rotationEnabled && Input.GetMouseButton(0))
        {
            curRotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
            curRotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            //rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            //rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;

            curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
            curRotationY = ClampAngle(curRotationY, minimumX, maximumX);
            //rotationY = ClampAngle(rotationY, minimumY, maximumY);
            //rotationX = ClampAngle(rotationX, minimumX, maximumX);
            //rotationY = ClampAngle(rotationY, minimumY, maximumY);

            thisTransform.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
        }

        //pan
        if (Input.GetMouseButton(2))
        {
            var x = Input.GetAxis("Mouse X") * panSensitivity;
            var y = Input.GetAxis("Mouse Y") * panSensitivity;

            thisTransform.position += thisTransform.TransformDirection(new Vector3(x, y));
        }

        //moves desktop player forward depending on scrollwheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            thisTransform.position += thisTransform.TransformDirection(scrollSpeed * new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel")));//thisTransform.TransformPoint(scrollSpeed * new Vector3(0, 0, -Input.GetAxis("Mouse ScrollWheel")));


        //synchronize xr camera with desktop camera transforms
        teleportPlayer.UpdatePlayerPositionWithoutHeight(new Position { pos = thisTransform.position, rot = thisTransform.localRotation });
    }



    //#endif

    private enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
    //    Debug.Log(draggedDir);
        return draggedDir;
    }


    //#endif
    void DisableEverything()
    {
        translationEnabled = false;
        rotationEnabled = false;
    }

    /// Enables rotation and translation control for desktop environments.
    /// For mobile environments, it enables rotation or translation according to
    /// the device capabilities.
    void EnableAccordingToPlatform()
    {
        rotationEnabled = translationEnabled = !capabilities.supportsImmersiveVR;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }


}
