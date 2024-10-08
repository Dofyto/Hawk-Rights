using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string SAVED_GAME = "savedGame";

    public static void SaveGame(SaveData data)
    {
        PlayerPrefs.SetString(SAVED_GAME, JsonUtility.ToJson(data));
        PlayerPrefs.Save(); // Ensure data is saved
    }

    public static SaveData LoadGame()
    {
        if (!IsGameSaved()) return null;
        return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SAVED_GAME));
    }

    public static bool IsGameSaved()
    {
        return PlayerPrefs.HasKey(SAVED_GAME);
    }

    public static void ClearSavedGame()
    {
        PlayerPrefs.DeleteKey(SAVED_GAME);
    }
}
