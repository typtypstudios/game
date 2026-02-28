using System.Reflection;
using UnityEngine;

public abstract class ScriptableSingleton<T> : ScriptableObject
    where T : ScriptableObject
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<T>(typeof(T).Name);// ?? Resources.LoadAll<T>("")[0];

                if (_instance == null)
                {
                    // Check for our custom attribute (same one used for MonoBehaviours)
                    bool canAutoCreate = typeof(T).GetCustomAttribute<NoAutoCreateAttribute>() == null;

                    if (canAutoCreate)
                    {
                        _instance = CreateInstance<T>();
                    }
                    else
                    {
                        Debug.LogError($"[ScriptableSingleton] {typeof(T).Name} asset is missing from Resources!");
                    }
                }
            }
            return _instance;
        }
    }
}