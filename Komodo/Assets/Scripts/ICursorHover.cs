using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICursorHover : IEventSystemHandler
{
    void OnHover(CursorHoverEventData cursorData);
}

public class CursorHoverEventData : BaseEventData
{
    // This doesn't have to be in this scope, but I didn't want to create
    // yet another script file for it, so this seemed the most logical place
    public static readonly ExecuteEvents.EventFunction<ICursorHover> cursorFollowDelegate
    = delegate (ICursorHover handler, BaseEventData data)
    {
        var casted = ExecuteEvents.ValidateEventData<CursorHoverEventData>(data);
        handler.OnHover(casted);
    };

    public Vector3 currentHitLocation;
   // public float damageAmount;

    public CursorHoverEventData(EventSystem eventSystem,
                           Vector3 currentHitLocation
                           ) : base(eventSystem)
    {
        this.currentHitLocation = currentHitLocation;
    }
}