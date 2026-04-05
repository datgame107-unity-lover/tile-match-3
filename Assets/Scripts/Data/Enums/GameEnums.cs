
public enum GameState
{
    Playing,
    Pause,
    Lose,
    Win,
    Creating,
}

public enum GameMode
{
    Level,
    EndLess,
    Daily,
}

public enum CurrencyType
{
    Flower,
    Diamond,
    Heart,
    Shuffle,
    Hint,
    Undo,
    PowerUp,
}

public enum ShopItemType
{
    Currency,
    Ability,
    Bundle,
}

public enum AchievementType
{
    ClearLevel,
    MatchTiles,
    UseBooster,
    EarnFlower,
    EarnDiamond,
    LoseHeart,
    HighScore,
    CompleteQuest,
}

public enum QuestType
{
    CollectItem,
    CompleteLevels,
    UseAbility,
}

public enum WarningType
{
    Delete,
    Confirm,
}

public enum SceneEnum
{
    Home,
    GameScene,
    Loading,
    LevelEditor,
}

public enum SoundID
{
    Click,
    TileRemove,
    Win,
    Lose,
    ClaimQuest,
    BuyItem,
    Success,
}