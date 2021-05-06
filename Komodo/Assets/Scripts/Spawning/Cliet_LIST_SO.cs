using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SCRIPTABLE_OBJECTS/List/Client_List", menuName = "new_Client_List", order = 0)]
public class Client_LIST_SO : ScriptableObject
{
    public List<EntityData_SO> clientlist;
  
}
