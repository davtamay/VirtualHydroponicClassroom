using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.EventSystems
{
    //3 things taken off to allow for indivodial camera raycasting to work, isfocused, isuppressedinthisfram, onclick removed
    [AddComponentMenu("Event/Standalone Input Module")]
    /// <summary>
    /// A BaseInputModule designed for mouse / keyboard / controller input.
    /// </summary>
    /// <remarks>
    /// Input module for working with, mouse, keyboard, or controller.
    /// </remarks>
    public class Standalone_InputModule_Modifie_IndCameraRays : PointerInputModule
    {
        private float m_PrevActionTime;
        private Vector2 m_LastMoveVector;
        private int m_ConsecutiveMoveCount = 0;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        private GameObject m_CurrentFocusedGameObject;

        private PointerEventData m_InputPointerEvent;


        //use this to fire click process for lazer
        public void SetTriggerForClick()
        {
            if (m_CurrentFocusedGameObject == null)
                return;

            var leftButtonData = GetMousePointerEventData(0).GetButtonState(PointerEventData.InputButton.Left).eventData;

            var pointerEvent = leftButtonData.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;


            var newPressed = ExecuteEvents.ExecuteHierarchy(m_CurrentFocusedGameObject, pointerEvent, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            ExecuteEvents.Execute(newPressed, leftButtonData.buttonData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, leftButtonData.buttonData, ExecuteEvents.pointerDownHandler);


            // m_isClickTrigger = false;

            // m_isClickTrigger = true;

            ////      m_isClickTrigger = true;

            ////  {
            //var mouseData = GetMousePointerEventData(0);
            //var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            //var pointerEvent = leftButtonData.buttonData;

            //pointerEvent.eligibleForClick = true;
            //pointerEvent.delta = Vector2.zero;
            //pointerEvent.dragging = false;
            //pointerEvent.useDragThreshold = true;
            //pointerEvent.pressPosition = pointerEvent.position;
            //pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;




            //ExecuteEvents.Execute(m_CurrentFocusedGameObject, pointerEvent, ExecuteEvents.pointerClickHandler);

            //DeselectIfSelectionChanged(m_CurrentFocusedGameObject, pointerEvent);

        }



        protected Standalone_InputModule_Modifie_IndCameraRays()
        {
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public enum InputMode
        {
            Mouse,
            Buttons
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public InputMode inputMode
        {
            get { return InputMode.Mouse; }
        }

        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        private string m_VerticalAxis = "Vertical";

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_SubmitButton = "Submit";

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_CancelButton = "Cancel";

        [SerializeField]
        private float m_InputActionsPerSecond = 10;

        [SerializeField]
        private float m_RepeatDelay = 0.5f;

        [SerializeField]
        [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        private bool m_ForceModuleActive;


        [Header("Camera Event RayCast Custom Made Variables")]
        [Space]
        //FIXME//NEW TRIGGER FOR CLICK

        //  [SerializeField]
        public LineRenderer current_LineRenderer;
        public Transform curLineRender_Trans;


        public bool isCurrentLineRendererOn;

        [SerializeField]
        private float lengthOfDefaultLine = 1.5f;

        public bool m_isClickTrigger;




        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnMobileDevice
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Force this module to be active.
        /// </summary>
        /// <remarks>
        /// If there is no module active with higher priority (ordered in the inspector) this module will be forced active even if valid enabling conditions are not met.
        /// </remarks>
        public bool forceModuleActive
        {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Number of keyboard / controller inputs allowed per second.
        /// </summary>
        public float inputActionsPerSecond
        {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Delay in seconds before the input actions per second repeat rate takes effect.
        /// </summary>
        /// <remarks>
        /// If the same direction is sustained, the inputActionsPerSecond property can be used to control the rate at which events are fired. However, it can be desirable that the first repetition is delayed, so the user doesn't get repeated actions by accident.
        /// </remarks>
        public float repeatDelay
        {
            get { return m_RepeatDelay; }
            set { m_RepeatDelay = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis
        {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis
        {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        /// <summary>
        /// Maximum number of input events handled per second.
        /// </summary>
        public string submitButton
        {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        /// <summary>
        /// Input manager name for the 'cancel' button.
        /// </summary>
        public string cancelButton
        {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }

        private bool ShouldIgnoreEventsOnNoFocus()
        {
            //return true;
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected)
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }

        //public override void UpdateModule()
        //{

        //    if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
        //    {
        //        if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag != null && m_InputPointerEvent.dragging)
        //        {
        //            ReleaseMouse(m_InputPointerEvent, m_InputPointerEvent.pointerCurrentRaycast.gameObject);
        //        }

        //        m_InputPointerEvent = null;

        //        return;
        //    }

        //    m_LastMousePosition = m_MousePosition;
        //    m_MousePosition = input.mousePosition;
        //}

        //private void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo)
        //{
        //    //TO ALLOW UP TO APPEAR AGAIN FOLLOWNG WAS COMMENTED; (Set on check on up onthisframe funcion on line 650)

        //    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

        //    var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);


        //    // if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
        //    //{
        //    //FIXME  1/11/20 - changed from original and added Flag.  //STOP CLICK FROM EXECUTING WHEN JUST RELEASING - THIS IS WHERE TO PUT THE EVENT THAT THE CLICK IS BASED ON
        //    if (m_isClickTrigger)
        //    {
        //        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
        //        m_isClickTrigger = false;
        //    }


        //    pointerEvent.eligibleForClick = false;
        //    pointerEvent.pointerPress = null;
        //    pointerEvent.rawPointerPress = null;

        //    //if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
        //    //    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

        //    // redo pointer enter / exit to refresh state
        //    // so that if we moused over something that ignored it before
        //    // due to having pressed on something else
        //    // it now gets it.
        //    //if (currentOverGo != pointerEvent.pointerEnter)
        //    //{
        //    //    HandlePointerExitAndEnter(pointerEvent, null);
        //    //    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
        //    //}

        //    m_InputPointerEvent = pointerEvent;
        //}

        //public override bool IsModuleSupported()
        //{
        //    return m_ForceModuleActive || input.mousePresent || input.touchSupported;
        //}

        //public override bool ShouldActivateModule()
        //{
        //    if (!base.ShouldActivateModule())
        //        return false;

        //    var shouldActivate = m_ForceModuleActive;
        //    shouldActivate |= input.GetButtonDown(m_SubmitButton);
        //    shouldActivate |= input.GetButtonDown(m_CancelButton);
        //    shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_HorizontalAxis), 0.0f);
        //    shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(m_VerticalAxis), 0.0f);
        //    shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
        //    shouldActivate |= input.GetMouseButtonDown(0);

        //    if (input.touchCount > 0)
        //        shouldActivate = true;

        //    return shouldActivate;
        //}

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        //public override void ActivateModule()
        //{
        //    //if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
        //    //    return;

        //    base.ActivateModule();
        //    //m_MousePosition = input.mousePosition;
        //    //m_LastMousePosition = input.mousePosition;

        //    var toSelect = eventSystem.currentSelectedGameObject;
        //    if (toSelect == null)
        //        toSelect = eventSystem.firstSelectedGameObject;

        //    eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        //}

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

     

        public void Set_Current_LineRender(Camera eventCamera)
        {
            current_LineRenderer = eventCamera.GetComponentInChildren<LineRenderer>(true);
            curLineRender_Trans = eventCamera.transform;

        }

        public bool isUpdating;
        public Vector3 currentCollisionLocation;
        public override void Process()
        {
            bool usedEvent = false;

            //send updates to selectedGameObject
            if (eventSystem.currentSelectedGameObject != null)
            {
                usedEvent = SendUpdateEventToSelectedObject();
                Send_HOVER_UpdateEventToSelectedObject();
               
            }

            //if (eventSystem.sendNavigationEvents)
            //{
            //    if (!usedEvent)
            //    {
            //        usedEvent |= SendMoveEventToSelectedObject();
            //        SendSubmitEventToSelectedObject();
            //    }
            //}

            //obtain camera look at coordinates to be able to work with the Event System
            var pointerEvent = GetMousePointerEventData(0).GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData;

         
            UpdateCollisionLocation(curLineRender_Trans, pointerEvent);

            ShowLine(current_LineRenderer, curLineRender_Trans, pointerEvent);

            Send_Slider_HOVER_UpdateEventToSelectedObject();
            //ProcessMove(pointerEvent);
            //ProcessDrag(pointerEvent);


            //if (!Mathf.Approximately(pointerEvent.scrollDelta.sqrMagnitude, 0.0f))
            //{
            //    var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(pointerEvent.pointerCurrentRaycast.gameObject);
            //    ExecuteEvents.ExecuteHierarchy(scrollHandler, pointerEvent, ExecuteEvents.scrollHandler);
            //}

        }

        public Transform cursor;
        private void UpdateCollisionLocation (Transform lineStart, PointerEventData pEvent) {
            currentCollisionLocation = lineStart.position + (lineStart.forward * (pEvent.pointerCurrentRaycast.distance + 0.025f));
            cursor.position = currentCollisionLocation;
            cursor.rotation = (Quaternion.FromToRotation(cursor.up, pEvent.pointerCurrentRaycast.worldNormal)) * cursor.rotation;
        }
       
        private void ShowLine (LineRenderer lRenderer, Transform lineStart, PointerEventData pEvent) 
        {
            

                //our current selection object
            var currentOverGo = pEvent.pointerCurrentRaycast.gameObject;

            //Set default line render selection lazer
            //if (cursor.gameObject.activeInHierarchy)
            //{
                lRenderer.SetPosition(0, lineStart.position);
                lRenderer.SetPosition(1, lineStart.position + (lineStart.forward * lengthOfDefaultLine));
            //}
            //else
            //{
            //    lRenderer.SetPosition(0, Vector3.zero);
            //    lRenderer.SetPosition(1, Vector3.zero);
            //}

            //when pointing at a target set the lazer to that distance
            if (currentOverGo)
            {
                lRenderer.SetPosition(1, currentCollisionLocation);
            }

            //skip searching for new buttons while lineRendering selection is on
           
              //  return;

            //check for new buttons to highlight
            if (currentOverGo != pEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pEvent, null);
                HandlePointerExitAndEnter(pEvent, currentOverGo);

                m_CurrentFocusedGameObject = pEvent.pointerCurrentRaycast.gameObject;
                eventSystem.SetSelectedGameObject(m_CurrentFocusedGameObject);
            }



        }



        /// <summary>
        /// Calculate and send a submit event to the current selected object.
        /// </summary>
        /// <returns>If the submit event was used by the selected object.</returns>
        protected bool SendSubmitEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            if (input.GetButtonDown(m_SubmitButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

            if (input.GetButtonDown(m_CancelButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            return data.used;
        }

        private Vector2 GetRawMoveVector()
        {
            Vector2 move = Vector2.zero;
            move.x = input.GetAxisRaw(m_HorizontalAxis);
            move.y = input.GetAxisRaw(m_VerticalAxis);

            if (input.GetButtonDown(m_HorizontalAxis))
            {
                if (move.x < 0)
                    move.x = -1f;
                if (move.x > 0)
                    move.x = 1f;
            }
            if (input.GetButtonDown(m_VerticalAxis))
            {
                if (move.y < 0)
                    move.y = -1f;
                if (move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// Calculate and send a move event to the current selected object.
        /// </summary>
        /// <returns>If the move event was used by the selected object.</returns>
        protected bool SendMoveEventToSelectedObject()
        {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
            {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

            // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
            if (similarDir && m_ConsecutiveMoveCount == 1)
            {
                if (time <= m_PrevActionTime + m_RepeatDelay)
                    return false;
            }
            // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
            else
            {
                if (time <= m_PrevActionTime + 1f / m_InputActionsPerSecond)
                    return false;
            }

            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

            if (axisEventData.moveDir != MoveDirection.None)
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                if (!similarDir)
                    m_ConsecutiveMoveCount = 0;
                m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            }
            else
            {
                m_ConsecutiveMoveCount = 0;
            }

            return axisEventData.used;
        }



        protected bool SendUpdateEventToSelectedObject()
        {
            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        protected bool Send_HOVER_UpdateEventToSelectedObject()
        {
            if (!isUpdating)
                return false;
            //     var data = GetBaseEventData();
            if (!current_LineRenderer.gameObject.activeInHierarchy)
                isUpdating = false;

            CursorHoverEventData data;
            
                 data = new CursorHoverEventData(
                                         EventSystem.current,
                                         currentCollisionLocation);
          


            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, CursorHoverEventData.cursorFollowDelegate);
            return data.used;
        }

        protected bool Send_Slider_HOVER_UpdateEventToSelectedObject()
        {
            if (!isUpdating)
                return false;
            //     var data = GetBaseEventData();
            if (!current_LineRenderer.gameObject.activeInHierarchy)
                isUpdating = false;

            SliderEventData data;

            data = new SliderEventData(
                                    EventSystem.current,
                                    currentCollisionLocation);

            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, SliderEventData.cursorFollowDelegate);
            return data.used;
        }

        protected bool Send_Drag_UpdateEventToSelectedObject()
        {
            BaseEventData data = GetBaseEventData();
            
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.initializePotentialDrag);
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.dragHandler);

            return data.used;
            //if (!isUpdating)
            //    return false;
            ////     var data = GetBaseEventData();
            //if (!current_LineRenderer.gameObject.activeInHierarchy)
            //    isUpdating = false;

            //CursorHoverEventData data;

            //data = new CursorHoverEventData(
            //                        EventSystem.current,
            //                        currentCollisionLocation);



            //ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, CursorHoverEventData.cursorFollowDelegate);
            //return data.used;
        }


        // Vector3 cachePositionLR;
        /// <summary>
        /// Calculate and process any mouse button state changes.
        /// </summary>

        protected void ProcessMousePress(MouseButtonEventData data)
        {


            // PointerDown notification
            //if (m_isClickTrigger)//data.PressedThisFrame())
            //{
            //    // m_isClickTrigger = false;

            //    pointerEvent.eligibleForClick = true;
            //    pointerEvent.delta = Vector2.zero;
            //    pointerEvent.dragging = false;
            //    pointerEvent.useDragThreshold = true;
            //    pointerEvent.pressPosition = pointerEvent.position;
            //    pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            //    DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            //    // search for the control that will receive the press
            //    // if we can't find a press handler set the press
            //    // handler to be what would receive a click.
            //    var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            //    // didnt find a press handler... search for a click handler
            //    if (newPressed == null)
            //        newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            //    // Debug.Log("Pressed: " + newPressed);

            //    float time = Time.unscaledTime;

            //    if (newPressed == pointerEvent.lastPress)
            //    {
            //        var diffTime = time - pointerEvent.clickTime;
            //        if (diffTime < 0.3f)
            //            ++pointerEvent.clickCount;
            //        else
            //            pointerEvent.clickCount = 1;

            //        pointerEvent.clickTime = time;
            //    }
            //    else
            //    {
            //        pointerEvent.clickCount = 1;
            //    }

            //    pointerEvent.pointerPress = newPressed;
            //    pointerEvent.rawPointerPress = currentOverGo;

            //    pointerEvent.clickTime = time;

            //    m_InputPointerEvent = pointerEvent;
            //}

            //FIXME  1/11/20 - Removed from original.// PointerUp notification CHANGE TO ALLOW UP EVENT
            //if (true)//data.ReleasedThisFrame())
            //{
            //    ReleaseMouse(pointerEvent, currentOverGo);
            //}
        }

        protected GameObject GetCurrentFocusedGameObject()
        {
            return m_CurrentFocusedGameObject;
        }
    }
}
