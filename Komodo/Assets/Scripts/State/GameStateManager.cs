using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable{
    void OnUpdate(float realTime);
    }

[SerializeField]
public class GameStateManager : SingletonComponent<GameStateManager>
{
    public static GameStateManager Instance
    {
        get { return ((GameStateManager)_Instance); }
        set { _Instance = value; }
    }

    public List<IUpdatable> _updateObjects = new List<IUpdatable>();
    
    public void RegisterUpdatableObject(IUpdatable obj)
    {
        if (!_updateObjects.Contains(obj))
        {
            _updateObjects.Add(obj);

        }
    }

    public void DeRegisterUpdatableObject(IUpdatable obj)
    {
        if (_updateObjects.Contains(obj))
        {
            _updateObjects.Remove(obj);

        }
    }

    void Update()
    {
        float rT = Time.realtimeSinceStartup;
        for (int i = 0; i < _updateObjects.Count; i++)
        {
            _updateObjects[i].OnUpdate(rT);
        }
        
    }
}
