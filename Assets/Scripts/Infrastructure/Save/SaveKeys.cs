public static class SaveKeys
{
    // =========================
    // PLAYER
    // =========================
    public static class Player
    {
        public const string Level = "player.level";
        public const string NoAds = "player.no_ads";
    }

    // =========================
    // AUDIO
    // =========================
    public static class Audio
    {
        public const string MusicOn = "audio.music_on";
        public const string SfxOn = "audio.sfx_on";
        public const string VibrationOn = "audio.vibration_on";
    }

    // =========================
    // CURRENCY
    // =========================
    public static class Currency
    {
        public static string Key(CurrencyType t)
            => $"currency.{t}";
    }

    // =========================
    // HEART SYSTEM
    // =========================
    public static class Heart
    {
        public const string RegenTimer = "heart.regen_timer";
    }

    // =========================
    // QUEST
    // =========================
    public static class Quest
    {
        public const string Progress = "quest.progress";
        public const string Day = "quest.day";
    }

    // =========================
    // ACHIEVEMENT
    // =========================
    public static class Achievement
    {
        public const string Progress = "achievement.progress";
    }

    // =========================
    // GAME MODE
    // =========================
    public static class EndLess
    {
        public const string HighScore = "endless.high_score";
    }

    // =========================
    // EDITOR
    // =========================
    public static class Editor
    {
        public const string LastEditedLevel = "editor.last_level";
        public const string MaxLevel = "editor.max_level";
    }
}