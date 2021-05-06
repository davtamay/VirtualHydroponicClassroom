using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Trigger_Move : Trigger_Base
{
   
    public float attenuation;
    private Vector3 initialPos;
    private Transform thisTransform;
    public float _magnitudeOfMove = 3;
    private float initialOffset;

    //INPUT_PARENT_FIELDS
    InputSelection inputSelection;
    private Transform _posOfHandLaser;
    public string _interactableTag = "UIInteractable";
    public GameObject currentGO;
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
        //Debug.Log("called");
        //if (StateSwitchDependingOnVR.IsAlive)
        //{
        var curVRstate = StateSwitchDependingOnVR.Instance.isInVR;
        if (vrState == curVRstate)
            return;

        vrState = curVRstate;

        if (StateSwitchDependingOnVR.Instance.isInVR)
            ChangeCamerasReference(StateSwitchDependingOnVR.Instance._vrCamera);
        else
            ChangeCamerasReference(StateSwitchDependingOnVR.Instance._outOfVrCamera);
        // }
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


    public override void OnTriggerEnter(Collider other)
    {
        //if ui skip
        if (other.gameObject.layer == 5)
            return;

        if (other.CompareTag(_interactableTag))
        {
            //THIS IS TO AVOID LOOSING CONNECTION (INITIALPOS) TO INITIAL OBJECT - DISABLED REMOVE THE CONNECTION
            if (other.gameObject != currentGO)
            {
                currentGO = other.gameObject;
               
            }
            else
                return;

            if (gizmo_toShow != null)
                gizmo_toShow.SetActive(true);


            initialPos = thisTransform.position;
            initialOffset = Vector3.Distance(_posOfHandLaser.position, initialPos);

            initialOBJECToffset = Vector3.Distance(_posOfHandLaser.position, other.transform.position);

            initialPosOffset =  other.transform.position - _posOfHandLaser.position;



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
    private float initialOBJECToffset;
    //public Transform originalObjectParent;
    private Vector3 initialPosOffset;

    public void AddAttenuation(float atten)
    {
        initialOBJECToffset += atten;
    }

    public void SubtractAttenuation(float atten)
    {
        initialOBJECToffset -= atten;
    }


    public override void OnTriggerStay(Collider other)
    {

        if (other.gameObject.layer == 5)
            return;


        if (inputSelection.isOverObject)
        {
            attenuation = Vector3.Distance(_posOfHandLaser.position, initialPos) / initialOffset;

            //TAKE OUT THE ONE START AT ZERO CAN DETERMINE DIRECTION OF ROTATION FORWARD RIGHT BACK LEFT

        }
    }
    public void Update()
    {
        if (currentGO != null)
        {
            currentGO.transform.position = _posOfHandLaser.forward.normalized * initialOBJECToffset + _posOfHandLaser.position;
        }
        else
        {
            if (gizmo_toShow != null && gizmo_toShow.activeInHierarchy)
                gizmo_toShow.SetActive(false);
        }

    }
    public override void OnTriggerExit(Collider other)
    {
        if (currentGO == null)
            return;

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

        }
        catch
        {
            Debug.LogWarning("Custom Warning: " + "Could not send Interaction : ");
        }


    }
    public override void OnDisable()
    {

        if (currentGO == null)
            return;

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
       
        currentGO = null;
    }
}
