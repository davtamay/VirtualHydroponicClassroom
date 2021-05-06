using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssemblyObjectGrabResponse : MonoBehaviour, IGrabable, IPointerClickHandler//, IPointerExitHandler
{
    //SET ASSEMBLY TAG ON THIS OBJECT
    public List<GameObject> listOfGhostObjectsToSetActiveWhenGrabbed;

    public NearGhostGOResponse curGhostGO;

    public Net_Register_GameObject nrGO;

    public void Start()
    {
        nrGO = GetComponent<Net_Register_GameObject>();
    }
    public void OnPickUp()
    {
        nrGO.entity_data.isCurrentlyGrabbed = true;
        for (int i = 0; i < listOfGhostObjectsToSetActiveWhenGrabbed.Count; i++)
        {
            listOfGhostObjectsToSetActiveWhenGrabbed[i].SetActive(true);
        }
       
    }

    public void OnDrop()
    {

        nrGO.entity_data.isCurrentlyGrabbed = false;
        for (int i = 0; i < listOfGhostObjectsToSetActiveWhenGrabbed.Count; i++)
        {
            listOfGhostObjectsToSetActiveWhenGrabbed[i].SetActive(false);
        }
        SendNetworkUpdatesAndRemove(nrGO);

    }
    public async void SendNetworkUpdatesAndRemove(Net_Register_GameObject nrGO)
    {
        //await Task.Delay(1);
        MainClientUpdater.Instance.PlaceInNetworkUpdateList(nrGO);
        await Task.Delay(10000);
        MainClientUpdater.Instance.RemoveFromInNetworkUpdateList(nrGO);

        //   Task.

    }

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}
    //public int PIECEcount;
    //place in an open slot
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject toRemove = null;
        List<GameObject> assemblyRespList = new List<GameObject>();
        List<GameObject> ghostRespList = new List<GameObject>();

        for (int i = 0;  i < listOfGhostObjectsToSetActiveWhenGrabbed.Count;  i++)
        {
            //remmove ghost objects associated with this object
            var ghost = listOfGhostObjectsToSetActiveWhenGrabbed[i].GetComponent<NearGhostGOResponse>();
            //  ghostobj = ghost;
            if (i == 0)
            {
                ghostRespList = ghost.correctObjectList;
                toRemove = listOfGhostObjectsToSetActiveWhenGrabbed[0];
            }

            if (ghost.correctObjectList.Contains(gameObject))
                ghost.correctObjectList.Remove(gameObject);
         
        }

        for (int e = 0; e < ghostRespList.Count; e++)
        {
         var assembly = ghostRespList[e].GetComponentInChildren<AssemblyObjectGrabResponse>(true);

            assembly.listOfGhostObjectsToSetActiveWhenGrabbed.Remove(toRemove);
        }

        ClientSpawnManager.Instance.AddToPieceCounter();

        transform.position = toRemove.transform.position;
        transform.rotation = toRemove.transform.rotation;

        SendNetworkUpdatesAndRemove(nrGO);

    }
}
