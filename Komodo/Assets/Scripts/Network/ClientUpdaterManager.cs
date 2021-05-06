//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//[System.Serializable]
//public class ClientUpdaterManager : SingletonComponent<ClientUpdaterManager>
//{
   
//    public List<GameObject> registeredClients;

 
//    public static ClientUpdaterManager Instance
//    {
//        get { return ((ClientUpdaterManager)_Instance); }
//        set { _Instance = value; }
//    }

//    public void SetSessionUniqueID(uint clientInt)
//    {

//        uint sessionID = (uint)Random.Range(0, uint.MaxValue);
//    //    var 
       
//        foreach (var item in ClientSpawnManager.Instance._clientSOList)
//        {
//           item.clientID = clientInt;
//           item.sessionID = sessionID;
           
//        }
//    }
//    public void ClientToUpdate(Coords newCords)
//    {
//        if (ClientSpawnManager.Instance._availableClientIDToGODict != null && ClientSpawnManager.Instance._availableClientIDToGODict.Count != 0)
//        {

//            foreach (var item in ClientSpawnManager.Instance._availableClientIDToGODict)
//            {
//                if (item.Value._EntityContainer_MAIN.entity_data.clientID == (uint)newCords.clientId)
//                {
//                    item.Value._EntityContainer_MAIN.entity_data.pos = newCords.pos;
//                    item.Value._EntityContainer_MAIN.entity_data.rot = new Vector4(newCords.rot.x, newCords.rot.y, newCords.rot.z, newCords.rot.w);


//                    break;
//                }
//            }

//        }
//    }



//}
