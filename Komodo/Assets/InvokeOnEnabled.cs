using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnEnabled : MonoBehaviour
{
    public UnityEvent onEnabled;
    public void OnEnable()
    {
        onEnabled.Invoke();
    }
}
