public static class SaveKeys
{
    public const string DefaultSlotId = "slot_0";
    public const string ActiveSlotPrefsKey = "save_active_slot";
    public const string GlobalSettingsKey = "global_settings";
    public const string SlotPrefix = "save_slot_";
    public const string SavesFolderName = "Saves";

    public static string GetSlotKey(string slotId)
    {
        return SlotPrefix + slotId;
    }
}
