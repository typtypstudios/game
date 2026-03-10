using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class NoAutoCreateAttribute : Attribute { }

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    // Check if the class has the [NoAutoCreate] attribute
                    bool canAutoCreate = typeof(T).GetCustomAttribute<NoAutoCreateAttribute>() == null;

                    if (canAutoCreate)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                    else
                    {
                        //Debug.LogError($"[Singleton] {typeof(T).Name} is marked with [NoAutoCreate] but was not found in the scene.");
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (transform.parent == null) DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}