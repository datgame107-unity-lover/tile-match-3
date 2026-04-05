// Scripts/LevelCreate/LevelJsonService.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelJsonService
{
    private readonly string _dir;

    public LevelJsonService()
    {
        _dir = Path.Combine(Application.persistentDataPath, "Levels");
        Directory.CreateDirectory(_dir);
    }

    private string FilePath(int index) =>
        Path.Combine(_dir, $"level_{index}.json");

    public void Save(LevelSaveData data)
    {
        File.WriteAllText(FilePath(data.levelIndex), JsonUtility.ToJson(data, true));
        Debug.Log($"[LevelJson] Saved level {data.levelIndex} → {FilePath(data.levelIndex)}");
    }

    public LevelSaveData Load(int index)
    {
        var path = FilePath(index);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<LevelSaveData>(File.ReadAllText(path));
    }

    public void Delete(int index)
    {
        var path = FilePath(index);
        if (File.Exists(path)) File.Delete(path);
    }

    public List<int> GetAllLevelIndices()
    {
        var result = new List<int>();
        foreach (var f in Directory.GetFiles(_dir, "level_*.json"))
        {
            var name = Path.GetFileNameWithoutExtension(f);
            if (int.TryParse(name.Replace("level_", ""), out int idx))
                result.Add(idx);
        }
        result.Sort();
        return result;
    }
    public List<LevelSaveData> LoadAll()
    {
        var list = new List<LevelSaveData>();

        foreach (var index in GetAllLevelIndices())
        {
            var data = Load(index);
            if (data != null)
                list.Add(data);
        }

        return list;
    }
}