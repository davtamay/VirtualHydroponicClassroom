using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asset_Management_Utility : SingletonComponent<Asset_Management_Utility>
{
    public static Asset_Management_Utility Instance
    {
        get { return ((Asset_Management_Utility)_Instance); }
        set { _Instance = value; }
    }
    public GameObject gO;
    public void LoadFromResource(string name)
    {
        //gO = Resources.Load<GameObject>(name) as GameObject;
        //Instantiate(gO);

    }
}
