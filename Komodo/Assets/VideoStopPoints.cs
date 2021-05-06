using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[System.Serializable]
public class StopPoints
{
    public float timeToStop;
    public UnityEvent invokeOnStopPoint;

}
public class VideoStopPoints : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public List<StopPoints> stopPointList;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        foreach (var item in stopPointList) {

            yield return new WaitUntil(() => videoPlayer.time > item.timeToStop);
                item.invokeOnStopPoint.Invoke();

            NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.STOP_VIDEO, targetEntity_id = (int)videoPlayer.time });
            NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.TIMEREFRESH_VIDEO, targetEntity_id = stopPointList.IndexOf(item) });


        }
    }
    public void PlayToAll()
    {
        videoPlayer.Play();
        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.START_VIDEO, targetEntity_id = (int)videoPlayer.time });
    }
    public void StopToAll()
    {
        videoPlayer.Pause();
        NetworkUpdateHandler.Instance.InteractionUpdate(new Interaction { interactionType = (int)INTERACTIONS.STOP_VIDEO, targetEntity_id = (int)videoPlayer.time });
    }

    public void SetTimeFromEvent(int i)
    {
        float time = stopPointList[i].timeToStop;
        videoPlayer.time = time;

        Debug.LogError(videoPlayer.time); //  time);

    }
    // Update is called once per frame

}
