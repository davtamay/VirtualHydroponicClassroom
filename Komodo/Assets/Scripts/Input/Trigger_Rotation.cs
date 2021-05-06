using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Trigger_Rotation : Trigger_Base
{

    public float attenuation;
    public Vector3 initialPos;
    public Transform _posOfHandLaser;

    public Transform thisTransform;
    public string _interactableTag = "UIInteractable";

    InputSelection inputSelection;

    public float _magnitudeOfRotation = 3;




    //Subset Menu Setup
    public GameObject gizmo_toShow;

    //CAMERA TO CHANGE REFERENCES
    public Canvas canvas;
    public LookAtConstraint lookAtConstraint;

    public void ChangeCamerasReference(Camera cam)
    {
        if (canvas == null)
            Start();

        canvas.worldCamera = cam;
        lookAtConstraint.RemoveSource(0);
        lookAtConstraint.AddSource(new ConstraintSource() { sourceTransform = cam.transform, weight = 1 });
        lookAtConstraint.worldUpObject = cam.transform;
    }
    private bool vrState;
    public void OnEnable()
    {
        //if (StateSwitchDependingOnVR.IsAlive)
        var curVRstate = StateSwitchDependingOnVR.Instance.isInVR;
        if (vrState == curVRstate)
            return;

        vrState = curVRstate;

        if (StateSwitchDependingOnVR.Instance.isInVR)
            ChangeCamerasReference(StateSwitchDependingOnVR.Instance._vrCamera);
        else
            ChangeCamerasReference(StateSwitchDependingOnVR.Instance._outOfVrCamera);
    }


    public void Awake()
    {
        inputSelection = transform.parent.GetComponent<InputSelection>();
        _posOfHandLaser = inputSelection.transform;
      //  _interactableTag = inputSelection._tagToLookFor;
        thisTransform = transform;
        gizmo_toShow = thisTransform.GetChild(0).gameObject;
    }
    public override void Start()
    {
        if (canvas == null)
        {
            canvas = gizmo_toShow.GetComponent<Canvas>();
            lookAtConstraint = gizmo_toShow.GetComponent<LookAtConstraint>();
        }
    }


    public float initialOffset;
   // public Vector3 initialScale;
    public GameObject currentGO;
    public override void OnTriggerEnter(Collider other)
    {
        //if ui skip
        if (other.gameObject.layer == 5)
            return;

        //THIS IS TO AVOID LOOSING CONNECTION (INITIALPOS) TO INITIAL OBJECT - DISABLED REMOVE THE CONNECTION
        if (other.gameObject != currentGO)
        {
            currentGO = other.gameObject;
        }
        else
        {
            if (gizmo_toShow != null)
                gizmo_toShow.SetActive(true);
            return;
        }

        if (other.CompareTag(_interactableTag))
        {
            initialPos = thisTransform.position;
            initialOffset = Vector3.Distance(_posOfHandLaser.position, initialPos);


            if (gizmo_toShow != null)
                gizmo_toShow.SetActive(true);


            //NETWORK REGISTER
            try
            {
                var currentInteractiveObject = other.GetComponent<Net_Register_GameObject>();

                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = ClientSpawnManager.Instance._mainClient_entityData.entityID,//GetComponent<Entity_Container>().entity_data.entityID,
                    targetEntity_id = currentInteractiveObject.positionWithin_urlList,
                    interactionType = (int)INTERACTIONS.GRAB,
                });

                MainClientUpdater.Instance.PlaceInNetworkUpdateList(currentInteractiveObject);
                // currentInteractiveObject.entity_data.isCurrentlyGrabbed = true;
            }
            catch
            {
                Debug.LogWarning("Custom Warning: " + "Could not send Interaction : ");
            }
        }
    }
  
    public void SetDirectionToRotate(int i)
    {
        if (!inputSelection.isOverObject)
            return;

            switch (i)
        {
            case 1:
                currentGO.transform.rotation *= Quaternion.AngleAxis(15.0f, Vector3.right);
                break;

            case 2:
                currentGO.transform.rotation *= Quaternion.AngleAxis(15.0f, -Vector3.right);
                break;
            case 3:
                currentGO.transform.rotation *= Quaternion.AngleAxis(15.0f, Vector3.up);
                break;
            case 4:
                currentGO.transform.rotation *= Quaternion.AngleAxis(15.0f, -Vector3.up);
                break;


        }

    }

    //public override void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.layer == 5)
    //        return;

    //    if (inputSelection.isOverObject)
    //    {
    //        attenuation = Vector3.Distance(_posOfHandLaser.position, initialPos) / initialOffset;
    //        //TAKE OUT THE ONE START AT ZERO CAN DETERMINE DIRECTION OF ROTATION FORWARD RIGHT BACK LEFT

    //        other.transform.rotation *= Quaternion.AngleAxis((attenuation -1) * _magnitudeOfRotation, Vector3.up);

          
    //    }
    //}
    public override void OnTriggerExit(Collider other)
    {
      
      
        if (currentGO == null)
            return;


        if (gizmo_toShow != null && gizmo_toShow.activeInHierarchy)
            gizmo_toShow.SetActive(false);

        try
        {
            //Net_Register_GameObject netRegisterObj = currentRigidBody.GetComponent<Net_Register_GameObject>();
            //#if !UNITY_EDITOR && UNITY_WEBGL
            var currentInteractiveObject = currentGO.GetComponent<Net_Register_GameObject>();

            NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
            {
                sourceEntity_id = ClientSpawnManager.Instance._mainClient_entityData.entityID,
                targetEntity_id = currentInteractiveObject.positionWithin_urlList,
                interactionType = (int)INTERACTIONS.DROP,
            });


            MainClientUpdater.Instance.RemoveFromInNetworkUpdateList(currentInteractiveObject);
            //    currentInteractiveObject.entity_data.isCurrentlyGrabbed = false;


        }
        catch
        {
            Debug.LogWarning("Custom Warning: " + "Could not send Interaction : ");
        }
    }
    public override void OnDisable()
    {
        if (gizmo_toShow != null && gizmo_toShow.activeInHierarchy)
            gizmo_toShow.SetActive(false);


        currentGO = null;

       
    }
}
