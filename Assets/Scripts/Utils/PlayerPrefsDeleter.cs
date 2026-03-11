#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PlayerPrefsDeleter : MonoBehaviour
{
    public void DeletePlayerPrefs() => PlayerPrefs.DeleteAll();
}

[CustomEditor(typeof(PlayerPrefsDeleter))]
public class PlayerPrefsDeleterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerPrefsDeleter deleter = (PlayerPrefsDeleter)target;
        if (GUILayout.Button("DELETE PLAYER PREFS")) deleter.DeletePlayerPrefs();
    }
}
#endif