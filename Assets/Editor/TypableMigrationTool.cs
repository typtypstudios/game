using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TypTyp.Input;
using TypTyp.TextSystem.Typable;

public static class TypableMigrationTool
{
    [MenuItem("Tools/Typable/Migrate WritableText (Selected)")]
    private static void MigrateSelected()
    {
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected.");
            return;
        }

        int migrated = 0;
        foreach (var root in selection)
        {
            if (root == null) continue;
            migrated += MigrateInHierarchy(root);
        }

        if (migrated > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }

        Debug.Log($"Migrated {migrated} WritableText component(s) in selection.");
    }

    [MenuItem("Tools/Typable/Migrate WritableText (Scene)")]
    private static void MigrateScene()
    {
        var writables = Object.FindObjectsByType<WritableText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int migrated = 0;
        foreach (var w in writables)
        {
            if (w == null) continue;
            migrated += MigrateGameObject(w.gameObject) ? 1 : 0;
        }

        if (migrated > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }

        Debug.Log($"Migrated {migrated} WritableText component(s) in scene.");
    }

    [MenuItem("Tools/Typable/Migrate WritableButton (Selected)")]
    private static void MigrateButtonsSelected()
    {
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected.");
            return;
        }

        int migrated = 0;
        foreach (var root in selection)
        {
            if (root == null) continue;
            migrated += MigrateButtonsInHierarchy(root);
        }

        if (migrated > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }

        Debug.Log($"Migrated {migrated} WritableButton component(s) in selection.");
    }

    [MenuItem("Tools/Typable/Migrate WritableButton (Scene)")]
    private static void MigrateButtonsScene()
    {
        var writables = Object.FindObjectsByType<WritableButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int migrated = 0;
        foreach (var w in writables)
        {
            if (w == null) continue;
            migrated += MigrateButtonGameObject(w.gameObject) ? 1 : 0;
        }

        if (migrated > 0)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }

        Debug.Log($"Migrated {migrated} WritableButton component(s) in scene.");
    }

    private static int MigrateInHierarchy(GameObject root)
    {
        int migrated = 0;
        var writables = root.GetComponentsInChildren<WritableText>(true);
        foreach (var w in writables)
        {
            if (w == null) continue;
            migrated += MigrateGameObject(w.gameObject) ? 1 : 0;
        }
        return migrated;
    }

    private static int MigrateButtonsInHierarchy(GameObject root)
    {
        int migrated = 0;
        var writables = root.GetComponentsInChildren<WritableButton>(true);
        foreach (var w in writables)
        {
            if (w == null) continue;
            migrated += MigrateButtonGameObject(w.gameObject) ? 1 : 0;
        }
        return migrated;
    }

    private static bool MigrateGameObject(GameObject go)
    {
        var writable = go.GetComponent<WritableText>();
        if (writable == null) return false;

        Undo.RecordObject(go, "Migrate WritableText");

        // Ensure TypingInputListener
        var input = go.GetComponent<TypingInputListener>();
        if (input == null)
            input = Undo.AddComponent<TypingInputListener>(go);

        // Ensure TypableController
        var controller = go.GetComponent<TypableController>();
        if (controller == null)
            controller = Undo.AddComponent<TypableController>(go);

        // Ensure TMPTypableView
        var view = go.GetComponent<TMPTypableView>();
        if (view == null)
            view = Undo.AddComponent<TMPTypableView>(go);

        // Ensure TypableTMPTextInitializer
        var initializer = go.GetComponent<TypableTMPTextInitializer>();
        if (initializer == null)
            initializer = Undo.AddComponent<TypableTMPTextInitializer>(go);

        // Wire TMP reference (search in children too)
        var tmp = go.GetComponent<TMPro.TMP_Text>();
        if (tmp == null)
            tmp = go.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (tmp != null)
        {
            var so = new SerializedObject(view);
            var textProp = so.FindProperty("tmp");
            if (textProp != null)
                textProp.objectReferenceValue = tmp;
            so.ApplyModifiedPropertiesWithoutUndo();

            var initSo = new SerializedObject(initializer);
            var initTextProp = initSo.FindProperty("text");
            if (initTextProp != null)
                initTextProp.objectReferenceValue = tmp;
            initSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // Wire controller views array
        {
            var so = new SerializedObject(controller);
            var viewsProp = so.FindProperty("views");
            viewsProp.arraySize = 1;
            viewsProp.GetArrayElementAtIndex(0).objectReferenceValue = view;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Wire initializer controller
        {
            var so = new SerializedObject(initializer);
            so.FindProperty("controller").objectReferenceValue = controller;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Disable WritableText (do not remove)
        writable.enabled = false;
        EditorUtility.SetDirty(writable);

        return true;
    }

    private static bool MigrateButtonGameObject(GameObject go)
    {
        var writable = go.GetComponent<WritableButton>();
        if (writable == null) return false;

        Undo.RecordObject(go, "Migrate WritableButton");

        // Ensure TypingInputListener
        var input = go.GetComponent<TypingInputListener>();
        if (input == null)
            input = Undo.AddComponent<TypingInputListener>(go);

        // Ensure TypableController
        var controller = go.GetComponent<TypableController>();
        if (controller == null)
            controller = Undo.AddComponent<TypableController>(go);

        // Ensure TMPTypableView
        var view = go.GetComponent<TMPTypableView>();
        if (view == null)
            view = Undo.AddComponent<TMPTypableView>(go);

        // Ensure TypableTMPTextInitializer
        var initializer = go.GetComponent<TypableTMPTextInitializer>();
        if (initializer == null)
            initializer = Undo.AddComponent<TypableTMPTextInitializer>(go);

        // Wire TMP reference if present
        var tmp = go.GetComponent<TMPro.TMP_Text>();
        if (tmp != null)
        {
            var so = new SerializedObject(view);
            var textProp = so.FindProperty("tmp");
            if (textProp != null)
                textProp.objectReferenceValue = tmp;
            so.ApplyModifiedPropertiesWithoutUndo();

            var initSo = new SerializedObject(initializer);
            var initTextProp = initSo.FindProperty("text");
            if (initTextProp != null)
                initTextProp.objectReferenceValue = tmp;
            initSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // Wire controller views array
        {
            var so = new SerializedObject(controller);
            var viewsProp = so.FindProperty("views");
            viewsProp.arraySize = 1;
            viewsProp.GetArrayElementAtIndex(0).objectReferenceValue = view;
            var configPresetProp = so.FindProperty("configPreset");
            var configProp = so.FindProperty("config");
            if (configPresetProp != null)
                configPresetProp.objectReferenceValue = null;
            if (configProp != null)
            {
                var stopProp = configProp.FindPropertyRelative("StopOnMistake");
                var markProp = configProp.FindPropertyRelative("MarkMistakes");
                var caseProp = configProp.FindPropertyRelative("CaseSensitive");
                var resetMistakeProp = configProp.FindPropertyRelative("ResetOnMistake");
                var resetCompleteProp = configProp.FindPropertyRelative("ResetOnComplete");
                if (stopProp != null) stopProp.boolValue = true;
                if (markProp != null) markProp.boolValue = false;
                if (caseProp != null) caseProp.boolValue = false;
                if (resetMistakeProp != null) resetMistakeProp.boolValue = false;
                if (resetCompleteProp != null) resetCompleteProp.boolValue = false;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Wire initializer controller
        {
            var so = new SerializedObject(initializer);
            so.FindProperty("controller").objectReferenceValue = controller;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Keep WritableButton enabled; migration only adds components.
        EditorUtility.SetDirty(writable);

        return true;
    }
}
