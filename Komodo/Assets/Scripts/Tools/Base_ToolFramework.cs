using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Base_ToolFramework : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Collider colliderInput;

    public string tagFlagMask_ForUIButton = "UIInteractable";

    public Vector3? newPlayerPos;
    public Vector3? newPlayerEulerRot;

    public bool isFloorDetector;
    public GameObject floorObj;


    int layerMaskIgnore = ~(1 << 2 | 1 << 10);
    int layerMaskWater = 1 << 9;

    public void OnEnable()=> colliderInput.enabled = true;

    public void OnDisable()
    {
        if (floorObj)
            floorObj.SetActive(false);

        colliderInput.enabled = false;
    }
    
    bool isKeepLocation;
    Vector3 locationToKeep;


    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        colliderInput = transform.parent.GetComponentInChildren<Collider>(true);

        if (floorObj)
            floorObj.SetActive(false);
    }


    public bool isOverObject;
    public bool isKeepCollision;
    public Button currentButton;


    public bool isInitialHit;
    public Vector3? initialLoc;
    public void Update()
    {
        if (colliderInput.enabled == false)
            return;

        Vector3 pos = transform.position + (transform.forward * 20);
        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, pos);

        if (!isFloorDetector)
        {
            //PLACING THIS HERE AFFECTS WORLD COLLIDER INTERACTIONS WITH LINE RENDERER?
            if (Physics.Linecast(transform.position, pos, out RaycastHit hit, layerMaskIgnore))
            {
             //   initialLoc = null;

                if (!isInitialHit)
                {
                    isInitialHit = true;
                    initialLoc = hit.point;
                    
                }

                if (isInitialHit)
                {
                   var dis = Vector3.Distance((Vector3)initialLoc, this.transform.position);
                    Debug.Log(dis);

                }


                //pos = hit.point;
                //if (hit.collider.CompareTag(tagFlagMask_ForUIButton))
                //{
                //    isOverObject = true;
                //    colliderInput.transform.position = hit.point;
                //}
                //else
                //{
                //    colliderInput.transform.position = hit.point;
                //    isOverObject = false;
                //}
            }
            else
            {
                initialLoc = null;
                isInitialHit = false;
                isOverObject = false;
            }
            lineRenderer.SetPosition(1, pos);

        }
        //else
        //{

        //    if (Physics.Linecast(transform.position, pos, out RaycastHit hit, layerMaskWater))//, LayerMask.GetMask("Walkable"), QueryTriggerInteraction.Collide))
        //    {

        //        pos = hit.point;


        //        if (!floorObj.activeInHierarchy)
        //            floorObj.SetActive(true);


        //        floorObj.transform.position = pos;

        //        newPlayerPos = pos;

        //        colliderInput.transform.position = pos;

        //        isKeepLocation = true;
        //        locationToKeep = pos;



        //    }

        //    //BREAK CURRENT TELEPORTATION
        //    if (lineRenderer.GetPosition(1).y > 1.8f)
        //    {
        //        pos = transform.position + (transform.forward * 20);
        //        colliderInput.transform.position = pos;
        //        newPlayerPos = null;
        //        floorObj.SetActive(false);

        //        isKeepLocation = false;
        //    }


        //    if (isKeepLocation)
        //    {
               

        //        lineRenderer.SetPosition(2, locationToKeep);
        //    }
        //    else
        //    {
               


        //        lineRenderer.SetPosition(2, pos);
        //    }
        //}



    }
}
