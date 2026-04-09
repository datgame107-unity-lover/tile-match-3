using UnityEngine;
using UnityEditor;

public class Match3AchievementGenerator : EditorWindow
{
    private class AchievementSetupData
    {
        public string id, title, description;
        public AchievementType type;
        public int target;
        public CurrencyType rewardType;
        public int rewardAmount;

        public AchievementSetupData(string id, string title, string desc, AchievementType type, int target, CurrencyType rType, int rAmount)
        {
            this.id = id;
            this.title = title;
            this.description = desc;
            this.type = type;
            this.target = target;
            this.rewardType = rType;
            this.rewardAmount = rAmount;
        }
    }

    [MenuItem("Tools/Generate Achievements")]
    public static void GenerateExactAchievements()
    {
        string folderPath = "Assets/ScriptableObjects/Achievements";

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Achievements"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Achievements");

        AchievementSetupData[] database = new AchievementSetupData[]
        {
            new AchievementSetupData("ACH_001", "Bắt đầu hành trình", "Vượt qua 10 màn chơi.", AchievementType.ClearLevel, 10, CurrencyType.Flower, 500),
            new AchievementSetupData("ACH_002", "Người phá đảo", "Vượt qua 50 màn chơi.", AchievementType.ClearLevel, 50, CurrencyType.Flower, 5),
            new AchievementSetupData("ACH_003", "Bậc thầy giải đố", "Vượt qua 150 màn chơi.", AchievementType.ClearLevel, 150, CurrencyType.Diamond, 50),

            new AchievementSetupData("ACH_004", "Tinh mắt", "Ghép thành công 100 khối.", AchievementType.MatchTiles, 100, CurrencyType.Flower, 200),
            new AchievementSetupData("ACH_005", "Bàn tay ma thuật", "Ghép thành công 2000 khối.", AchievementType.MatchTiles, 2000, CurrencyType.Flower, 1500),
            new AchievementSetupData("ACH_006", "Chuyên gia xếp hình", "Ghép thành công 10000 khối.", AchievementType.MatchTiles, 10000, CurrencyType.Diamond, 30),

            new AchievementSetupData("ACH_007", "Cần sự trợ giúp", "Sử dụng vật phẩm hỗ trợ 5 lần.", AchievementType.UseBooster, 5, CurrencyType.Flower, 100),
            new AchievementSetupData("ACH_008", "Thích nhờ vả", "Sử dụng vật phẩm hỗ trợ 50 lần.", AchievementType.UseBooster, 50, CurrencyType.Flower, 3),
            new AchievementSetupData("ACH_009", "Pháp sư", "Sử dụng vật phẩm hỗ trợ 200 lần.", AchievementType.UseBooster, 200, CurrencyType.Diamond, 20),

            new AchievementSetupData("ACH_010", "Yêu thiên nhiên", "Thu thập 50 bông hoa.", AchievementType.EarnFlower, 50, CurrencyType.Flower, 300),
            new AchievementSetupData("ACH_011", "Người làm vườn", "Thu thập 300 bông hoa.", AchievementType.EarnFlower, 300, CurrencyType.Flower, 5),
            new AchievementSetupData("ACH_012", "Cánh đồng hoa", "Thu thập 1000 bông hoa.", AchievementType.EarnFlower, 1000, CurrencyType.Diamond, 40),

            new AchievementSetupData("ACH_013", "Lấp lánh", "Kiếm được 10 kim cương.", AchievementType.EarnDiamond, 10, CurrencyType.Flower, 500),
            new AchievementSetupData("ACH_014", "Thợ mỏ", "Kiếm được 100 kim cương.", AchievementType.EarnDiamond, 100, CurrencyType.Flower, 10),
            new AchievementSetupData("ACH_015", "Kho báu hoàng gia", "Kiếm được 500 kim cương.", AchievementType.EarnDiamond, 500, CurrencyType.Diamond, 100),

            new AchievementSetupData("ACH_016", "Vấp ngã", "Đánh mất 5 trái tim.", AchievementType.LoseHeart, 5, CurrencyType.Flower, 200),
            new AchievementSetupData("ACH_017", "Đứng lên từ thất bại", "Đánh mất 50 trái tim.", AchievementType.LoseHeart, 50, CurrencyType.Flower, 2),
            new AchievementSetupData("ACH_018", "Tinh thần thép", "Đánh mất 200 trái tim.", AchievementType.LoseHeart, 200, CurrencyType.Diamond, 20),

            new AchievementSetupData("ACH_019", "Ghi danh", "Đạt tổng cộng 10,000 điểm.", AchievementType.HighScore, 10000, CurrencyType.Flower, 500),
            new AchievementSetupData("ACH_020", "Phá kỷ lục", "Đạt tổng cộng 100,000 điểm.", AchievementType.HighScore, 100000, CurrencyType.Flower, 5),
            new AchievementSetupData("ACH_021", "Vô tiền khoáng hậu", "Đạt tổng cộng 500,000 điểm.", AchievementType.HighScore, 500000, CurrencyType.Diamond, 50),

            new AchievementSetupData("ACH_022", "Chăm chỉ", "Hoàn thành 10 nhiệm vụ.", AchievementType.CompleteQuest, 10, CurrencyType.Flower, 500),
            new AchievementSetupData("ACH_023", "Đáng tin cậy", "Hoàn thành 50 nhiệm vụ.", AchievementType.CompleteQuest, 50, CurrencyType.Flower, 5),
            new AchievementSetupData("ACH_024", "Kẻ cuồng việc", "Hoàn thành 150 nhiệm vụ.", AchievementType.CompleteQuest, 150, CurrencyType.Diamond, 50)
        };

        // Chạy vòng lặp để tạo file Asset
        foreach (var data in database)
        {
            AchievementDataSO newAchievement = ScriptableObject.CreateInstance<AchievementDataSO>();

            newAchievement.id = data.id;
            newAchievement.title = data.title;
            newAchievement.description = data.description;
            newAchievement.type = data.type;
            newAchievement.target = data.target;
            newAchievement.rewardType = data.rewardType;
            newAchievement.rewardAmount = data.rewardAmount;
            newAchievement.icon = null; // Cần kéo thả icon thủ công sau

            // Lưu file với tên là ID (VD: ACH_001.asset)
            string assetPath = $"{folderPath}/{data.id}.asset";
            AssetDatabase.CreateAsset(newAchievement, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=cyan>Đã tạo thành công 24 Achievement Match-3 tại: {folderPath}</color>");
    }
}