using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "String_List", menuName = "new_String_List", order = 0)]
public class String_List : ScriptableObject
{
    //public List<string> string_list;
    public List<URL_Content> url_list;


    [System.Serializable]
    public struct URL_Content
    {
        public int id;
        public string name;
        public string url;

       

        public float scale;
        public Vector3 position;
        public Vector3 euler_rotation;

        public bool isWholeObject;
        public string loadInstruction;
       

    }
  
}
