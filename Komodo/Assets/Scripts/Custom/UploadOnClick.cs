using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadOnClick : MonoBehaviour
{
    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;
    Color m_MouseDownColor = Color.blue;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    MeshRenderer m_Renderer;

    private bool _isMouseDown;
    private bool _isMouseInside;

    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();
        //Fetch the original color of the GameObject
        m_OriginalColor = m_Renderer.material.color;
    }

    private void Update()
    {
        if (_isMouseInside && !_isMouseDown)
        {
            m_Renderer.material.color = m_MouseOverColor;
            return;
        }
        if (_isMouseDown)
        {
            m_Renderer.material.color = m_MouseDownColor;
            return;
        }
        m_Renderer.material.color = m_OriginalColor;
    }

    void OnMouseOver()
    {
        // change the color of the gameobject to red when the mouse is over gameobject
        _isMouseInside = true;
    }

    void OnMouseExit()
    {
        // reset the color of the gameobject back to normal
        _isMouseInside = false;
    }

    void OnMouseDown()
    {
        // Change the color of the GameObject to red when the mouse is over GameObject
        _isMouseDown = true;
        Debug.Log("Cube clicked.");
    //    using (var assetLoader = new AssetLoader()) {
    //var assetLoaderOptions = AssetLoaderOptions.CreateInstance();   //Creates the AssetLoaderOptions instance.
    //                                                                //AssetLoaderOptions let you specify options to load your model.
    //                                                                //(Optional) You can skip this object creation and it's parameter or pass null.
    
    ////You can modify assetLoaderOptions before passing it to LoadFromFile method. You can check the AssetLoaderOptions API reference at:
    ////https://ricardoreis.net/trilib/manual/html/class_tri_lib_1_1_asset_loader_options.html
    
    //var wrapperGameObject = gameObject;                             //Sets the game object where your model will be loaded into.
    //                                                                //(Optional) You can skip this object creation and it's parameter or pass null.

    //var myGameObject = assetLoader.LoadFromFile("PATH TO MY FILE.FBX", assetLoaderOptions, wrapperGameObject); //Loads the model synchronously and stores the reference in myGameObject.
//}
    }

    void OnMouseUp()
    {
        // Change the color of the GameObject to red when the mouse is over GameObject
        _isMouseDown = false;
    }
}
