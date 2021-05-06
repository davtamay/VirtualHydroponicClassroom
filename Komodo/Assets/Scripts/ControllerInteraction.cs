using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using WebXR;

public enum INTERACTIONS
{
    LOOK = 0,
    LOOK_END = 1,
    RENDERING = 2,
    NOT_RENDERING = 3,
    GRAB = 4,
    DROP = 5,
    CHANGE_SCENE = 6,
    SLICE_OBJECT = 7,
    LOCK = 8,
    UNLOCK = 9,
    LINE = 10,
    LINE_END = 11,
    START_VIDEO = 12,
    STOP_VIDEO = 13,
    TIMEREFRESH_VIDEO = 14,

    
    ADDPIENCE = 15,
    OPENBOX = 16,
}
//cant grab same object twice with same hand
public class ControllerInteraction : MonoBehaviour, IUpdatable
{
    [System.Serializable]
    public class InteractionEvent : UnityEvent<Interaction> { }
    public InteractionEvent onInteract;
    public UnityEvent onTriggerButtonDown;
    public UnityEvent onTriggerButtonUp;
    public UnityEvent onGripButtonDown;
    public UnityEvent onGripButtonUp;
    public UnityEvent onPrimaryButtonDown;
    public UnityEvent onPrimaryButtonUp;
    public UnityEvent onSecondaryButtonDown;
    public UnityEvent onSecondaryButtonUp;
    public UnityEvent onThumbstickButtonDown;
    public UnityEvent onThumbstickButtonUp;
    public UnityEvent onLeftFlick;
    public UnityEvent onRightFlick;
    public UnityEvent onDownFlick;
    public UnityEvent onUpFlick;
    private bool isHorAxisReset;
    private bool isVerAxisReset;    
    public Transform currentTransform = null;
    public Transform currentParent = null;

    public static Transform curSharedParTransform;
    public Rigidbody currentRB;
    public Net_Register_GameObject currentNetRegisteredGameObject;

    //public Net_Register_GameObject currentNetRegister;
  //  public Rigidbody currentRigidbody;
    public static Transform firstObjectGrabbed;
    public static Transform secondObjectGrabbed;
    public List<Transform> contactTransformList = new List<Transform>();

    //to keep decompositional objects together with parent object
    //private Dictionary<Transform, Transform> contactTransformParentDic = new Dictionary<Transform, Transform>();
    //private Dictionary<Transform, Rigidbody> contactTransformRigidBody = new Dictionary<Transform, Rigidbody>();
  //  private Dictionary<Transform, Net_Register_GameObject> transformToNetRegisteredObjectDictionary = new Dictionary<Transform, Net_Register_GameObject>();

  //  [SerializeField] private GameObject pointerInputObject;
    [SerializeField] private bool hasObject;
    //[SerializeField] private static int currentObjectId;
    private Transform thisTransform;
    private Animator thisAnimCont;
    WebXRController webXRController;
    public static List<GameObject> handTransform;
    private Collider thisCollider;
    private Rigidbody thisRigidBody;
    public static ControllerInteraction firstControllerInteraction;
    public static ControllerInteraction secondControllerInteraction;
    public int handEntityType;
    public GameObject clientMenu;
    public Vector3 oldPos;
    public Vector3 newPos;
    public Vector3 velocity;
    public float throwForce = 3f;
    public Vector3 initialScale;
    public float initialDistance;
    public bool isBothHandsHaveObject;
    public static bool isInitialDoubleGrab;

    void Awake()
    {
        if (firstControllerInteraction == null)
            firstControllerInteraction = this;
        else
            secondControllerInteraction = this;

        //add both hands
        handTransform = new List<GameObject>(2);
        handTransform.AddRange(GameObject.FindGameObjectsWithTag("Hand"));

        thisTransform = transform; 
        thisAnimCont = gameObject.GetComponent<Animator>();
        thisRigidBody = GetComponent<Rigidbody>();
    }    
    
    void Start()
    {
        handEntityType = (int)GetComponent<Entity_Container>().entity_data.current_Entity_Type;

        thisCollider = GetComponent<Collider>();

        GameStateManager.Instance.RegisterUpdatableObject(this);
        webXRController = gameObject.GetComponent<WebXRController>();

        oldPos = thisTransform.position;
    }

    public void OnDisable()
    {
        //send call to release object as last call
        if (currentTransform)
            if (currentNetRegisteredGameObject)
            {
                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = int.Parse(ClientSpawnManager.Instance.mainPlayer_head.clientID.ToString() + handEntityType.ToString()),
                    targetEntity_id = currentNetRegisteredGameObject.entity_data.entityID,// currentNetRegisteredGameObject.positionWithin_urlList,
                    interactionType = (int) INTERACTIONS.DROP,
                });
            }

        if (GameStateManager.IsAlive)
            GameStateManager.Instance.DeRegisterUpdatableObject(this);
    }

    public void OnUpdate(float realTime)  {
        //to enable throwing physics objects
        if (currentTransform) {
            if (currentRB) {
                newPos = thisTransform.position;
                var dif = newPos - oldPos;
                velocity = dif / Time.deltaTime;
                oldPos = newPos;
            }
        }

        float triggerThreshold = 0.5f;
        float gripThreshold = 0.5f;

//#if UNITY_EDITOR

        //bool isTriggerButtonDown = webXRController.GetAxis("Trigger") > triggerThreshold;
        //bool isTriggerButtonUp = webXRController.GetAxis("Trigger") <= triggerThreshold;
        //bool isGripButtonDown = webXRController.GetAxis("Grip") > gripThreshold;
        ////is called every frame if not
        //bool isGripButtonUp = false;// webXRController.GetAxis("Grip") <= gripThreshold;

//#elif UNITY_WEBGL

        bool isTriggerButtonDown = webXRController.GetButtonDown("Trigger");
        bool isTriggerButtonUp = webXRController.GetButtonUp("Trigger");
        bool isGripButtonDown = webXRController.GetButtonDown("Grip");
        bool isGripButtonUp = webXRController.GetButtonUp("Grip");
        
//#endif
        float normalizedTime = webXRController.GetButton("Trigger") ? 1 : webXRController.GetAxis("Grip");
        thisAnimCont.Play("Take", -1, normalizedTime);

        if (isGripButtonDown)
        {
            onGripButtonDown.Invoke();
            PickUp();
        }

        if (isGripButtonUp)
        {
            onGripButtonUp.Invoke();
            Drop();
        }

        if (isTriggerButtonUp)
        {
            onTriggerButtonUp.Invoke();
        }

        if (isTriggerButtonDown)
        {
            onTriggerButtonDown.Invoke();
        }

        //A button - GamepadID = 4 either axis or button conflict
        bool isPrimaryButtonDown = webXRController.GetButtonDown("PrimaryButton");
        bool isPrimaryButtonUp = webXRController.GetButtonUp("PrimaryButton");
        bool isSecondaryButtonDown = webXRController.GetButtonDown("SecondaryButton");
        bool isSecondaryButtonUp = webXRController.GetButtonUp("SecondaryButton");

        //A button - primarybutton
        if (isPrimaryButtonDown)
        {
            onPrimaryButtonDown.Invoke();
        }

        if (isPrimaryButtonUp)
        {
            onPrimaryButtonUp.Invoke();
        }

        if (isSecondaryButtonDown)
        {
            onSecondaryButtonDown.Invoke();
        }

        if (isSecondaryButtonUp)
        {
            onSecondaryButtonUp.Invoke();
        }

        //ThumbstickButton setup
        bool isThumbstickButtonDown = webXRController.GetButtonDown("ThumbstickPress");
        bool isThumbstickButtonUp = webXRController.GetButtonUp("ThumbstickPress");
        float horAxis = webXRController.GetAxis("ThumbstickX");
        float verAxis = webXRController.GetAxis("ThumbstickY");

        //Reset Horizontal Flick
        if (horAxis >= -0.5f && horAxis <= 0.5f)
        {
            isHorAxisReset = true;
        }

        //Left flick
        if (horAxis < -0.5f && isHorAxisReset)
        {
            isHorAxisReset = false;
            onRightFlick.Invoke();
        }

        //Right flick
        if (horAxis > 0.5f && isHorAxisReset)
        {
            isHorAxisReset = false;
            onLeftFlick.Invoke();
        }

        //Reset Vertical Flick
        if (verAxis >= -0.5f && verAxis <= 0.5f)
        {
            isVerAxisReset = true;
        }

        if (verAxis < -0.5f && isVerAxisReset)
        {
            isVerAxisReset = false;
            onDownFlick.Invoke();
        }

        if (verAxis > 0.5f && isVerAxisReset)
        {
            isVerAxisReset = false;
            onUpFlick.Invoke();
        }

        if (isThumbstickButtonDown)
        {
           onThumbstickButtonDown.Invoke();
        }

        if (isThumbstickButtonUp)
        {
           onThumbstickButtonUp.Invoke();
        }

        if (isBothHandsHaveObject)
        {
            if (firstObjectGrabbed == null)
            {
                isBothHandsHaveObject = false;
                return;
            }

            if (isInitialDoubleGrab == false)
            {
                isInitialDoubleGrab = true;
                initialDistance = Vector3.Distance(handTransform[0].transform.position, handTransform[1].transform.position);
                initialScale = firstObjectGrabbed.transform.localScale;
                return;
            }

            var scaleRatioBasedOnDistance = Vector3.Distance(handTransform[0].transform.position, handTransform[1].transform.position) / initialDistance;

            if (float.IsNaN(firstObjectGrabbed.transform.localScale.y)) return;

            firstObjectGrabbed.transform.localScale = initialScale * scaleRatioBasedOnDistance;
            firstObjectGrabbed.transform.position = handTransform[0].transform.position + (handTransform[1].transform.position - handTransform[0].transform.position) / 2;
        }
    }

    public IEnumerator SetActionAfterAnimation()
    {
        thisAnimCont.Play("Take", -1, 1);
        yield return null;
    }

    [ContextMenu("PICK UP")]
    public void PickUp()
    {
        //Physics.SyncTransforms();
        if (!hasObject)
        {
         
            currentTransform = null;

            Collider[] colls = Physics.OverlapSphere(this.thisTransform.position, 0.1f);

          
            currentTransform = GetNearestRigidBody(colls);

           
           


            if (!currentTransform)
                return;


            var grabEvent = currentTransform.GetComponent<IGrabable>();
            if (grabEvent != null)
                grabEvent.OnPickUp();
        //    currentTransform.SetParent(transform.GetChild(0), false);

            if (currentNetRegisteredGameObject)
            {
              //  Debug.LogError("cLIENT PICK UP");
                //don't grab objects that are being grabbed by others avoid disqalifying user's second hand
                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = int.Parse(ClientSpawnManager.Instance.mainPlayer_head.clientID.ToString() + handEntityType.ToString()),
                    targetEntity_id = currentNetRegisteredGameObject.entity_data.entityID,
                    interactionType = (int) INTERACTIONS.GRAB,
                });

                MainClientUpdater.Instance.PlaceInNetworkUpdateList(currentNetRegisteredGameObject);
            }


            //check if first hand has the same object as the second hand 
            if (firstObjectGrabbed == currentTransform && secondControllerInteraction == this || 
                secondObjectGrabbed == currentTransform && firstControllerInteraction == this)
            {
                isBothHandsHaveObject = true;
            }


            //check first hand if it has object
            if (firstControllerInteraction == this && firstObjectGrabbed == null)
            {
                firstObjectGrabbed = currentTransform;

            //    Debug.Log(firstObjectGrabbed); 

                //ATTACH CHILD HERE!!!
                currentTransform.SetParent(thisTransform, true);

                if (currentRB)
                    currentRB.isKinematic = true;
            }

            //check second hand if it has object
            if (secondControllerInteraction == this && secondObjectGrabbed == null)
            {
              

                secondObjectGrabbed = currentTransform;
               // Debug.Log(secondObjectGrabbed);
                currentTransform.SetParent(thisTransform, true);

                if (currentRB)
                    currentRB.isKinematic = true;
            }

            hasObject = true;
        }

    }

    [ContextMenu("DROP")]
    public void Drop()
    {
        if (hasObject)
        {

            


            //Both hand event, one hand releases object and removes object scalling
            if (firstObjectGrabbed && secondObjectGrabbed)
            {
                //if same object double grab release setup
                if (firstObjectGrabbed == secondObjectGrabbed)
                {
                    //setpositionto_oneLeft
                    firstObjectGrabbed.transform.position = handTransform[0].transform.position + (handTransform[1].transform.position - handTransform[0].transform.position) / 2;

                    if (secondControllerInteraction == this)
                    {
                        //reattach to other hand
                        currentTransform.SetParent(firstControllerInteraction.thisTransform, true);
                        secondObjectGrabbed = null;
                    }
                    else if (firstControllerInteraction == this)
                    {
                        currentTransform.SetParent(secondControllerInteraction.thisTransform, true);
                        firstObjectGrabbed = null;
                    }

                    firstControllerInteraction.isBothHandsHaveObject = false;
                    secondControllerInteraction.isBothHandsHaveObject = false;

                    isInitialDoubleGrab = false;
                }
                else
                {
                    if (secondControllerInteraction == this)
                    {
                        secondObjectGrabbed = null;
                    }
                    else if (firstControllerInteraction == this)
                    {
                        firstObjectGrabbed = null;
                    }

                    if (currentParent)
                        currentTransform.SetParent(currentParent, true);

                    //set physics 
                    if (currentRB)
                    {
                        currentRB.isKinematic = false;

                        //throw object
                        currentRB.AddForce(velocity * throwForce, ForceMode.Impulse);
                    }
                    //else//to send corect physics system data set it here
                }

            }
            //check to remove appropriate object from whichever hand
            else if (firstObjectGrabbed == null || secondObjectGrabbed == null)
            {
                if (secondControllerInteraction == this)
                    secondObjectGrabbed = null;
                else if (firstControllerInteraction == this)
                    firstObjectGrabbed = null;

                if (currentParent)
                    currentTransform.SetParent(currentParent, true);

                if (curSharedParTransform)
                {
                    currentTransform.SetParent(curSharedParTransform, true);
                    curSharedParTransform = null;
                }
                //set physics 
                if (currentRB)
                {
                    currentRB.isKinematic = false;

                    //throw object
                    currentRB.AddForce(velocity * throwForce, ForceMode.Impulse);
                }
            }


            if (currentNetRegisteredGameObject)
            {
                //  Debug.LogError("cLIENT DROP");
                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = int.Parse(ClientSpawnManager.Instance.mainPlayer_head.clientID.ToString() + handEntityType.ToString()),
                    targetEntity_id = currentNetRegisteredGameObject.entity_data.entityID,//currentNetRegisteredGameObject.entity_data.entityID, // transformToNetRegisteredObjectDictionary[currentTransform].entity_data.entityID,
                    interactionType = (int)INTERACTIONS.DROP,
                });
          

                MainClientUpdater.Instance.RemoveFromInNetworkUpdateList(currentNetRegisteredGameObject);

                //if droping a physics object update it for all.
                if (currentRB)
                    if (!MainClientUpdater.Instance.physics_entityContainers_InNetwork_OutputList.Contains(currentNetRegisteredGameObject))
                        MainClientUpdater.Instance.physics_entityContainers_InNetwork_OutputList.Add(currentNetRegisteredGameObject);


                var grabEvent = currentTransform.GetComponent<IGrabable>();
                if (grabEvent != null)
                    grabEvent.OnDrop();
            }




            //currentRB
            currentTransform = null;
            hasObject = false;
        }
    }

   


    private Transform GetNearestRigidBody(Collider[] colliders)
    {
        float minDistance = float.MaxValue;
        float distance = 0.0f;
        List<Transform> transformToRemove = new List<Transform>();
        Collider nearestTransform = null;

        foreach (Collider col in colliders)
        {
         
            if (!col.CompareTag("Interactable"))
                continue;

            if (!col.gameObject.activeInHierarchy)
                continue;

            distance = (col.ClosestPoint(thisTransform.position) - thisTransform.position).sqrMagnitude; // (contactBody.position - thisTransform.position).sqrMagnitude;

            if (distance > 0.01f)
                continue;

         //   Debug.Log("pick up is called");
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTransform = col;
            }
        }
        // didnt find nearest collider return null
        if (nearestTransform == null)
            return null;


      

        currentRB = null;
        currentParent = null;
        currentNetRegisteredGameObject = null;

        Transform nearPar = null;

        //set shared parent to reference when changing hands - set this ref when someone is picking up first object and//
        //whenever someone has on object on left hand then grabs that same object with the right hand, releases right hand to grab new object
        //with the left hand grab this new object - however, the shared parent is still the left

        //set last object to be picked up as the shared parent
       
        nearPar = nearestTransform.transform.parent;

        if (nearPar)
            if (nearPar != firstControllerInteraction.thisTransform && nearPar != secondControllerInteraction.thisTransform)
            {
                curSharedParTransform = nearestTransform.transform.parent;
                currentParent = nearestTransform.transform.parent;
            }

        var netObj = nearestTransform.GetComponent<Net_Register_GameObject>();

        if (netObj)
        {
            //return null if the object is considered grabbed or locked
            if (netObj.entity_data.isCurrentlyGrabbed)
                return null;

             currentNetRegisteredGameObject = netObj;

            if (currentNetRegisteredGameObject.entity_data.current_Entity_Type == Entity_Type.physicsObject)
            {
                currentRB = currentNetRegisteredGameObject.GetComponent<Rigidbody>();

                if (currentRB == null)
                    Debug.LogWarning("No Rigid body on physics object Entity Type");
            }
        }
        return nearestTransform.transform;
    }
}
