#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveDataTools
{
    [MenuItem("Tools/Save Data/Delete All Saved Game Data")]
    private static void DeleteAllSavedGameData()
    {
        string savesPath = Path.Combine(Application.persistentDataPath, SaveKeys.SavesFolderName);
        bool confirmed = EditorUtility.DisplayDialog(
            "Delete Saved Game Data",
            $"This will delete save files in:\n{savesPath}\n\nand clear this project's active save slot preference.",
            "Delete",
            "Cancel");

        if (!confirmed)
        {
            return;
        }

        if (Directory.Exists(savesPath))
        {
            Directory.Delete(savesPath, true);
        }

        PlayerPrefs.DeleteKey(SaveKeys.ActiveSlotPrefsKey);
        PlayerPrefs.Save();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Saved Game Data Deleted",
            "All save-system data for this project has been removed.",
            "OK");
    }
}
#endif
