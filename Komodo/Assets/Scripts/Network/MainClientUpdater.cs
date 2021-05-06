using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class Coord_UnityEvent : UnityEvent<Position> { }

public class MainClientUpdater : SingletonComponent<MainClientUpdater>, IUpdatable
{

    public Coord_UnityEvent coordExport;
    public Transform[] transformsNetworkOutput;

    public List<Net_Register_GameObject> entityContainers_InNetwork_OutputList = new List<Net_Register_GameObject>();
    public List<Net_Register_GameObject> physics_entityContainers_InNetwork_OutputList = new List<Net_Register_GameObject>();

    private uint length;
    
    [SerializeField] private int clientID;

    [SerializeField]
    private bool isSendingUpdates;

    public Vector3 leftHandOriginalLocalPosition;
    public Vector3 rightHandOriginalLocalPosition;

    public static MainClientUpdater Instance
    {
        get { return ((MainClientUpdater)_Instance); }
        set { _Instance = value; }
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => ClientSpawnManager.Instance.isClientAvatarSetup_Finished );
            isSendingUpdates = true;

        GameStateManager.Instance.RegisterUpdatableObject(this);

        clientID = NetworkUpdateHandler.Instance.client_id;

        leftHandOriginalLocalPosition = transformsNetworkOutput[1].localPosition;
        rightHandOriginalLocalPosition = transformsNetworkOutput[2].localPosition;


    }
   
    public void OnDisable()
    {
        if(GameStateManager.IsAlive)
        GameStateManager.Instance.DeRegisterUpdatableObject(this);
    }
    public void PlaceInNetworkUpdateList(Net_Register_GameObject nRO)
    {
        if (!entityContainers_InNetwork_OutputList.Contains(nRO))
            entityContainers_InNetwork_OutputList.Add(nRO);
    }
    public void RemoveFromInNetworkUpdateList(Net_Register_GameObject nRO)
    {
        if (entityContainers_InNetwork_OutputList.Contains(nRO))
            entityContainers_InNetwork_OutputList.Remove(nRO);

        //delay removing entity
        //  StartCoroutine(DelayRemoveFromNetwork(nRO));
    }
    public IEnumerator DelayRemoveFromNetwork(Net_Register_GameObject nRO)
    {
        yield return new WaitForSeconds(0.8f);

        //untrigger despawn -> allow to update to remove 
        if (entityContainers_InNetwork_OutputList.Contains(nRO))
            entityContainers_InNetwork_OutputList.Remove(nRO);

    }

    public void RemoveALLInNetworkUpdateList(Net_Register_GameObject nRO) => entityContainers_InNetwork_OutputList.Clear();
    


    public void OnUpdate(float realTime)
    {
        if (!isSendingUpdates)
            return;

       
        //HEAD
        SendUpdatesToNetwork(Entity_Type.users_head, transformsNetworkOutput[0].position, transformsNetworkOutput[0].rotation);

        //L_HAND
        if (leftHandOriginalLocalPosition != transformsNetworkOutput[1].localPosition)
        {
            SendUpdatesToNetwork(Entity_Type.users_Lhand, transformsNetworkOutput[1].position, transformsNetworkOutput[1].rotation);
            leftHandOriginalLocalPosition = transformsNetworkOutput[1].localPosition;
        }

        //R_HAND
        if (rightHandOriginalLocalPosition != transformsNetworkOutput[2].localPosition)
        {
            SendUpdatesToNetwork(Entity_Type.users_Rhand, transformsNetworkOutput[2].position, transformsNetworkOutput[2].rotation);
            rightHandOriginalLocalPosition = transformsNetworkOutput[2].localPosition;

        }
        //NETWORK_OBJECTS
        foreach (var entityContainers in entityContainers_InNetwork_OutputList)
            Send_GameObject_UpdatesToNetwork(entityContainers);

        foreach (var entityContainers in physics_entityContainers_InNetwork_OutputList)
            Send_PHYSICS_GameObject_UpdatesToNetwork(entityContainers);

        foreach (var item in nOToRemove)
        {
            physics_entityContainers_InNetwork_OutputList.Remove(item);
        }
        nOToRemove.Clear();

    }

    public void SendUpdatesToNetwork(Entity_Type entityType, Vector3 position, Quaternion rotation)
    {

        Position coords = new Position
        {
            clientId = this.clientID,
            entityId = (this.clientID * 10) + (int)entityType,
            entityType = (int)entityType,
            rot = rotation,
            pos = position,
        };
        coordExport.Invoke(coords);

    }


    public void Send_GameObject_UpdatesToNetwork(Net_Register_GameObject eContainer)
    {

        Position coords = new Position
        {
            clientId = this.clientID,//(int) eContainer.entity_data.clientID, 
            entityId = (int)eContainer.entity_data.entityID,//((int)eContainer.entity_data.clientID * 10) + (int)eContainer.entity_data.current_Entity_Type,
            entityType = (int)eContainer.entity_data.current_Entity_Type,
            rot = eContainer.transform.rotation,
            pos = eContainer.transform.position,

            //use x to give a definite scale rather than obtaining averages.
            //since using parenting for objects, we need to translate local to global scalling when having it in your hand, when releasing we need to return such objects scalling from global to local scale
            scaleFactor = eContainer.transform.lossyScale.x,
            //  scaleFactor = eContainer.transform.localScale.x,
        };
        coordExport.Invoke(coords);

    }

    List<Net_Register_GameObject> nOToRemove = new List<Net_Register_GameObject>();

    public void Send_PHYSICS_GameObject_UpdatesToNetwork(Net_Register_GameObject eContainer)
    {
        
        if (!ClientSpawnManager.Instance._EntityID_To_RigidBody.ContainsKey(eContainer.entity_data.entityID))
        {
            ClientSpawnManager.Instance._EntityID_To_RigidBody.Add(eContainer.entity_data.entityID , eContainer.GetComponent<Rigidbody>());
        }

        var rb = ClientSpawnManager.Instance._EntityID_To_RigidBody[eContainer.entity_data.entityID]; // _EntityID_To_RigidBody[newData.entityId];

        if (!rb)
        {
            Debug.LogError("There is no rigidbody in netobject entity id DICTIONARY: " + eContainer.entity_data.entityID);
            return;
        }
        //remove if physics is sleeping
        if(!rb.isKinematic && rb.IsSleeping() 
           //if is currently grabbed
            || eContainer.entity_data.isCurrentlyGrabbed)
        {

            nOToRemove.Add(eContainer);
            SetBackPHYSICS(eContainer);

        //    rb.Sleep();
        }
            
        Position coords = new Position
        {
            clientId = this.clientID,//(int) eContainer.entity_data.clientID, 
            entityId = (int)eContainer.entity_data.entityID,//((int)eContainer.entity_data.clientID * 10) + (int)eContainer.entity_data.current_Entity_Type,
            entityType = (int)eContainer.entity_data.current_Entity_Type,
            rot = eContainer.transform.rotation,
            pos = eContainer.transform.position,

            //use x to give a definite scale rather than obtaining averages.
            //since using parenting for objects, we need to translate local to global scalling when having it in your hand, when releasing we need to return such objects scalling from global to local scale
            scaleFactor = eContainer.transform.lossyScale.x,
            //  scaleFactor = eContainer.transform.localScale.x,
        };
        coordExport.Invoke(coords);

    }

    public void SetBackPHYSICS(Net_Register_GameObject eContainer)
    {
        
        Position coords = new Position
        {
            clientId = this.clientID,//(int) eContainer.entity_data.clientID, 
            entityId = (int)eContainer.entity_data.entityID,//((int)eContainer.entity_data.clientID * 10) + (int)eContainer.entity_data.current_Entity_Type,
            entityType = 33,
            //rot = eContainer.transform.rotation,
            //pos = eContainer.transform.position,

            ////use x to give a definite scale rather than obtaining averages.
            ////since using parenting for objects, we need to translate local to global scalling when having it in your hand, when releasing we need to return such objects scalling from global to local scale
            //scaleFactor = eContainer.transform.lossyScale.x,
            //  scaleFactor = eContainer.transform.localScale.x,
        };
        coordExport.Invoke(coords);

    }

}