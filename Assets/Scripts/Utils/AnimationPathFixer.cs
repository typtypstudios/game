#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationPathFixer : EditorWindow
{
    private string prefix = "";
    private List<AnimationClip> clips = new();

    [MenuItem("Tools/Fix Animation Paths")]
    static void Open() => GetWindow<AnimationPathFixer>("Fix Animation Paths");

    private void OnGUI()
    {
        prefix = EditorGUILayout.TextField("Prefix", prefix);

        EditorGUILayout.LabelField("Drag clips here:");

        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop AnimationClips here");

        Event evt = Event.current;
        if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var obj in DragAndDrop.objectReferences)
                    if (obj is AnimationClip clip && !clips.Contains(clip))
                        clips.Add(clip);
            }
            evt.Use();
        }

        for (int i = clips.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(clips[i], typeof(AnimationClip), false);
            if (GUILayout.Button("X", GUILayout.Width(20))) clips.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Fix Selected Clips"))
        {
            foreach (var clip in clips) FixClip(clip, prefix);
            AssetDatabase.SaveAssets();
        }
    }

    private void FixClip(AnimationClip clip, string prefix)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            AnimationUtility.SetEditorCurve(clip, binding, null);
            var newBinding = binding;
            newBinding.path = prefix + "/" + binding.path;
            AnimationUtility.SetEditorCurve(clip, newBinding, curve);
        }
    }
}
#endif