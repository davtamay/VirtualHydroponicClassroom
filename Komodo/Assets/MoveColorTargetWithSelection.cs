using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveColorTargetWithSelection : MonoBehaviour, ICursorHover
{
    public Transform target;

    public void OnHover(CursorHoverEventData cursorData)
    {
        target.transform.position = cursorData.currentHitLocation;
    }
}
