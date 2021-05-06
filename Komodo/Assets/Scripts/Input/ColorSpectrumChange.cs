using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSpectrumChange : MonoBehaviour
{
    public Transform centerColorPosition;
    public Transform redColorPosition;
    public Transform blueColorPosition;
    public Transform greenColorPosition;

    public Transform currentColorTarget;

    public Vector3 redColorCurrrentValue;
    public Vector3 blueColorCurrrentValue;
    public Vector3 greenColorCurrrentValue;

    public Vector3 redPositiveValue;//+ depending on 0 origin
    public Vector3 redNegativeValue;//-

    public Image currentColorDisplay;

    public void Start()
    {
        redPositiveValue = redColorPosition.position;
        redNegativeValue = -redColorPosition.position;
    }

    public void Update()
    {
        float redDistance = default;
        float redTargetDistance = default;
        float redColorValue = default;

        float blueDistance = default;
        float blueTargetDistance = default;
        float blueColorValue = default;

        float greenDistance = default;
        float greenTargetDistance = default;
        float greenColorValue = default;

        if (redColorPosition.localPosition.z >= currentColorTarget.localPosition.z && currentColorTarget.localPosition.z >= 0)//&& redColorPosition.localPosition.z >= currentColorTarget.localPosition.z)
        {
            redDistance = Vector3.Distance(redColorPosition.position, centerColorPosition.position);
            redTargetDistance = Vector3.Distance(redColorPosition.position, currentColorTarget.position);
            redColorValue = Mathf.Abs( (redTargetDistance / redDistance) - 1f);//Mathf.Clamp((redTargetDistance / redDistance) -1f , 0, 1);
        }


      if(currentColorTarget.localPosition.x <= 0)

      ///  if (centerColorPosition.localPosition.x >= blueColorPosition.localPosition.x)
        //   if (blueColorPosition.localPosition.z <= currentColorTarget.localPosition.z)
        //        && currentColorTarget.localPosition.z -centerColorPosition.localPosition.z <= 0
        //   )
        {


            //if (currentColorTarget.localPosition.z <= 0 && blueColorPosition.localPosition.z <= currentColorTarget.localPosition.z)
            //{
            blueDistance = Vector3.Distance(blueColorPosition.position, centerColorPosition.position);
            blueTargetDistance = Vector3.Distance(blueColorPosition.position, currentColorTarget.position);
            blueColorValue = Mathf.Abs((blueTargetDistance / blueDistance) - 1f); // Mathf.Clamp(Mathf.Abs((blueTargetDistance / blueDistance)), 0, 1);
            //    }
        }

        if (currentColorTarget.localPosition.x >= 0)
        // if(centerColorPosition.localPosition.x <= greenColorPosition.localPosition.x)
        //   if (greenColorPosition.localPosition.x <= currentColorTarget.localPosition.x)
        //      && currentColorTarget.localPosition.z - centerColorPosition.localPosition.z <= 0


        {
            //if (currentColorTarget.localPosition.z <= centerColorPosition.localPosition.z && greenColorPosition.localPosition.z >= currentColorTarget.localPosition.z)
            //{
            greenDistance = Vector3.Distance(greenColorPosition.position, centerColorPosition.position);
            greenTargetDistance = Vector3.Distance(greenColorPosition.position, currentColorTarget.position);
            greenColorValue = Mathf.Abs((greenTargetDistance / greenDistance) - 1f);// Mathf.Clamp(Mathf.Abs((greenTargetDistance / greenDistance)), 0, 1);
        }
     //   }
        // (redColorPosition.position - centerColorPosition.position).magnitude; //* 255;          //  Vector3.Distance(centerColorPosition.position, redColorPosition.position). * 255;
        //     float redColorSpectrum = redDistance.magnitude / 255f;

        Debug.Log("Color:" + new Color(redColorValue, blueColorValue, greenColorValue));
        currentColorDisplay.color = new Color(redColorValue, blueColorValue, greenColorValue);
        //  /255
    }

}
