using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scroll_VR_EventHandler : MonoBehaviour, ISliderHover, IPointerEnterHandler
{
    public Slider slider;
    RectTransform rectTrans;
    Rect rect;
    public Standalone_InputModule_Modifie_IndCameraRays standalone;
    public void Start()
    {
     //   slider = GetComponent<Slider>();
        rectTrans = GetComponent<RectTransform>();
        Vector3 posOFScroll = GetRectTransformNormal(rectTrans);
       // Debug.Log(posOFScroll);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 posOFScroll = GetRectTransformNormal(rectTrans);
        Debug.Log(posOFScroll);

    }
    public void OnSliderHover(SliderEventData cursorData)
    {
        var locPos = rectTrans.InverseTransformPoint(standalone.currentCollisionLocation);
       var xNorm = (locPos.x - 0f) / 100f;//(0.2f - 0f);
       // var yNorm = Mathf.Clamp01((locPos.y - 0f) / 10f) ;// Mathf.Max(0, (locPos.y - 0f) / 100f);//(0.5f - 0f));

        slider.normalizedValue = xNorm + 0.6f;

        Debug.Log(xNorm + 0.6f);
    }

    //public void OnHover(CursorHoverEventData cursorData)
    //{
    //  var locPos = rectTrans.InverseTransformPoint(cursorData.currentHitLocation);
    //    var xNorm = (locPos.x - 0f) / (0.2f - 0f);
    //    var yNorm = (locPos.y - 0f) / (0.5f - 0f);
    //    slider.normalizedValue = yNorm;
    //    // //X_normalized = (b - a) * [(x - y) / (z - y)] + a

    //    // Vector3 posOFScroll = GetRectTransformNormal(rectTrans);
    //    //   Debug.Log(posOFScroll);
    //      Debug.Log(yNorm);
    //    //  var norm = x *rectTrans.;


    //    //  var normalized = lo
    //    //slider.normalizedValue = Vector3.Normalize(locPos);

    //}
    /// <summary>
    /// For RectTransform, calculate it's normal in world space
    /// </summary>
    static Vector3 GetRectTransformNormal(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 BottomEdge = corners[3] - corners[0];
        Vector3 LeftEdge = corners[1] - corners[0];
        rectTransform.GetWorldCorners(corners);
        return Vector3.Cross(LeftEdge, BottomEdge).normalized;
    }


  
}
