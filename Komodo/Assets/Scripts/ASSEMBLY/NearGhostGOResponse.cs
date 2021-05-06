using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class NearGhostGOResponse : MonoBehaviour
{
    //PLACE THIS ON EACH GHOST GAME OBJECT TO HELP SNAP OBJECTS TO GHOST OBJEC
    
    //these will Emit correct response GO
    public List<GameObject> correctObjectList;

    private Net_Register_GameObject netGOToCheck;

    public Material correctMaterial;
    public Material wrongMaterial;
  
    public Material defaultMaterial;

    public MeshRenderer thisRenderer;
    public Collider thisCollider;

    public bool isCorrect = false;

    public GameObject currentGO;
    public void Start()
    {
        thisRenderer = GetComponent<MeshRenderer>();
        thisCollider = GetComponent<Collider>();
    }
    //erase it from our setactive list
    public AssemblyObjectGrabResponse currentAssemblyResponse;
    public void OnTriggerEnter(Collider col)
    {
      

        if (col.CompareTag("Interactable") && correctObjectList.Contains(col.gameObject))
        {
            isCorrect = true;

            currentAssemblyResponse = col.GetComponent<AssemblyObjectGrabResponse>();
        //    Debug.Log("Trigger yes");
            netGOToCheck = col.GetComponent<Net_Register_GameObject>();
          
             thisRenderer.material = correctMaterial;

            currentGO = col.gameObject;

         
        }
        else
        {

            thisRenderer.material = defaultMaterial;// wrongMaterial;
        }
    }
   
    public void OnTriggerExit(Collider col)
    {


        if (col.CompareTag("Interactable") && correctObjectList.Contains(col.gameObject))
        {
            //ontriggerExit is not called when gameObject ACTIVE is Set to false;
            isCorrect = false;
            Debug.Log("TRIGGER EXIT CALLED");
        }
        else
        {

            isCorrect = false;
          // wrongMaterial;
        }
        thisRenderer.material = defaultMaterial;


        //MeshRenderer mr = col.GetComponent<MeshRenderer>();



        //    thisRenderer.material = defaultMaterial;


        //netGOToCheck = null;









    }
    public void OnDisable()
    {
        //need to account if a piece is touching two sme ghost objects, which will cause the object to be placed in later ghost wile disabling other ghost
        if (isCorrect)
        {
            GameObject toRemove = null;
            List<GameObject> assemblyRespList = new List<GameObject>();
            List<GameObject> ghostRespList = new List<GameObject>();

            for (int i = 0; i < correctObjectList.Count; i++)
            {
                //remmove ghost objects associated with this object
                var ghost = correctObjectList[i].GetComponent<AssemblyObjectGrabResponse>();
                //  ghostobj = ghost;
                if (i == 0)
                {
                    ghostRespList = ghost.listOfGhostObjectsToSetActiveWhenGrabbed;
                    toRemove = correctObjectList[0];
                }

                if (ghost.listOfGhostObjectsToSetActiveWhenGrabbed.Contains(gameObject))
                    ghost.listOfGhostObjectsToSetActiveWhenGrabbed.Remove(gameObject);

            }

            for (int e = 0; e < ghostRespList.Count; e++)
            {
                var assembly = ghostRespList[e].GetComponentInChildren<NearGhostGOResponse>(true);

                assembly.correctObjectList.Remove(toRemove);
            }

            //remove from our active list
            //foreach (var item in correctObjectList)
            //{
            //    var piece = item.GetComponent<AssemblyObjectGrabResponse>();
            //    piece.listOfGhostObjectsToSetActiveWhenGrabbed.Contains(gameObject);
            //    piece.listOfGhostObjectsToSetActiveWhenGrabbed.Remove(gameObject);
            //}

            ////detect how many pieces we have accumulated to invoke funcions
            ClientSpawnManager.Instance.AddToPieceCounter();

            //netGOToCheck.entity_data.isCurrentlyGrabbed = true;

            //correctObjectList.Remove(currentGO.gameObject);

            //snap this object to ghost object
            currentGO.transform.position = transform.position;
            currentGO.transform.rotation = transform.rotation;

            //turn of ghost object
            thisRenderer.enabled = false;
            thisCollider.enabled = false;

         //  SendNetworkUpdatesAndRemove(netGOToCheck);

        }
        
    }
    public async void SendNetworkUpdatesAndRemove(Net_Register_GameObject nrGO)
    {
        MainClientUpdater.Instance.PlaceInNetworkUpdateList(nrGO);
        await Task.Delay(10000);
        MainClientUpdater.Instance.RemoveFromInNetworkUpdateList(nrGO);

        //   Task.

    }

    //public IEnumerator SendNetworkUpdatesAndRemove(Net_Register_GameObject nrGO)
    //{
    //    MainClientUpdater.Instance.PlaceInNetworkUpdateList(nrGO);
    //    yield return new WaitForSeconds(2);

    //}
}
