#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class AchievementEditorWindow : EditorWindow
{   

    private List<AchievementData> achievements;
    private Vector2 scroll;
    private const string ACHIEVEMENT_PATH =
    "Assets/ScriptableObjects/Achievements";

    [MenuItem("Tools/Achievement Editor")]
    public static void Open()
    {
        GetWindow<AchievementEditorWindow>("Achievement Editor");
    }

    private void OnEnable()
    {
        LoadAchievements();
    }

    private void LoadAchievements()
    {
        achievements = new List<AchievementData>();
        string[] guids = AssetDatabase.FindAssets("t:AchievementData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            achievements.Add(AssetDatabase.LoadAssetAtPath<AchievementData>(path));
        }
    }
    private void CreateNewAchievement()
    {
        AchievementData newAchievement =
            ScriptableObject.CreateInstance<AchievementData>();

        newAchievement.type = AchievementType.PlayGame;
        newAchievement.target = 1;
        newAchievement.id = $"achievement_{System.Guid.NewGuid()}";
        newAchievement.name = "New Achievement";

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(
            $"{ACHIEVEMENT_PATH}/Achievement_New.asset");

        AssetDatabase.CreateAsset(newAchievement, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = newAchievement;
        EditorGUIUtility.PingObject(newAchievement);

        LoadAchievements();
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add New Achievement", GUILayout.Height(30)))
        {
            CreateNewAchievement();
        }
        if (GUILayout.Button("Reload"))
        {
            LoadAchievements();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5); // thêm khoảng cách

        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var achievement in achievements)
        {
            EditorGUILayout.BeginVertical("box");

            achievement.type = (AchievementType)EditorGUILayout.EnumPopup("Type", achievement.type);
            achievement.id = EditorGUILayout.TextField("ID", achievement.id);
            achievement.name = EditorGUILayout.TextField("Name", achievement.name);
            achievement.description = EditorGUILayout.TextField("Description", achievement.description);
            achievement.target = EditorGUILayout.IntField("Target", achievement.target);
            achievement.icon = (Sprite)EditorGUILayout.ObjectField("Icon", achievement.icon, typeof(Sprite), false);
            achievement.disabled =
    EditorGUILayout.Toggle("Disabled", achievement.disabled);

            EditorGUILayout.EndVertical();
            EditorUtility.SetDirty(achievement);

        }

        EditorGUILayout.EndScrollView();
    }
}
#endif
