using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Non_MainClientData : MonoBehaviour//, IUpdatable
{
    public Entity_Container _EntityContainer_MAIN;
    public Entity_Container _EntityContainer_hand_L;
    public Entity_Container _EntityContainer_hand_R;

    public uint Id = 0;

    private Transform handL;
    private Transform handR;

    public void Start()
    {
      //  GameStateManager.Instance.RegisterUpdatableObject(this);

        handL = _EntityContainer_hand_L.transform;
        handR = _EntityContainer_hand_L.transform;
    }
  
    //public void OnDestroy()
    //{
    //    if (GameStateManager.IsAlive)
    //        GameStateManager.Instance.DeRegisterUpdatableObject(this);

    //    //if(ClientSpawnManager.IsAlive && ClientSpawnManager.Instance._clientDict.c )
    //    //  ClientSpawnManager.Instance._clientDict.Remove(transform.gameObject);
    //}


  
    public void OnTransformUpdate()
    {
        handL.position = _EntityContainer_hand_L.entity_data.pos;
        handL.rotation = new Quaternion (_EntityContainer_hand_L.entity_data.rot.x, _EntityContainer_hand_L.entity_data.rot.y, _EntityContainer_hand_L.entity_data.rot.z, _EntityContainer_hand_L.entity_data.rot.w);
        handR.position = _EntityContainer_hand_R.entity_data.pos;
        handR.rotation = new Quaternion(_EntityContainer_hand_R.entity_data.rot.x, _EntityContainer_hand_R.entity_data.rot.y, _EntityContainer_hand_R.entity_data.rot.z, _EntityContainer_hand_R.entity_data.rot.w);

    }

 

    //public void OnUpdate(float realTime)
    //{
    //    //OnTransformUpdate();
    //}
}
