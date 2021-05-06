using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using WebXR;

public class TeleportPlayer : MonoBehaviour
{
    //data container to offset hand movement for Webxr controller movement update 
    public EntityData_SO clientData;

    public Transform player;
    public Transform xrPlayer;
    public Transform handsParent;

    public void Awake()
    {
        if (!player) {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        //Get xr player to change position
        if (!xrPlayer) {
            xrPlayer = GameObject.FindGameObjectWithTag("XRCamera").transform;
        }
        //Get xr player to change position
        if (!handsParent) {
            handsParent = GameObject.FindGameObjectWithTag("Hands").transform;
        }
    }

    public void UpdateHandsParentPosition() {
        handsParent.SetPositionAndRotation(player.position, player.rotation);
    }

    //used in desktop
    public void UpdatePlayerPositionWithoutHeight(Position newData)
    {
        xrPlayer.position = newData.pos;
        xrPlayer.localRotation = newData.rot;
    }
    public void UpdatePlayerRotation(Position newData)
    {
     //   xrplayer.position = newData.pos;
        xrPlayer.localRotation = newData.rot;

#if UNITY_EDITOR
        player.localRotation = newData.rot;
#endif
    }

    //used in vr
    public void UpdatePlayerPosition(Position newData)
    {
        //used in VR
        var finalPosition = newData.pos;
        finalPosition.y = newData.pos.y + WebXR.WebXRManager.Instance.DefaultHeight;

#if UNITY_EDITOR

        player.position = finalPosition;

#elif UNITY_WEBGL

        xrPlayer.position = finalPosition;
        
#endif

        clientData.pos = finalPosition;
    }

    public void UpdatePlayerHeight(float newHeight)
    {
        WebXR.WebXRManager.Instance.DefaultHeight = newHeight;
        //used in VR

        var finalPosition = Vector3.zero;
#if UNITY_EDITOR
        finalPosition = player.position;
        finalPosition.y = WebXR.WebXRManager.Instance.DefaultHeight;

        player.position = finalPosition;


#elif UNITY_WEBGL
        finalPosition = xrPlayer.position;
        finalPosition.y = WebXR.WebXRManager.Instance.DefaultHeight;

        xrPlayer.position = finalPosition;
        
#endif

        clientData.pos = finalPosition;
    }

}
