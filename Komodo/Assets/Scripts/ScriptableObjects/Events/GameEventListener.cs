// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

namespace RoboRyanTron.Unite2017.Events
{
	[System.Serializable]
    public class unityEvent_DialogueArg : UnityEvent<Dialogue_Change> { }
    [System.Serializable]
    public class unityEvent_string : UnityEvent<string> { }

    [System.Serializable]
    public class unityEvent_int : UnityEvent<int> { }

    [System.Serializable]
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public GameEvent Event;


        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        public unityEvent_DialogueArg ResponseWithDialogueToExport;
        
        public unityEvent_string url_To_Export;

        public unityEvent_int levelSet_to_Export;
		//public Vector3UnityEvent unityEvent;
        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }

        public void OnDialogue_EventRaised(Dialogue_Change dc)
        {
            ResponseWithDialogueToExport.Invoke(dc);
        }

        public void On_URL_EventRaised(string url)
        {
            url_To_Export.Invoke(url);
        }
        public void On_Level_EventRaise(int level)
        {
            levelSet_to_Export.Invoke(level);
        }

    }
}