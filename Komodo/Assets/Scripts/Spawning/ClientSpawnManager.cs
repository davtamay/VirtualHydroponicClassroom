using BzKovSoft.ObjectSlicerSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class UnityEvent_Int : UnityEvent<int> { }

[System.Serializable]
public class UnityEvent_String : UnityEvent<string> { }


public struct New_Text
{
    public int target;
    public int stringType;
    public string text;

}
public enum STRINGTYPE
{
    TUTORIAL,
    CLIENT_NAME,
    DIALOGUE,
}


public class ClientSpawnManager : SingletonComponent<ClientSpawnManager>
{

    public static ClientSpawnManager Instance
    {
        get { return ((ClientSpawnManager)_Instance); }
        set { _Instance = value; }
    }

    public VideoPlayer videoPlayer;
    public VideoStopPoints videoStopPoints;

    [Header("Current User Setup")]
    public GameObject _mainPlayer;
    public EntityData_SO _mainClient_entityData;
   
    public GameObject _MAIN_UI_Interface;

    public EntityData_SO mainPlayer_head;
    public EntityData_SO mainPlayer_L_Hand;
    public EntityData_SO mainPlayer_R_Hand;

    public ChildTextCreateOnCall clientTagSetup;
    //public FreeFlightController freeFlight_desktopControlls;
    public bool isMainClientInitialized = false;

    [Space]

    [Header("Spawn_Setup")]
    public Transform _CenterToSpawnClients;
    public int _clientReserveCount;
    public float _spreadRadius;

    [Header("External User Setup")]
    public GameObject _otherPlayersDummyPrefab;
   
    public Dictionary<int, Non_MainClientData> _availableClientIDToGODict = new Dictionary<int, Non_MainClientData>();

    [Header("Network Setup References")]
    public Transform templateParentLineRenderer;
    LineRenderer templateLR;

    [Header("List of Reserved & Current Clients")]
    public List<int> _client_ID_List = new List<int>();
    public List<GameObject> _availableGOList = new List<GameObject>();
    public List<Non_MainClientData> _ExternalClientList = new List<Non_MainClientData>();

    public List<Text> _clientUser_Names_UITextReference = new List<Text>();
    public List<Text> _clientUser_Dialogue_UITextReference = new List<Text>();

    //#region NETWORK OBJECTS
    [Header("List of Network Objects")]
    public List<GameObject> allNetWork_GO_upload_list = new List<GameObject>();

    public Dictionary<int, Net_Register_GameObject> _EntityID_To_NetObject = new Dictionary<int, Net_Register_GameObject>();
    public Dictionary<int, Rigidbody> _EntityID_To_RigidBody = new Dictionary<int, Rigidbody>();
    //public Dictionary<int, int> _EntityID_to_urlListIndex = new Dictionary<int, int>();
    //public Dictionary<int, int> _urlListIndex_to_EntityID = new Dictionary<int, int>();

    //public Dictionary<int, Net_Register_GameObject> _indexURLList_To_NetObject;

    //list of decomposed for entire set locking
    public Dictionary<int, List<Net_Register_GameObject>> decomposedAssetReferences_Dict = new Dictionary<int, List<Net_Register_GameObject>>();

    public String_List listOfObjects;

    [Header("Type of Client Specific Calls")]
    public UnityEvent onClient_IsTeacher;
    public UnityEvent onClient_IsStudent;

    [Header("Setup funcions to send name to  Clients Events to Listen to")]
    //public UnityEvent_Int onClientAdded;
    //public UnityEvent_Int onClientRemoved;

    public UnityEvent_String onClientAdded;
    public UnityEvent_String onClientRemoved;


    //LIST OF NETWORK DOWNLOADED
    List<Net_Register_GameObject> downloadedAssets_NetRegComponents = new List<Net_Register_GameObject>();
    
    [Header("String References")]
    private List<int> _currentTextProcessingList = new List<int>();
    private List<string> _currentTextProcessingList_Strings = new List<string>();
    public Dictionary<int, float> secondsToWaitDic = new Dictionary<int, float>();

    [Space]

    [Header("Scene References")]
    public List<string> scene_Additives_Loaded = new List<string>();
    List<AsyncOperation> _sceneloading_asyncOperList = new List<AsyncOperation>();
    public Scene_List sceneList;

    [Header("UI Button Refferences")]
    public List<Button> _assetButtonRegisterList;
    public List<Toggle> _assetLockToggleRegisterList = new List<Toggle>();
    public List<Button> _sceneButtonRegisterList;


    //Current Session Information
    [Header("Current Session Sync")]
    public SessionState currentSessionState;

    //Keep track of what is rendered or not
    public List<bool> _renderAssetFlag = new List<bool>();

    [Space]

    [Header("Initiation Process Steps - Keep all diabled")]
    public Text loadStatusUITEXT;
    public bool isClientAvatarSetup_Finished;
    public bool isURL_Loading_Finished = false;
    public bool isUIButtonsSetup_Finished = false;
    public bool IsSyncStateSetup_Finished = false;

    //Early processing refs
    List<int> clientID_early_proces_list = new List<int>();
    List<Interaction> Interaction_Refresh_early_proces_list = new List<Interaction>();

    List<New_Text> text_Refresh_early_proces_list = new List<New_Text>();

    Dictionary<int, Position> clientRefresh_earlyProcess_Dict = new Dictionary<int, Position>();

    public int piecesAttached;
    public int piecesNeededFor1stBox;
    public UnityEvent InvokeWhenCounterReaches1sBox;

    public int piecesNeededFor2ndBox;
    public UnityEvent InvokeWhenCounterReaches2ndBox;

    public void AddToPieceCounter()
    {
        piecesAttached++;

        if (piecesNeededFor1stBox == piecesAttached)
        {

            InvokeWhenCounterReaches1sBox.Invoke();
        }

        if (piecesNeededFor2ndBox == piecesAttached)
        {
            InvokeWhenCounterReaches2ndBox.Invoke();

        }

        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.ADDPIENCE });


    }
    //initiation process --> ClientAvatars -> URL Downloads -> UI Setup -> SyncState
    public IEnumerator Start()
    {
        templateLR = templateParentLineRenderer.GetComponent<LineRenderer>();

        videoStopPoints = videoPlayer.GetComponent<VideoStopPoints>();

        //Interaction_Refresh(new Interaction { interactionType = (int)INTERACTIONS.START_VIDEO,targetEntity_id = 1 });
        //Interaction_Refresh(new Interaction { interactionType = (int)INTERACTIONS.TIMEREFRESH_VIDEO, targetEntity_id = 0 });

     

        //Debug.LogError(videoPlayer.time);
        //Debug.LogError(videoPlayer.time);
        //AddNewClient(3);

        //AddNewClient(10);
        //RemoveClient(4);

        //AddNewClient(10);

        //AddNewClient(9);
        //RemoveClient(0);
        //     Draw_Refresh(new Draw(1, 11111, 11, 1, Vector3.one * 7, Vector4.one));


        //turn of desktop controlls during initiating
        //freeFlight_desktopControlls.isUpdating = false;//enabled = false;


        //Deactivate ui interface while loading
        _MAIN_UI_Interface.SetActive(false);

        //SyncSessionState(JsonUtility.ToJson(new SessionState { entities = ent, locked = lck }));
        loadStatusUITEXT.text = "Loading Avatars To Display";
        yield return StartCoroutine(Instantiate_Reserved_Clients());
        loadStatusUITEXT.text = "Downloading and Loading Assets";

        //RESET PREVIOUS DATA
        _mainClient_entityData.rot = new Vector4();
        _mainClient_entityData.pos = Vector3.zero;

        _mainPlayer = GameObject.FindGameObjectWithTag("Player");

       
        //Setup Client ID;
        mainPlayer_head.clientID = (uint)NetworkUpdateHandler.Instance.client_id;
        mainPlayer_L_Hand.clientID = (uint)NetworkUpdateHandler.Instance.client_id;
        mainPlayer_R_Hand.clientID = (uint)NetworkUpdateHandler.Instance.client_id;

        //Setup Entity ID
        mainPlayer_head.entityID = (111 * 1000) + ((int)Entity_Type.main_Player * 100) + (1);
        mainPlayer_L_Hand.entityID = (111 * 1000) + ((int)Entity_Type.main_Player * 100) + (2);
        mainPlayer_R_Hand.entityID = (111 * 1000) + ((int)Entity_Type.main_Player * 100) + (3);

        yield return new WaitUntil(() => isURL_Loading_Finished);


        //obtain netregisterReferences
        for (int i = 0; i < listOfObjects.url_list.Count; i++)
            downloadedAssets_NetRegComponents.Add(allNetWork_GO_upload_list[i].GetComponent<Net_Register_GameObject>());


        if (transform.childCount > 0 && transform.GetChild(0).childCount > 0)
        {
            Color color = new Color(0, 0, 0, 0);
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = color;
            TurnOffFadeCanvas();
         }
        else
            Debug.LogWarning("There is no canvas element for the client.");

        _MAIN_UI_Interface.SetActive(true);

        //Invoke isTeacher/isStudent Events appropriately
        if (_mainClient_entityData.isTeacher)
            onClient_IsTeacher.Invoke();
        else
            onClient_IsStudent.Invoke();

        //wait for buttons to be setup and currentstate to be given by server 
     

        yield return new WaitUntil(() => _sceneButtonRegisterList.Count > 0 && currentSessionState != null);
        loadStatusUITEXT.text = "Loading Enviornment and UI";

        if (_sceneButtonRegisterList.Count == 0)
            Debug.LogWarning("No Environmental Scenes Added");

        //Get Current State but dont remove clients yet since we do not want to remove clients that we added during intiailization
        Refresh_CurrentState(false);

        //add ourselves
        AddNewClient(NetworkUpdateHandler.Instance.client_id, true);

          
        //process calls sent over before initializing unity for the client = -1 to remove client
        for (int i = 0; i < clientID_early_proces_list.Count; i++)
        {
            //decide to delete or add client
            if (Mathf.Sign(clientID_early_proces_list[i]) == 1)
                AddNewClient(clientID_early_proces_list[i]);
            //else //offset client ids back for correct deactivation;
            //    RemoveClient((clientID_early_proces_list[i] * -1) - 1);

            yield return null;
        }
        clientID_early_proces_list.Clear();

       

        //run early calls to uninitiated objects
        Position[] clientPos = new Position[clientRefresh_earlyProcess_Dict.Count];
        clientRefresh_earlyProcess_Dict.Values.CopyTo(clientPos, 0);

        for (int i = 0; i < clientRefresh_earlyProcess_Dict.Count; i++)
        {
            Client_Refresh(clientPos[i]);
               yield return null;
        }
        clientRefresh_earlyProcess_Dict.Clear();

        yield return new WaitUntil(() => {
            if (listOfObjects.url_list.Count > 0)
                return _assetButtonRegisterList.Count > 0;
            else
                return true;
        });

        isUIButtonsSetup_Finished = true;

        loadStatusUITEXT.text = "finishing synchronising session state";

        //process early interaction calls
        for (int i = 0; i < Interaction_Refresh_early_proces_list.Count; i++)
        {
            Interaction_Refresh(Interaction_Refresh_early_proces_list[i]);
            yield return null;
        }
        Interaction_Refresh_early_proces_list.Clear();

        //process early text calls
        for (int i = 0; i < text_Refresh_early_proces_list.Count; i++)
        {
            Text_Refresh_Process(text_Refresh_early_proces_list[i]);
            yield return null;
        }
        text_Refresh_early_proces_list.Clear();

        
        //Start receiving early calls to turn assets on and off 
        foreach (KeyValuePair<int, bool> asset in assetsToEnable_Dic)
        {
            NetworkCall_On_Select_Asset_Refence_Button(asset.Key, asset.Value);
            yield return null;
        }
        assetsToEnable_Dic.Clear();

        IsSyncStateSetup_Finished = true;


    
     
        //Notify Systems that we are fineshed loading
        NetworkUpdateHandler.Instance.On_Initiation_Loading_Finished();
        //AddNewClient(2);

        //AddNewClient(1);
        //RemoveClient(6);

        //AddNewClient(0);

        //AddNewClient(9);
        //RemoveClient(0);
        //     Draw_Refresh(new Dr

    }





    private int mainClienSpawntIndex = -1;

    private Dictionary<int, int> clientIDToAvatarIndex = new Dictionary<int, int>();
    private Dictionary<int, string> clientIDToName = new Dictionary<int, string>();

    //to connect funcion through the editor
    // public void UpdateClients(int clientID) => AddNewClient(clientID);

    public void AddNewClient(int clientID, bool isMainPlayer = false)
    {
        if (!isURL_Loading_Finished)
        {
            //allow main client to skip the waiting process and not add to early process list
            clientID_early_proces_list.Add(clientID);
            return;
        }

        //setup newclient
        if (!_client_ID_List.Contains(clientID))
        {
            _client_ID_List.Add(clientID);

            string nameLabel = default;
            //get name of client
            nameLabel = NetworkUpdateHandler.Instance.GetPlayerNameFromClientID(clientID);
            

            if (_client_ID_List.Count >= _clientReserveCount)
            {
                Debug.LogWarning("REACHED MAX CLIENTS IN GAME - ADD TO CLIENT RESERVE COUNT FOR MORE USERS");
                return;
            }

            //go through all available slots
            for (int i = 0; i < _clientReserveCount - 1; i++)
            {
                //skip avatars that are already on, and your reserved spot
                if (_availableGOList[i].activeInHierarchy || mainClienSpawntIndex == i)
                    continue;

                Non_MainClientData non_mainClientData = _availableGOList[i].GetComponentInChildren<Non_MainClientData>();
                non_mainClientData.Id = (uint)clientID;

            

                Debug.Log(clientID);

                if(!_availableClientIDToGODict.ContainsKey(clientID))
                _availableClientIDToGODict.Add(clientID, non_mainClientData);
                else
                    _availableClientIDToGODict[clientID] = non_mainClientData;

                if (!clientIDToAvatarIndex.ContainsKey(clientID))
                    clientIDToAvatarIndex.Add(clientID, i);
                else
                    clientIDToAvatarIndex[clientID] = i;

                if (!clientIDToName.ContainsKey(clientID))
                    clientIDToName.Add(clientID, nameLabel);
                else
                    clientIDToName[clientID] = nameLabel;


                //create main ui tag f
                clientTagSetup.CreateTextFromString(nameLabel);

                //select how to handle avatars
                if (!isMainPlayer)
                {
                    _availableGOList[i].SetActive(true);

                    //set text label
                    New_Text newText = new New_Text
                    {
                        stringType = (int)STRINGTYPE.CLIENT_NAME,
                        target = clientID,//clientIDToAvatarIndex[clientID],
                        text = nameLabel,
                    };

                    ClientSpawnManager.Instance.Text_Refresh_Process(newText);
                }
                else
                {
                    _availableGOList[i].SetActive(false);

                    mainClienSpawntIndex = i;


                    var temp = _availableClientIDToGODict[clientID].transform;
                    var ROT = _availableClientIDToGODict[clientID]._EntityContainer_MAIN.entity_data.rot;

#if !UNITY_EDITOR
                    //GameObject
                    _mainPlayer.transform.position = temp.position;
                    _mainPlayer.transform.rotation = new Quaternion(ROT.x, ROT.y, ROT.z, ROT.w);

                    //hands entity data
                    _mainClient_entityData.pos = temp.position;
                    _mainClient_entityData.rot = new Vector4(ROT.x, ROT.y, ROT.z, ROT.w);
                    _mainPlayer.transform.parent.GetChild(0).localPosition = temp.position;
                    _mainPlayer.transform.parent.GetChild(0).localRotation = new Quaternion(ROT.x, ROT.y, ROT.z, ROT.w);
#endif

                    //Turn Off Dummy 
                    var parObject = temp.parent.parent.gameObject;
                    parObject.name = "Main_Client";

                    isMainClientInitialized = true;

                }
                break;
            }



        }








    }
    public void RemoveClient(int clientID)
    {
        //for early network calls before initiation;
        if (!isURL_Loading_Finished)
        {
            //give remove clients an offset away from zero and then convert it back to 
            clientID_early_proces_list.Add((clientID * -1) - 1);
            return;
        }

        DestroyClient(clientID);
    }

    public async void DestroyClient(int clientID)
    {
        if (_client_ID_List.Contains(clientID))
        {
            //wait for setup to be complete for early calls
            while (!_availableClientIDToGODict.ContainsKey(clientID))
                await Task.Delay(1);



            clientTagSetup.DeleteTextFromString(clientIDToName[clientID]);
  
            _availableClientIDToGODict[clientID].transform.parent.gameObject.SetActive(false);
         //   _availableClientIDToGODict.Remove(clientID);

            _client_ID_List.Remove(clientID);

        }

    }

   

    public void TurnOffFadeCanvas() => transform.GetChild(0).gameObject.SetActive(false);

   

    #region Register Network Managed Objects
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nGO"></param>
    /// <param name="asseListIndex"> This is the url index in list</param>
    /// <param name="customEntityID"></param>
    public void LinkNewNetworkObject(GameObject nGO, int asseListIndex = -1, int customEntityID = 0)
    {


        Net_Register_GameObject tempNet = nGO.AddComponent<Net_Register_GameObject>();

        if (asseListIndex != -1)
            if (!decomposedAssetReferences_Dict.ContainsKey(asseListIndex))
            {
                List<Net_Register_GameObject> newNetLst = new List<Net_Register_GameObject>();
                newNetLst.Add(tempNet);
                decomposedAssetReferences_Dict.Add(asseListIndex, newNetLst);
               
            }
            else
            {
                List<Net_Register_GameObject> netList = decomposedAssetReferences_Dict[asseListIndex];
                netList.Add(tempNet);
                decomposedAssetReferences_Dict[asseListIndex] = netList;
            }


        //Setup NetworkComponent
        if (customEntityID != 0)
            tempNet.Instantiate(asseListIndex, customEntityID);
        else
            tempNet.Instantiate(asseListIndex);

  
       

    }/// <summary>
    /// Setup References For NetObjects 
    /// </summary>
    /// <param name="entityID"></param>
    /// <param name="nRG"></param>
    /// <returns></returns>
    public void RegisterNetWorkObject(int entityID, Net_Register_GameObject nRG)
    {
        _EntityID_To_NetObject.Add(entityID, nRG);
        allNetWork_GO_upload_list.Add(nRG.gameObject);
    }
    #endregion


   
    #region Dashboard Button Reference Setup
    public void On_Select_Asset_Refence_Button(int index, Button button, bool isNetworkCall)
    {
        GameObject currentObj = default;
        Net_Register_GameObject netRegisterComponent = default;

        currentObj = allNetWork_GO_upload_list[index];
       //GetComponentInChildren<Net_Register_GameObject>(true);

        //associate toggle button lock with asset isgrabbed flagg to prevent anyone from grabing it
        if (!_renderAssetFlag[index])
        {
            _renderAssetFlag[index] = true;

            if (button != null && button.image != null)
            {
                // FadeAlphaGraphicUI.CrossFadeAlphaFixed_Coroutine(button.image, 1, 0.1f, null);
                button.image.color = new Color(0, 0.5f, 0, 1);
                //button.image.CrossFadeAlphaFixed(1, 0.1f, null);
                var colors = button.colors;
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
                button.colors = colors;

              

                EventSystem.current.SetSelectedGameObject(button.gameObject);

            }


            if (isNetworkCall)
            {
                netRegisterComponent = currentObj.GetComponent<Net_Register_GameObject>();

                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = _mainClient_entityData.entityID,
                    targetEntity_id = netRegisterComponent.entity_data.entityID,//index,//netRegisterComponent.positionWithin_urlList,
                    interactionType = (int)INTERACTIONS.RENDERING,

                });
            //    MainClientUpdater.Instance.PlaceInNetworkUpdateList(netRegisterComponent);
            }
            // Debug.Log("RECEIVED INTERACTION :" + "entityID: " + netRegisterComponent.entity_data.entityID + "TYPE:" + (int)INTERACTIONS.RENDERING);
            //#endif
            currentObj.SetActive(true);
            
        }
        else
        {
            _renderAssetFlag[index] = false;

            if (button != null && button.image != null)
            {
                // FadeAlphaGraphicUI.CrossFadeAlphaFixed_Coroutine(button.image, 0, 0.1f, null);
                button.image.color = new Color(0, 0, 0, 0);
                //  button.image.CrossFadeAlphaFixed(0, 0.1f, null);
                var colors = button.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                button.colors = colors;

                button.OnSelect(new BaseEventData(EventSystem.current));
            }

            currentObj.SetActive(false);

            if (isNetworkCall)
            {
                netRegisterComponent = currentObj.GetComponent<Net_Register_GameObject>();

          //      MainClientUpdater.Instance.RemoveFromInNetworkUpdateList(netRegisterComponent);

                NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
                {
                    sourceEntity_id = _mainClient_entityData.entityID,
                    targetEntity_id = netRegisterComponent.entity_data.entityID,
                    interactionType = (int)INTERACTIONS.NOT_RENDERING,

                });
            }

        }
    }

    public void On_Select_Scene_Refence_Button(Scene_Reference sceneRef, Button button)
    {
        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
        {
            sourceEntity_id = _mainClient_entityData.entityID,
            targetEntity_id = sceneRef.sceneIndex,
            interactionType = (int)INTERACTIONS.CHANGE_SCENE,//cant covert since it gives 0 at times instead of the real type?

        });

        NETWORK_CALL_On_Select_Scene_Refence(sceneRef.sceneIndex);
    }
    #endregion

    #region External Call_Dashboard Button Reference Setup

   

    public void NETWORK_CALL_On_Select_Scene_Refence(int sceneID)
    {

        foreach (string scene in scene_Additives_Loaded)
            SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

     //   Resources.UnloadUnusedAssets();


        //clear the list
        scene_Additives_Loaded.Clear();

        //add the scene that is being loaded
        scene_Additives_Loaded.Add(sceneList._scene_Reference_List[sceneID].name);


        SceneManager.LoadSceneAsync(sceneList._scene_Reference_List[sceneID].name, LoadSceneMode.Additive);


        //enable previous scene button
        foreach (var button in _sceneButtonRegisterList)
            button.interactable = true;

        //disable current scene button to avoid re loading same scene again
        _sceneButtonRegisterList[sceneID].interactable = false;

        


    }

    Dictionary<int, bool> assetsToEnable_Dic = new Dictionary<int, bool>();

    public void NetworkCall_On_Select_Asset_Refence_Button(int entityID, bool activeState)
    {
        if(!isUIButtonsSetup_Finished)
        {
            if(!assetsToEnable_Dic.ContainsKey(entityID))
                assetsToEnable_Dic.Add(entityID, activeState);
            else
                assetsToEnable_Dic[entityID] = activeState;

            return;
        }

        GameObject currentObj = default;
        Net_Register_GameObject netRegisterComponent = default;

        if(!_EntityID_To_NetObject.ContainsKey(entityID))
        {
            Debug.LogError("EnitityID: " + entityID + ", Does not exist in entityID to UIButtonList_Index returning");
            return;
        }
        var urlListindex = _EntityID_To_NetObject[entityID].positionWithin_urlList;

        _renderAssetFlag[urlListindex] = activeState;
        
        currentObj = allNetWork_GO_upload_list[urlListindex];
        if(!currentObj)
        { Debug.LogWarning("Current object not found : " + urlListindex); }

        if (_EntityID_To_NetObject.ContainsKey(entityID))
        {
            netRegisterComponent = _EntityID_To_NetObject[entityID];//currentObj.GetComponent<Net_Register_GameObject>();
        }
        else
        {
            Debug.LogWarning("no netobject found entity ID : " + entityID);
            return;
        }
        Button button = _assetButtonRegisterList[urlListindex];
   
        if (!_renderAssetFlag[urlListindex])// (!activeState)//!allNetWork_GO_upload_list[index].activeInHierarchy)
        {
            _renderAssetFlag[urlListindex] = true;

            if (button != null && button.image != null)
            {
                //FadeAlphaGraphicUI.CrossFadeAlphaFixed_Coroutine(button.image, 1, 0.1f, null);
                button.image.color = new Color(0, 0.5f, 0, 1);
                //button.image.CrossFadeAlphaFixed(1, 0.1f, null);

                var colors = button.colors;
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
                button.colors = colors;

                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
            currentObj.SetActive(true);
        }
        else
        {
            _renderAssetFlag[urlListindex] = false;


            if (button != null && button.image != null)
            {
              //  FadeAlphaGraphicUI.CrossFadeAlphaFixed_Coroutine(button.image, 0, 0.1f, null);
                button.image.color = new Color(0, 0, 0, 0);
                // button.image.CrossFadeAlphaFixed(0, 0.1f, null);
                var colors = button.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                button.colors = colors;

                button.OnSelect(new BaseEventData(EventSystem.current));
            }

            currentObj.SetActive(false);

        }
    }
    #endregion




    #region Interaction Network Receiving Calls

    public GameObject BOX1;
    public GameObject BOX2;
    public void SETBOXACTIVENETWORKCALL(int boxnumber)
    {
        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.OPENBOX, targetEntity_id = boxnumber });
        //switch (boxnumber)
        //{
        //    case 1:
               
        //  //      BOX1.SetActive(true);
        //        break;
        //    case 2:
        //        BOX2.SetActive(true);
        //        break;

        //}

        
    }

    public void Interaction_Refresh(Interaction newData) // StartCoroutine(Interaction_Process(newData));
    {
        if (!isURL_Loading_Finished)
        {
            Interaction_Refresh_early_proces_list.Add(newData);
            return;
        }


        switch (newData.interactionType)
        {

            case (int)INTERACTIONS.RENDERING:

                NetworkCall_On_Select_Asset_Refence_Button(newData.targetEntity_id, false);

                break;

            case (int)INTERACTIONS.NOT_RENDERING:

                NetworkCall_On_Select_Asset_Refence_Button(newData.targetEntity_id, true);

                break;

            case (int)INTERACTIONS.GRAB:

                if (_EntityID_To_NetObject.ContainsKey(newData.targetEntity_id))
                    _EntityID_To_NetObject[newData.targetEntity_id].entity_data.isCurrentlyGrabbed = true;
                else
                    Debug.LogWarning("Client Entity does not exist for Grab interaction--- EntityID " + newData.targetEntity_id);

                break;

            case (int)INTERACTIONS.DROP:

                if (_EntityID_To_NetObject.ContainsKey(newData.targetEntity_id))
                {
                    _EntityID_To_NetObject[newData.targetEntity_id].entity_data.isCurrentlyGrabbed = false;
                }
                else
                    Debug.LogWarning("Client Entity does not exist for Drop interaction--- EntityID" + newData.targetEntity_id);


              
                //else
                //    _objectLoacalScale_Dic[newData.entityId] = newData.scaleFactor);


                break;

                
            case (int)INTERACTIONS.CHANGE_SCENE:

                //check the loading wait for changing into a new scene - to avoid loading multiple scenes
                StartCoroutine(Network_SceneChange(newData.targetEntity_id));

                break;

            case (int)INTERACTIONS.LOCK:

                foreach (Net_Register_GameObject item in decomposedAssetReferences_Dict[newData.sourceEntity_id])
                {
                    item.entity_data.isCurrentlyGrabbed = true;
                }
               // _EntityID_To_NetObject[newData.targetEntity_id].entity_data.isCurrentlyGrabbed = true;

                //disable button interaction for others
                if (newData.sourceEntity_id != -1)
                {
                    //remove disabling buttons for others
                    //_assetButtonRegisterList[newData.sourceEntity_id].interactable = false;
                    //_assetLockToggleRegisterList[newData.sourceEntity_id].interactable = false;
                    _assetLockToggleRegisterList[newData.sourceEntity_id].isOn = true;

                }
                break;
            case (int)INTERACTIONS.UNLOCK:

                foreach (Net_Register_GameObject item in decomposedAssetReferences_Dict[newData.sourceEntity_id])
                {
                    item.entity_data.isCurrentlyGrabbed = false;
                }
                //_EntityID_To_NetObject[newData.targetEntity_id].entity_data.isCurrentlyGrabbed = false;

                if (newData.sourceEntity_id != -1)
                {
                    //_assetButtonRegisterList[newData.sourceEntity_id].interactable = true;
                    //_assetLockToggleRegisterList[newData.sourceEntity_id].interactable = true;
                    _assetLockToggleRegisterList[newData.sourceEntity_id].isOn = false;

                }
                break;
            case (int)INTERACTIONS.START_VIDEO:

                videoPlayer.Play();
               // videoPlayer.time = newData.targetEntity_id;
                break;

            case (int)INTERACTIONS.STOP_VIDEO:
                videoPlayer.Pause();
                break;

            case (int)INTERACTIONS.TIMEREFRESH_VIDEO:

                videoStopPoints.SetTimeFromEvent(newData.targetEntity_id);
              //  videoPlayer.time = newData.targetEntity_id;
                //videoPlayer.Stop();
                break;

            case (int)INTERACTIONS.ADDPIENCE:
                AddToPieceCounter();
                break;
            case (int)INTERACTIONS.OPENBOX:

                switch (newData.targetEntity_id)
                {
                    case 1:
                        BOX1.SetActive(true);
                        break;

                        case 2:
                        BOX2.SetActive(true);
                        break;

                }
                break;

        }

    }
   

    public IEnumerator Network_SceneChange(int sID)
    {
        for (int i = 0; i < _sceneloading_asyncOperList.Count; i++)
        {
            yield return new WaitUntil(() => _sceneloading_asyncOperList[i].isDone);
            _sceneloading_asyncOperList.Remove(_sceneloading_asyncOperList[i]);
        }
        foreach (var loadWait in _sceneloading_asyncOperList)
        {
            yield return new WaitUntil(() => loadWait.isDone);
        }

        NETWORK_CALL_On_Select_Scene_Refence(sID);

    }
    #endregion
    //To avoid duplicating stroke ids because sending different ids states ma 
  //  private List<int> strokeIDFinished = new List<int>();
    public void Draw_Refresh(Draw newData)
    {
        LineRenderer currentLineRenderer = default;

        if (!allStrokeIDValidator.ContainsKey(newData.strokeId))
        {
            GameObject lineRendGO = new GameObject("LineR:" + newData.strokeId);
            lineRendGO.transform.SetParent(templateParentLineRenderer, true);
            currentLineRenderer = lineRendGO.AddComponent<LineRenderer>();
            currentLineRenderer.positionCount = 0;

            currentLineRenderer.useWorldSpace = false;
            currentLineRenderer.startWidth = templateLR.startWidth;
            currentLineRenderer.receiveShadows = false;
            currentLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            allStrokeIDValidator.Add(newData.strokeId, newData.strokeId);

            lineRenderersInQueue.Add(newData.strokeId, currentLineRenderer);
        }

        if (lineRenderersInQueue.ContainsKey(newData.strokeId))
        {
            currentLineRenderer = lineRenderersInQueue[newData.strokeId];
        }




        switch (newData.strokeType)
        {

            case (int)Entity_Type.Line:

                currentLineRenderer.sharedMaterial = templateLR.sharedMaterial;

                var brushColor = new Vector4(newData.curColor.x, newData.curColor.y, newData.curColor.z, newData.curColor.w);
                currentLineRenderer.startColor = brushColor;
                currentLineRenderer.endColor = brushColor;

                ++currentLineRenderer.positionCount;
                currentLineRenderer.receiveShadows = false;
                currentLineRenderer.allowOcclusionWhenDynamic = false;
                currentLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, newData.curStrokePos);

                break;

            case (int)Entity_Type.LineEnd:

                ++currentLineRenderer.positionCount;
                currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, newData.curStrokePos);

                LineRenderer lr = templateParentLineRenderer.GetComponent<LineRenderer>();

                //used to set correct pivot point when scalling object by grabbing
                GameObject pivot = new GameObject("LineRender:" + (newData.strokeId), typeof(BoxCollider));
                pivot.tag = "Drawing";
                // pivot.tag = "Interactable";

                if (!_EntityID_To_NetObject.ContainsKey(newData.strokeId))
                ClientSpawnManager.Instance.LinkNewNetworkObject(pivot, newData.strokeId, newData.strokeId);
                //set consistent entity id between users
                //  ClientSpawnManager.Instance.LinkNewNetworkObject(pivot, newData.entityId, newData.entityId);//ClientSpawnManager.Instance.allNetWork_GO_upload_list.Count);

                //var rb = pivot.GetComponent<Rigidbody>();
                //rb.isKinematic = true;
                //rb.useGravity = false;

                var bColl = pivot.GetComponent<BoxCollider>();

                Bounds newBounds = new Bounds(currentLineRenderer.GetPosition(0), Vector3.one * 0.01f);

                for (int i = 0; i < currentLineRenderer.positionCount; i++)
                    newBounds.Encapsulate(new Bounds(currentLineRenderer.GetPosition(i), Vector3.one * 0.01f));//lineRenderer.GetPosition(i));

                pivot.transform.position = newBounds.center;
                bColl.center = currentLineRenderer.transform.position;  //newBounds.center;//averageLoc / lr.positionCount;//lr.GetPosition(0)/2;
                bColl.size = newBounds.size;

                currentLineRenderer.transform.SetParent(pivot.transform, true);
                pivot.transform.SetParent(templateParentLineRenderer.transform);

                //if (lineRenderersInQueue.ContainsKey(newData.strokeId))
                //{
                //    lineRenderersInQueue.Remove(newData.strokeId);
                //}

                break;

            case (int)Entity_Type.LineDelete:


                if (_EntityID_To_NetObject.ContainsKey(newData.strokeId))
                {
                 //   Debug.LogError("rECEIVIG ERASE : " + newData.strokeId);

                    if (lineRenderersInQueue.ContainsKey(newData.strokeId))
                        lineRenderersInQueue.Remove(newData.strokeId);

                    
                    Destroy(_EntityID_To_NetObject[newData.strokeId].gameObject);
                    _EntityID_To_NetObject.Remove(newData.strokeId);
                }
                break;
        }

    }


    #region Client and Object NetworkReceivingCalls
    //FILTERING SYSTEM
    public void Client_Refresh(Position newData)
    {
     //   Debug.Log("new clientRefresh packet from : " + newData.clientId);
        if (!isURL_Loading_Finished)
        {
            //last transform update per entity made before initiation
            if (!clientRefresh_earlyProcess_Dict.ContainsKey(newData.entityId))
                clientRefresh_earlyProcess_Dict.Add(newData.entityId, newData);
            else
                clientRefresh_earlyProcess_Dict[newData.entityId] = newData;

            return;
        }

        // CLIENT_REFRESH_PROCESS(newData);
        if (!_client_ID_List.Contains(newData.clientId) && newData.entityType != (int)Entity_Type.objects && newData.entityType != (int)Entity_Type.physicsObject)
        {
            AddNewClient(newData.clientId);
            Debug.Log(newData.clientId + " : client ID is being registered through Client_Refresh");
        }

        //MOVE CLIENTS AND OBJECTS
        switch (newData.entityType)
        {
            //HEAD MOVE
            case (int)Entity_Type.users_head:
                //changed from local to world
                if (_availableClientIDToGODict.ContainsKey(newData.clientId))
                {
                    //to test out issue of client not rendering 
                   // _availableClientIDToGODict[newData.clientId].transform.parent.gameObject.SetActive(true);
                    //_availableGOList[i].SetActive(true);
                    _availableClientIDToGODict[newData.clientId]._EntityContainer_MAIN.transform.position = newData.pos;
                    _availableClientIDToGODict[newData.clientId]._EntityContainer_MAIN.transform.rotation = new Quaternion(newData.rot.x, newData.rot.y, newData.rot.z, newData.rot.w);
                }
                else
                    Debug.LogWarning("Client ID : " + newData.clientId + " not found in Dictionary dropping head movement packet");
                break;
            //HANDL MOVE
            case (int)Entity_Type.users_Lhand:

                if (_availableClientIDToGODict.ContainsKey(newData.clientId))
                {
                    Transform handTransRef = _availableClientIDToGODict[newData.clientId]._EntityContainer_hand_L.transform;

                    if (!handTransRef.gameObject.activeInHierarchy)
                        handTransRef.gameObject.SetActive(true);

                    handTransRef.position = newData.pos;
                    handTransRef.rotation = new Quaternion(newData.rot.x, newData.rot.y, newData.rot.z, newData.rot.w);
                }
                else
                    Debug.LogWarning("Client ID : " + newData.clientId + " not found in Dictionary dropping left hand movement packet");
                break;
            //HANDR MOVE
            case (int)Entity_Type.users_Rhand:
                if (_availableClientIDToGODict.ContainsKey(newData.clientId))
                {
                    Transform handTransRef = _availableClientIDToGODict[newData.clientId]._EntityContainer_hand_R.transform;

                    if (!handTransRef.gameObject.activeInHierarchy)
                        handTransRef.gameObject.SetActive(true);

                    handTransRef.position = newData.pos;
                    handTransRef.rotation = new Quaternion(newData.rot.x, newData.rot.y, newData.rot.z, newData.rot.w);


                    //_availableClientIDToGODict[newData.clientId]._EntityContainer_hand_R.transform.position = newData.pos;
                    //_availableClientIDToGODict[newData.clientId]._EntityContainer_hand_R.transform.rotation = new Quaternion(newData.rot.x, newData.rot.y, newData.rot.z, newData.rot.w);
                }
                else
                    Debug.LogWarning("Client ID : " + newData.clientId + " not found in Dictionary dropping right hand movement packet");
                break;
            //OBJECT MOVE
            case (int)Entity_Type.objects:

                if (_EntityID_To_NetObject.ContainsKey(newData.entityId))
                {
                    _EntityID_To_NetObject[newData.entityId].transform.position = newData.pos;
                    _EntityID_To_NetObject[newData.entityId].transform.rotation = newData.rot;
                    //_EntityID_To_NetObject[newData.entityId].transform.localScale = Vector3.one * newData.scaleFactor;
                    // _EntityID_To_NetObject[newData.entityId].transform.scalocalScale = Vector3.one * newData.scaleFactor;


                      UnityExtensionMethods.SetGlobalScale(_EntityID_To_NetObject[newData.entityId].transform, Vector3.one * newData.scaleFactor);

                    ////to provide correct scalling once object is dropped
                    //if (!_objectLoacalScale_Dic.ContainsKey(newData.entityId))
                    //    _objectLoacalScale_Dic.Add(newData.entityId, newData.scaleFactor);
                    //else
                    //    _objectLoacalScale_Dic[newData.entityId] = newData.scaleFactor;

                }
                else
                    Debug.LogWarning("Entity ID : " + newData.entityId + "not found in Dictionary dropping object movement packet");

                break;

            case (int)Entity_Type.physicsObject:

                //alternate kinematic to allow for sending non physics transform updates;
                if (_EntityID_To_NetObject.ContainsKey(newData.entityId))
                {
                    if (!_EntityID_To_RigidBody.ContainsKey(newData.entityId))
                    {
                        _EntityID_To_RigidBody.Add(newData.entityId, _EntityID_To_NetObject[newData.entityId].GetComponent<Rigidbody>());
                    }

                    var rb = _EntityID_To_RigidBody[newData.entityId];

                    if(!rb)
                    {
                        Debug.LogError("There is no rigidbody in netobject entity id: " + newData.entityId);
                        return;
                    }


                    rb.isKinematic = true;
                    _EntityID_To_NetObject[newData.entityId].transform.position = newData.pos;
                    _EntityID_To_NetObject[newData.entityId].transform.rotation = newData.rot;
                    UnityExtensionMethods.SetGlobalScale(_EntityID_To_NetObject[newData.entityId].transform, Vector3.one * newData.scaleFactor);



                }
                else
                    Debug.LogWarning("Entity ID : " + newData.entityId + "not found in Dictionary dropping physics object movement packet");

                break;

                //SET PHYSICS BACK UP
            case 33:

                //alternate kinematic to allow for sending non physics transform updates;
                if (_EntityID_To_NetObject.ContainsKey(newData.entityId))
                {

                    //skip opperation if current object is grabbed to avoid turning physics back on
                    if (_EntityID_To_NetObject[newData.entityId].entity_data.isCurrentlyGrabbed)
                        return;


                    if (!_EntityID_To_RigidBody.ContainsKey(newData.entityId))
                    {
                        _EntityID_To_RigidBody.Add(newData.entityId, _EntityID_To_NetObject[newData.entityId].GetComponent<Rigidbody>());
                    }

                    var rb = _EntityID_To_RigidBody[newData.entityId];

                    if (!rb)
                    {
                        Debug.LogError("There is no rigidbody in netobject entity id: " + newData.entityId);
                        return;
                    }

                    rb = _EntityID_To_NetObject[newData.entityId].GetComponent<Rigidbody>();

                    rb.isKinematic = false;

                }
                else
                    Debug.LogWarning("Entity ID : " + newData.entityId + "not found in Dictionary dropping physics object movement packet");

                break;
        }
    }

    public Dictionary<int,float> _objectLoacalScale_Dic = new Dictionary<int, float>();
 

    //Setting up Line Rendering Calls

    public Dictionary<int, LineRenderer> lineRenderersInQueue = new Dictionary<int, LineRenderer>();
    public Dictionary<int, int> allStrokeIDValidator = new Dictionary<int, int>();



    #endregion

    #region SessionState Update Calls


    public void Refresh_CurrentState(bool removeClients)
    {
     
        NETWORK_CALL_On_Select_Scene_Refence(currentSessionState.scene);

        //Gather list of clients to remove by comparing current list with currentstate list
        List<int> cIdsToRemove = new List<int>();
        foreach (var clientID in _client_ID_List)
        {
            bool isClientPresent = false;
            foreach (var curClientIDs in currentSessionState.clients)
            {
                if (clientID == curClientIDs)
                {
                    isClientPresent = true;
                    break;
                }
            }

            if (!isClientPresent)
                cIdsToRemove.Add(clientID);
        }

        //remove clients
        //if (removeClients)
        //{
        //    foreach (int cR in cIdsToRemove)
        //        RemoveClient(cR);
        //}

        //add clients
        foreach (var clientID in currentSessionState.clients)
        {
            if (clientID != NetworkUpdateHandler.Instance.client_id)
                AddNewClient(clientID);
        }

        List<int> entitiesToEnable = new List<int>();
        List<int> locksToEnable = new List<int>();
        foreach (var downloadedGO_NetReg in downloadedAssets_NetRegComponents)
        {
            var isAssetOn = false;
            var isLockOn = false;

            foreach (var entity in currentSessionState.entities)
            {

                if (entity.id == downloadedGO_NetReg.entity_data.entityID)
                {
                    isAssetOn = true;
                    break;
                }
            }

            if (isAssetOn)
                NetworkCall_On_Select_Asset_Refence_Button(downloadedGO_NetReg.entity_data.entityID, false);
            else
                NetworkCall_On_Select_Asset_Refence_Button(downloadedGO_NetReg.entity_data.entityID, true);


            foreach (var entity in currentSessionState.entities)
            {
                
                if (entity.id == downloadedGO_NetReg.entity_data.entityID) // NOTE(rob): shouldn't this be checking entity.locked? 
                {
                         isLockOn = true;

                    break;
                }
            }

            if (isLockOn)
                Interaction_Refresh(new Interaction(sourceEntity_id: _EntityID_To_NetObject[downloadedGO_NetReg.entity_data.entityID].positionWithin_urlList, targetEntity_id: downloadedGO_NetReg.entity_data.entityID, interactionType: (int)INTERACTIONS.LOCK));
            else
                Interaction_Refresh(new Interaction(sourceEntity_id: _EntityID_To_NetObject[downloadedGO_NetReg.entity_data.entityID].positionWithin_urlList, targetEntity_id: downloadedGO_NetReg.entity_data.entityID, interactionType: (int)INTERACTIONS.UNLOCK));

        }



    }

    [System.Serializable]
    public struct EntityState
    {
        public int id;
        public float[] latest;
        public bool render;
        public bool locked;
    }

    [System.Serializable]
    public class SessionState
    {
        public int[] clients;
        public EntityState[] entities;
        public int scene;
        public bool isRecording;

    }

    public void SyncSessionState(string stateString)
    {
        var stateStruct = JsonUtility.FromJson<SessionState>(stateString);

        currentSessionState = stateStruct;

        //only update when things are setup if not keep reference in current session state class.
        if (IsSyncStateSetup_Finished)
        {
            Refresh_CurrentState(true);
        }
       
    }
   

    #endregion
    #region String Update Calls

   

    public struct SpeechToText
    {
        public int session_id;
        public int client_id;
        public string text;
        public string type;
        public int ts;
    }



    public void Text_Refresh(String data)
    {
        var deserializedData = JsonUtility.FromJson<SpeechToText>(data);
        New_Text newStt;
        newStt.target = deserializedData.client_id;
        newStt.text = deserializedData.text;
        newStt.stringType = (int)STRINGTYPE.DIALOGUE;
        Text_Refresh_Process(newStt);
    }
    public void Text_Refresh_Process(New_Text newText)
    {
        if (!isURL_Loading_Finished)
        {
            text_Refresh_early_proces_list.Add(newText);
            return;
        }

        
        if (!_client_ID_List.Contains(newText.target))
        {
            Debug.LogWarning("No client Found, Can't render text");
            return;
        }

        switch (newText.stringType)
        {
            case (int)STRINGTYPE.TUTORIAL:
                break;

            case (int)STRINGTYPE.DIALOGUE:

                //Get client index for text look up to use for displaying

              var  clientIndex = clientIDToAvatarIndex[newText.target];
                //   var clientIndex = _client_ID_List.IndexOf(newText.target);

                string foo = SplitWordsByLength(newText.text, 20);

                StartCoroutine(SetTextTimer(clientIndex, foo, 0.9f * Mathf.Log(newText.text.Length)));


                break;

            case (int)STRINGTYPE.CLIENT_NAME:


                clientIndex = clientIDToAvatarIndex[newText.target]; // _client_ID_List.IndexOf(newText.target);
          //      Debug.Log("CI " + clientIndex + "CID " + newText.target);
                _clientUser_Names_UITextReference[clientIndex].text = newText.text;
              //  _clientUser_Names_UITextReference[newText.target].text = newText.text;
                break;

        }

    }




    private static string SplitWordsByLength(string str, int maxLength)
    {
        List<string> chunks = new List<string>();
        while (str.Length > 0)
        {
            if (str.Length <= maxLength)                    //if remaining string is less than length, add to list and break out of loop
            {
                chunks.Add(str);
                break;
            }

            string chunk = str.Substring(0, maxLength);     //Get maxLength chunk from string.

            if (char.IsWhiteSpace(str[maxLength]))          //if next char is a space, we can use the whole chunk and remove the space for the next line
            {
                chunks.Add(chunk + "\n");
                str = str.Substring(chunk.Length + 1);      //Remove chunk plus space from original string
            }
            else
            {
                int splitIndex = chunk.LastIndexOf(' ');    //Find last space in chunk.
                if (splitIndex != -1)                       //If space exists in string,
                    chunk = chunk.Substring(0, splitIndex); //  remove chars after space.
                str = str.Substring(chunk.Length + (splitIndex == -1 ? 0 : 1));      //Remove chunk plus space (if found) from original string
                chunks.Add(chunk + "\n");                          //Add to list
            }
        }
        return string.Concat(chunks);
        // return chunks;
    }



    //  private List<ValueTuple> messageInput = new List<ValueTuple>();

   


    public IEnumerator SetTextTimer(int index, string textD, float seconds = 5)
    {
        if (!secondsToWaitDic.ContainsKey(index))
            secondsToWaitDic.Add(index, seconds);
        else
            secondsToWaitDic[index] += seconds;

        if (!_currentTextProcessingList.Contains(index))
        {
            _clientUser_Dialogue_UITextReference[index].text = textD;
            _currentTextProcessingList.Add(index);
            StartCoroutine(ShutOFFText(index, seconds));

        }
        else

        {
            _currentTextProcessingList.Add(index);
            //  _clientUser_Dialogue_UITextReference[index].transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(secondsToWaitDic[index]);

            secondsToWaitDic[index] -= seconds;


            StartCoroutine(ShutOFFText(index, seconds));
            //   _currentTextProcessingList_Coroutine.Add(StartCoroutine(ShutOFFText(index, seconds)));

            _clientUser_Dialogue_UITextReference[index].text = textD;



        }
        yield return null;

    }

    public IEnumerator ShutOFFText(int index, float seconds)
    {

        _clientUser_Dialogue_UITextReference[index].transform.parent.gameObject.SetActive(true);

        //  secondsToWaitDic[index] -= seconds;
        yield return new WaitForSeconds(seconds);

        _currentTextProcessingList.Remove(index);

        if (!_currentTextProcessingList.Contains(index))
        {
            _clientUser_Dialogue_UITextReference[index].transform.parent.gameObject.SetActive(false);
        }



    }


    #endregion

    #region Slicing Calls For Network Export
    public void TESTINGSlicing(int entityData, Vector3 bladeDir, Vector3 moveDir, Vector3 knifeOrigin)//public void TESTINGSlicing(int entityData, Quaternion rotation, Vector3 pos, Vector3 dir, Vector3 origin)
    {

        if (_EntityID_To_NetObject.ContainsKey(entityData))
        {
            //    Debug.Log("NotGoing");
            KnifeSliceableAsync kSaSync = _EntityID_To_NetObject[entityData].GetComponent<KnifeSliceableAsync>();
            kSaSync.PropagatedSlice(bladeDir, moveDir, knifeOrigin);

        }
        else
        {

            StartCoroutine(WaitForSlicingObject(entityData, bladeDir, moveDir, knifeOrigin));
            return;
        }

    }
    public IEnumerator WaitForSlicingObject(int entityData, Vector3 bladeDir, Vector3 moveDir, Vector3 knifeOrigin)
    {

        while (true)
        {
            if (!_EntityID_To_NetObject.ContainsKey(entityData))
            {
                yield return null;
                Debug.Log("OBJECT DOES NOT EXIST");
            }
            else
            {
                yield return new WaitForSeconds(3);
                //  yield return new WaitUntil(() =>  _EntityID_To_NetObject[entityData].GetComponent<Rigidbody>());

                KnifeSliceableAsync kSaSync = _EntityID_To_NetObject[entityData].GetComponent<KnifeSliceableAsync>();
                kSaSync.PropagatedSlice(bladeDir, moveDir, knifeOrigin);
                yield break;

            }
        }

    }
    #endregion

    #region Setup Client Spots
    public IEnumerator Instantiate_Reserved_Clients()
    {

        float degrees = 360f / _clientReserveCount;
        Vector3 offset = new Vector3(0, 0, 4);

        GameObject instantiation = default;

        //Create all players with simple GameObject Representation
        for (int i = 0; i < _clientReserveCount; i++)
        {

            Vector3 TransformRelative = Quaternion.Euler(0f, degrees * i + 1, 0f) * (transform.position + offset);
            var gameObjectParentRelative = new GameObject($"Client_{i + 1}");

            instantiation = Instantiate(_otherPlayersDummyPrefab, Vector3.zero, Quaternion.identity, gameObjectParentRelative.transform);

            //Obtain each avatars UI_TEXT REFERENCE FROM THEIR CANVAS IN PREFAB
            //GET HEAD TO GET CANVAS FOR TEXT COMPONENTS
            Transform canvas = instantiation.transform.GetChild(0).GetComponentInChildren<Canvas>().transform;

            //GET APPROPRIATE TEXT COMPONENT CHARACTER NAME AND DIALOGUE
            _clientUser_Names_UITextReference.Add(canvas.GetChild(0).GetComponent<Text>());
            canvas.GetChild(0).GetComponent<Text>().text = $"Client_{i + 1}";

            _clientUser_Dialogue_UITextReference.Add(canvas.GetChild(1).GetChild(0).GetComponent<Text>());

            //Set up links for network call references
            var non_mainClientData = instantiation.GetComponentInChildren<Non_MainClientData>(true);

            non_mainClientData._EntityContainer_hand_L.entity_data = ScriptableObject.CreateInstance<EntityData_SO>();
            non_mainClientData._EntityContainer_hand_R.entity_data = ScriptableObject.CreateInstance<EntityData_SO>();
            var clientData_Main = non_mainClientData._EntityContainer_MAIN.entity_data = ScriptableObject.CreateInstance<EntityData_SO>();

            gameObjectParentRelative.transform.position = TransformRelative;

            //Same orientation
            TransformRelative.y = _CenterToSpawnClients.transform.position.y;

            Quaternion newRot = Quaternion.LookRotation(_CenterToSpawnClients.transform.position - TransformRelative, Vector3.up);

            clientData_Main.rot = new Vector4(newRot.x, newRot.y, newRot.z, newRot.w);

            non_mainClientData.transform.parent.localRotation = new Quaternion(newRot.x, newRot.y, newRot.z, newRot.w);

            _availableGOList.Add(instantiation);

            instantiation.SetActive(false);

        }

        isClientAvatarSetup_Finished = true;
        yield return null;
    }

    private Vector3 GetSlotLocation(int slot)
    {

        Vector3 location = transform.position;

        float degrees = 360f / _clientReserveCount + 1;
        // degrees = 30;
        degrees *= slot;

        //   location.y = -1f;

        location.x = Mathf.Cos(Mathf.Deg2Rad * degrees);
        location.x *= _spreadRadius;
        location.z = Mathf.Cos(Mathf.Deg2Rad * degrees);
        location.z *= _spreadRadius;
        return location;
    }

    #endregion

}
