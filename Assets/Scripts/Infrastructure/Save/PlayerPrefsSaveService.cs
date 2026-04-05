// Scripts/Infrastructure/Save/PlayerPrefsSaveService.cs
using UnityEngine;

public class PlayerPrefsSaveService : ISaveService
{
    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public int GetInt(string key, int def = 0) => PlayerPrefs.GetInt(key, def);
    public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
    public float GetFloat(string key, float def = 0f) => PlayerPrefs.GetFloat(key, def);
    public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
    public string GetString(string key, string def = "") => PlayerPrefs.GetString(key, def);
    public bool HasKey(string key) => PlayerPrefs.HasKey(key);
    public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
    public void Save() => PlayerPrefs.Save();
}