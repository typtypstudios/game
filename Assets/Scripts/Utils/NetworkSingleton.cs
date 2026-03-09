using UnityEngine;
using Unity.Netcode;
using System;
using System.Reflection;
public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
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
                    bool canAutoCreate =
                        typeof(T).GetCustomAttribute<NoAutoCreateAttribute>() == null;

                    if (canAutoCreate)
                    {
                        GameObject go = new GameObject(typeof(T).Name + " (NetworkSingleton)");
                        _instance = go.AddComponent<T>();
                    }
                    else
                    {
                        Debug.LogError(
                            $"[NetworkSingleton] {typeof(T).Name} is marked with [NoAutoCreate] but was not found."
                        );
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
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}