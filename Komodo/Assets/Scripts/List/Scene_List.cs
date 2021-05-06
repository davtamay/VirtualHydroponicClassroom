using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor;

[System.Serializable]
public struct Scene_Reference
{
    public string name;
    public int sceneIndex;
}

[CreateAssetMenu(fileName = "Scene_List", menuName = "new_Scene_List", order = 0)]
public class Scene_List : ScriptableObject
{
    [Header("Add scenes to show in game to this list")]
    public List<Object> scene_list;

    [Tooltip("Adding scenes to the above list updates the list shown in game, according to the below field")]
    public List<Scene_Reference> _scene_Reference_List;

    //check if the user change the asset to change available scens displayed
    public void OnValidate()
    {
        _scene_Reference_List.Clear();

        int currentScene = 0;

        foreach (var item in scene_list)
        {
            _scene_Reference_List.Add(new Scene_Reference
            {
                name = item.name,
                sceneIndex = currentScene,
            });

            ++currentScene;
        }
    }
}