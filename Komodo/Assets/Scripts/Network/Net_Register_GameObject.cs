using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Net_Register_GameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Register object to reference lists on clientspawnmanager to be refered to for synchronization
    public EntityData_SO entity_data;
    public int positionWithin_urlList = -1;

    public bool isAutomatic_Register = false;

    public bool isPhysics;

    public bool isRegistered = false;


    Rigidbody thisNORigidBody;
    public void Awake()
    {
        isRegistered = false;
       
    }
    public void InitializeEarly()
    {
        //set -1 because they are not on ui list
        Instantiate(-1);
    }
    public IEnumerator Start()
    {
        if (isPhysics)
        {
            thisNORigidBody = GetComponent<Rigidbody>();

            if (!thisNORigidBody)
                Debug.LogError("No Rigidbody on a Net_Register_Game obejct with a flag of isPhysics");

        }


        if (isAutomatic_Register)
        {
      
                yield return new WaitUntil(() => ClientSpawnManager.Instance.isURL_Loading_Finished);
         

          if(isRegistered == false)
            Instantiate(-1);
        //    yield return null;
        }
    }

    //this is the unique identifier for decomposed objecct unique numbering
    private static int uniqueDefaultID;
    public void Instantiate(int positionWithinURL_Index, int uniqueEntityID = -1)
    {
        this.positionWithin_urlList = positionWithinURL_Index;

        entity_data = ScriptableObject.CreateInstance<EntityData_SO>();

        if (!isPhysics)
            entity_data.current_Entity_Type = Entity_Type.objects;
        else
            entity_data.current_Entity_Type = Entity_Type.physicsObject;

        entity_data.clientID = (uint)NetworkUpdateHandler.Instance.client_id;

        //// ENTITYID DERIVED EXAMPLE =  CLIENTID - 65, ENTITY TYPE - 3, Count - 1 = ENTITYID 6531
        if (uniqueEntityID == -1)
            entity_data.entityID = (999 * 1000) + ((int)Entity_Type.objects * 100) + (uniqueDefaultID++);//1111 + ((int)Entity_Type.objects * 10000); //Convert.ToUInt32(string.Format("{0}{1}{2}",1111, (uint)Entity_Type.objects, (uint)ClientSpawnManager.Instance._net_object_ID_List.Contains));
        else
            entity_data.entityID = uniqueEntityID;
        
        //Setup References in ClientSpawnManager
        ClientSpawnManager.Instance.RegisterNetWorkObject(entity_data.entityID, this);
        isRegistered = true;

      
            

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.rigidbody)
            return;

        if (isPhysics && collision.rigidbody.CompareTag("Interactable"))
        {
            if(!MainClientUpdater.Instance.physics_entityContainers_InNetwork_OutputList.Contains(this))
            MainClientUpdater.Instance.physics_entityContainers_InNetwork_OutputList.Add(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try { NetworkUpdateHandler.Instance.InteractionUpdate(
            new Interaction
            {
                interactionType = (int)INTERACTIONS.LOOK,
                sourceEntity_id = ClientSpawnManager.Instance._mainClient_entityData.entityID,
                targetEntity_id = entity_data.entityID,
            });
        
        } catch
        {
            Debug.LogWarning("Couldn't process look interaction event");
        }
     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            NetworkUpdateHandler.Instance.InteractionUpdate(
           new Interaction
           {
               interactionType = (int)INTERACTIONS.LOOK_END,
               sourceEntity_id = ClientSpawnManager.Instance._mainClient_entityData.entityID,
               targetEntity_id = entity_data.entityID,
           });

        }
        catch
        {
            Debug.LogWarning("Couldn't process look interaction event");
        }
    }
}
