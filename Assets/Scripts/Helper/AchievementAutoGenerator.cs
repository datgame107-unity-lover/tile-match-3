using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AchievementAutoGenerator
{
    private const string DATA_ROOT_PATH = "Assets/ScriptableObjects/Achievements";
    private const string ICON_ROOT_PATH = "Assets/Sprites";

    // Target theo tier (có thể chỉnh)
    private static readonly int[] TIER_TARGETS = { 10, 50, 200 };

    [MenuItem("Tools/Achievement/Generate All Achievements")]
    public static void GenerateAll()
    {
        foreach (AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            GenerateForType(type);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Achievement] Generate All Achievements completed.");
    }

    private static void GenerateForType(AchievementType type)
    {
        string typeFolder = $"{DATA_ROOT_PATH}/";
        if (!Directory.Exists(typeFolder))
        {
            Directory.CreateDirectory(typeFolder);
        }

        for (int tier = 1; tier <= 3; tier++)
        {
            string assetPath = $"{typeFolder}/{type}_T{tier}.asset";

            // Nếu đã tồn tại thì bỏ qua (tránh ghi đè)
            if (AssetDatabase.LoadAssetAtPath<AchievementData>(assetPath) != null)
                continue;

            AchievementData data = ScriptableObject.CreateInstance<AchievementData>();

            data.type = type;
            data.id = $"{type}_T{tier}";
            data.name = GetName(type, tier);
            data.description = GetDescription(type, tier);
            data.target = TIER_TARGETS[tier - 1];
            data.rewards = new List<QuestReward>();
            data.icon = LoadIcon(type, tier);

            AssetDatabase.CreateAsset(data, assetPath);
        }
    }

    #region ICON

    private static Sprite LoadIcon(AchievementType type, int tier)
    {
        string spriteName = $"{type}{tier}";
        string sheetPath = "Assets/Sprites/achivement_icons-Photoroom.png";

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(sheetPath);

        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite && sprite.name == spriteName)
                return sprite;
        }

        Debug.LogWarning($"[AchievementAutoGenerator] Missing sprite: {spriteName}");
        return null;
    }


    #endregion

    #region TEXT DATA

    private static string GetName(AchievementType type, int tier)
    {
        return type switch
        {
            AchievementType.PlayGame => tier switch
            {
                1 => "First Click Ever",
                2 => "Just One More Game",
                _ => "Certified Addict"
            },

            AchievementType.ClearLevel => tier switch
            {
                1 => "Baby Steps",
                2 => "Level Crusher",
                _ => "Level Destroyer"
            },

            AchievementType.CollectItem => tier switch
            {
                1 => "Shiny Things",
                2 => "Hoarder Mode",
                _ => "Inventory Nightmare"
            },

            AchievementType.MatchTiles => tier switch
            {
                1 => "Match Made Easy",
                2 => "Combo Lover",
                _ => "Tile Wizard"
            },

            AchievementType.UseBooster => tier switch
            {
                1 => "Panic Button",
                2 => "Booster Fan",
                _ => "Booster Abuser"
            },

            AchievementType.HighScore => tier switch
            {
                1 => "Personal Best",
                2 => "Score Hunter",
                _ => "Score Legend"
            },

            AchievementType.EarnFlower => tier switch
            {
                1 => "Flower Picker",
                2 => "Garden Owner",
                _ => "Flower Tycoon"
            },

            AchievementType.SpendFlower => tier switch
            {
                1 => "Worth It",
                2 => "Big Spender",
                _ => "Broke Again"
            },

            AchievementType.EarnDiamond => tier switch
            {
                1 => "Shiny Rock",
                2 => "Diamond Lover",
                _ => "Diamond King"
            },

            AchievementType.SpendDiamond => tier switch
            {
                1 => "Luxury Taste",
                2 => "Diamond Burner",
                _ => "Zero Diamonds"
            },

            AchievementType.LoseHeart => tier switch
            {
                1 => "Oops",
                2 => "Try Again",
                _ => "Out of Hearts"
            },

            AchievementType.LoginDays => tier switch
            {
                1 => "Welcome Back",
                2 => "Daily Visitor",
                _ => "Never Miss a Day"
            },

            AchievementType.CompleteQuest => tier switch
            {
                1 => "Task Done",
                2 => "Quest Machine",
                _ => "Quest Master"
            },

            _ => $"{type} Tier {tier}"
        };
    }

    private static string GetDescription(AchievementType type, int tier)
    {
        return type switch
        {
            AchievementType.PlayGame => "Playing is easy. Stopping is hard.",
            AchievementType.ClearLevel => "Another level down. Many more to go.",
            AchievementType.CollectItem => "You might need these someday.",
            AchievementType.MatchTiles => "Tiles obey your commands.",
            AchievementType.UseBooster => "Boosters make life easier.",
            AchievementType.HighScore => "Higher numbers, bigger smiles.",
            AchievementType.EarnFlower => "Flowers don’t grow themselves.",
            AchievementType.SpendFlower => "Flowers are meant to be spent.",
            AchievementType.EarnDiamond => "Premium feels different.",
            AchievementType.SpendDiamond => "Worth every diamond. Probably.",
            AchievementType.LoseHeart => "Failure is part of learning.",
            AchievementType.LoginDays => "Thanks for coming back.",
            AchievementType.CompleteQuest => "NPCs trust you now.",
            _ => "Achievement unlocked."
        };
    }

    #endregion
}
