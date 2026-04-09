using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class QuestItemGenerator : EditorWindow
{
    private class QuestSetup
    {
        public string id, name;
        public QuestType type;
        public int targetAmount;
        public string targetTileId; // Dùng để map với TileDataSO
        public List<QuestReward> rewards = new List<QuestReward>();
    }

    [MenuItem("Tools/Generate Quest & Tile Data")]
    public static void GenerateQuests()
    {
        // 1. Tạo thư mục
        CreateFolder("Assets/ScriptableObjects");
        CreateFolder("Assets/ScriptableObjects/Tiles");
        CreateFolder("Assets/ScriptableObjects/Quests");

    

        // 3. DANH SÁCH 12 QUEST MẪU
        List<QuestSetup> questDatabase = new List<QuestSetup>
        {
            // Complete Levels
            new QuestSetup { id = "Q_LV_01", name = "Vượt 1 màn chơi", type = QuestType.CompleteLevels, targetAmount = 1,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Flower, amount = 50 } } },
            new QuestSetup { id = "Q_LV_02", name = "Vượt 3 màn chơi", type = QuestType.CompleteLevels, targetAmount = 3,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Diamond, amount = 10 } } },
            new QuestSetup { id = "Q_LV_03", name = "Vượt 5 màn chơi", type = QuestType.CompleteLevels, targetAmount = 5,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Hint, amount = 1 } } },
            new QuestSetup { id = "Q_LV_04", name = "Vượt 10 màn chơi", type = QuestType.CompleteLevels, targetAmount = 10,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.PowerUp, amount = 1 } } },

            // Collect Items (Cần map TargetTileId)
            new QuestSetup { id = "Q_ITEM_01", name = "Thu thập Táo", type = QuestType.CollectItem, targetAmount = 50, targetTileId = "Apple",
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Flower, amount = 100 } } },
            new QuestSetup { id = "Q_ITEM_02", name = "Thu thập Lá Cây", type = QuestType.CollectItem, targetAmount = 100, targetTileId = "Leaf",
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Diamond, amount = 5 } } },
            new QuestSetup { id = "Q_ITEM_03", name = "Thu thập Nước", type = QuestType.CollectItem, targetAmount = 200, targetTileId = "Water",
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Undo, amount = 1 } } },
            new QuestSetup { id = "Q_ITEM_04", name = "Thu thập Nấm", type = QuestType.CollectItem, targetAmount = 300, targetTileId = "Mushroom",
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Shuffle, amount = 1 } } },

            // Use Ability
            new QuestSetup { id = "Q_USE_01", name = "Dùng 1 trợ giúp", type = QuestType.UseAbility, targetAmount = 1,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Flower, amount = 30 } } },
            new QuestSetup { id = "Q_USE_02", name = "Dùng 3 trợ giúp", type = QuestType.UseAbility, targetAmount = 3,
                rewards = new List<QuestReward> { new QuestReward { type = CurrencyType.Diamond, amount = 5 } } },
            new QuestSetup { id = "Q_USE_03", name = "Dùng 5 trợ giúp", type = QuestType.UseAbility, targetAmount = 5,
                rewards = new List<QuestReward> {
                    new QuestReward { type = CurrencyType.Flower, amount = 50 },
                    new QuestReward { type = CurrencyType.Diamond, amount = 5 }
                } },
            new QuestSetup { id = "Q_USE_04", name = "Dùng 10 trợ giúp", type = QuestType.UseAbility, targetAmount = 10,
                rewards = new List<QuestReward> {
                    new QuestReward { type = CurrencyType.Hint, amount = 1 },
                    new QuestReward { type = CurrencyType.Shuffle, amount = 1 }
                } },
        };

        // 4. TẠO QUEST DATA MẪU
        foreach (var data in questDatabase)
        {
            QuestDataSO newQuest = ScriptableObject.CreateInstance<QuestDataSO>();

            newQuest.questID = data.id;
            newQuest.questName = data.name;
            newQuest.type = data.type;
            newQuest.targetAmount = data.targetAmount;
            newQuest.rewards = data.rewards;

        

            string assetPath = $"Assets/ScriptableObjects/Quests/{data.id}.asset";
            AssetDatabase.CreateAsset(newQuest, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=green>Đã tạo thành công 4 Tile mẫu và 12 Quest mẫu tại thư mục Assets/Data!</color>");
    }

    private static void CreateFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = path.Substring(0, path.LastIndexOf('/'));
            string folderName = path.Substring(path.LastIndexOf('/') + 1);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}