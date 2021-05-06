using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : SingletonComponent<Scene_Manager>
{
    public static Scene_Manager Instance
    {
        get { return ((Scene_Manager)_Instance); }
        set { _Instance = value; }
    }

    public void LoadSceneAdditiveAsync(string scene, bool mergeScenes = false)
    {

       AsyncOperation aSync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        if (mergeScenes)
            StartCoroutine(MergeScenes(aSync, SceneManager.GetSceneByName(scene), SceneManager.GetActiveScene()));
            
    }
    public void LoadSceneAdditiveAsync(Scene scene, bool mergeScenes = false)
    {

        AsyncOperation aSync = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);

        if (mergeScenes)
            StartCoroutine(MergeScenes(aSync, SceneManager.GetSceneByName(scene.name), SceneManager.GetActiveScene()));

    }
    public IEnumerator MergeScenes(AsyncOperation async, Scene source, Scene target)
    {
        yield return new WaitUntil(() => async.isDone);
        SceneManager.MergeScenes(source, target);

    }
}
