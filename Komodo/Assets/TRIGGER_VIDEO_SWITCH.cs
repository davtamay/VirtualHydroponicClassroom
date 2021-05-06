using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TRIGGER_VIDEO_SWITCH : MonoBehaviour
{
    public VideoPlayer currentVP;
    
    void Start()
    {
        currentVP = GetComponent<VideoPlayer>();
    }
    public void SwitchVideo(string url)
    {
        StartCoroutine(SwitchVideoEnum(url));
    }
    public IEnumerator SwitchVideoEnum(string url)
    {

        currentVP.url = url;
        currentVP.Prepare();
        yield return new WaitUntil(() => currentVP.isPrepared);
        currentVP.Play();

    }
    public void StopVideo()
    {
        currentVP.Stop();

    }

 
}
