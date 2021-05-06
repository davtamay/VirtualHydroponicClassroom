using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetUp_ButtonURLs : MonoBehaviour
{
    public GameObject _buttonTemplate;
    

    public Transform transformToPlaceButtonUnder;

    public bool isURLButtonList = true;
    public String_List URL_List;

    public bool isEnvironmentButtonList = false;
    public Scene_List _sceneList;

    List<Button> allButtons = new List<Button>();

    public void Start()
    {

        Init();
        
    }
    public async void Init()
    {
        while (!ClientSpawnManager.Instance.isURL_Loading_Finished)
          await  Task.Delay(1);
     //   yield return new WaitUntil(() => ClientSpawnManager.Instance.isURL_LoadingFinished);

        if (isURLButtonList)
        {
            if (!transformToPlaceButtonUnder)
                transformToPlaceButtonUnder = transform;

            List<GameObject> buttonLinks = new List<GameObject>();

            for (int i = 0; i < URL_List.url_list.Count; i++)
            {
                GameObject temp = Instantiate(_buttonTemplate, transformToPlaceButtonUnder);

                Button tempButton = temp.GetComponentInChildren<Button>(true);
                ClientSpawnManager.Instance._assetButtonRegisterList.Add(tempButton);
                ClientSpawnManager.Instance._renderAssetFlag.Add(false);

                Toggle tempLockToggle = temp.GetComponentInChildren<Toggle>();

                ClientSpawnManager.Instance._assetLockToggleRegisterList.Add(tempLockToggle);


               SetButtonDelegateURL(tempButton, i, tempLockToggle);
                Text tempText = temp.GetComponentInChildren<Text>(true);
                tempText.text = URL_List.url_list[i].name;

                buttonLinks.Add(temp);
            }

     
           
        }
        else if(isEnvironmentButtonList){

            if (!transformToPlaceButtonUnder)
                transformToPlaceButtonUnder = transform;

            List<GameObject> buttonLinks = new List<GameObject>();

            for (int i = 0; i < _sceneList._scene_Reference_List.Count; i++)
            {
                GameObject temp = Instantiate(_buttonTemplate, transformToPlaceButtonUnder);

                Button tempButton = temp.GetComponentInChildren<Button>(true);

                ClientSpawnManager.Instance._sceneButtonRegisterList.Add(tempButton);

                SetButtonDelegate_Scene(tempButton, _sceneList._scene_Reference_List[i]);
                Text tempText = temp.GetComponentInChildren<Text>(true);

                tempText.text = _sceneList._scene_Reference_List[i].name;// scene_list[i].name;//scenes[i].name;

                buttonLinks.Add(temp);
                allButtons.Add(tempButton);

            }
        }
    }
    public void SetButtonDelegateURL(Button button, int index, Toggle toggleLock)
    {
      
        //setup lock mechanism
        toggleLock.isOn = false;
        toggleLock.onValueChanged.AddListener((bool lockState) => { CallBackOnAssetLockSelect(lockState, index); }
        );

        //setup asset spawning mechanism
        button.onClick.AddListener(delegate {
            ClientSpawnManager.Instance.On_Select_Asset_Refence_Button(index, button, true);
        });
    }

    public void CallBackOnAssetLockSelect(bool currentLockStatus, int index)
    {
        foreach (Net_Register_GameObject item in ClientSpawnManager.Instance.decomposedAssetReferences_Dict[index])
        {
            item.entity_data.isCurrentlyGrabbed = currentLockStatus;
        }

       //var tempGO = ClientSpawnManager.Instance.allNetWork_GO_upload_list[index];
       //var netRegisterComponent = tempGO.GetComponent<Net_Register_GameObject>();
       // netRegisterComponent.entity_data.isCurrentlyGrabbed = currentLockStatus;

        int lockState = 0;

        //SETUP and send network lockstate
        if (currentLockStatus)
        {
            lockState = (int)INTERACTIONS.LOCK;
        }
        else
        {
            lockState = (int)INTERACTIONS.UNLOCK;
        }

        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction
        {
            sourceEntity_id = index,//_mainClient_entityData.entityID,
            targetEntity_id = ClientSpawnManager.Instance.decomposedAssetReferences_Dict[index][0].entity_data.entityID,//netRegisterComponent.entity_data.entityID,
            interactionType = lockState,//6,//(int)INTERACTIONS.CHANGE_SCENE,//cant covert since it gives 0 at times instead of the real type

        });

       // ClientSpawnManager.Instance

    }

    public void SetButtonDelegate_Scene(Button button, Scene_Reference sceneRef)
    {
       

        button.onClick.AddListener(delegate {
            foreach (Button but in allButtons)
            {
                but.interactable = true;
            };
        });

        button.onClick.AddListener(delegate {
            ClientSpawnManager.Instance.On_Select_Scene_Refence_Button(sceneRef, button);
        });

      
    }

}
