// Scripts/Core/GameBootstrapper.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Currency Config")]
    public int maxHeart = 5;
    public float heartRegenTime = 30f;
    public bool resetLevel = false;

    [Header("Sound Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Shop")]
    public List<ShopItemSO> shopItems;

    [Header("Sound Clips")]
    public List<AudioClip> backgroundMusics;
    public AudioClip clickClip;
    public AudioClip tileRemoveClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip claimQuestClip;
    public AudioClip buyItemClip;
    public AudioClip successClip;

    [Header("Progress Config")]
    public List<QuestDataSO> dailyQuests;
    public List<QuestReward> dailyQuestRewards;
    public List<QuestReward> dailyWinRewards;
    public List<AchievementDataSO> achievements;
    public int dailyWinTarget = 5;

    [Header("Tile datas")]
    [SerializeField] private TileDatabaseSO tileDatabase;
    private CurrencyService currencyService;

    [Header("Grid config")]
    [SerializeField] private GridConfig gridConfig;
    private static bool initialized;

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);

        Bootstrap();
        if (resetLevel)
        {
            ServiceLocator.Get<GameStateService>().SaveLevel(1);

        }
    }

    private void Bootstrap()
    {

        // 1. Infrastructure
        var saveService = new PlayerPrefsSaveService();
        ServiceLocator.Register<ISaveService>(saveService);
        ServiceLocator.Register<ILeaderboardService>(new NullLeaderboardService());
        ServiceLocator.Register(new LevelBuilder());
        // 2. Application services
        var gameState = new GameStateService(saveService);
        ServiceLocator.Register<GameStateService>(gameState);

        currencyService = new CurrencyService(saveService, maxHeart, heartRegenTime);
        ServiceLocator.Register<CurrencyService>(currencyService);

        var soundService = new SoundService(saveService);
        ServiceLocator.Register<SoundService>(soundService);


        var scoreService = new ScoreService(saveService);
        ServiceLocator.Register<ScoreService>(scoreService);

        var shopService = new ShopService(shopItems, currencyService);
        ServiceLocator.Register<ShopService>(shopService);

        var iapService = new IAPService();
        ServiceLocator.Register<IAPService>(iapService);

        var progressService = new ProgressService(saveService, currencyService, dailyQuests, dailyQuestRewards, dailyWinRewards, achievements, dailyWinTarget);
        ServiceLocator.Register<ProgressService>(progressService);
        var jsonService = new LevelJsonService();
        ServiceLocator.Register(jsonService);

        var levels = jsonService.LoadAll();
        ServiceLocator.Register<GridConfig>(gridConfig);
        ServiceLocator.Register(
            new LevelDataManager(levels));
        tileDatabase.Init();
        ServiceLocator.Register(tileDatabase);
        // 3. Wire sound sources + clips
        soundService.SetAudioSources(musicSource, sfxSource);
        soundService.RegisterClip(SoundID.Click, clickClip);
        soundService.RegisterClip(SoundID.TileRemove, tileRemoveClip);
        soundService.RegisterClip(SoundID.Win, winClip);
        soundService.RegisterClip(SoundID.Lose, loseClip);
        soundService.RegisterClip(SoundID.ClaimQuest, claimQuestClip);
        soundService.RegisterClip(SoundID.BuyItem, buyItemClip);
        soundService.RegisterClip(SoundID.Success, successClip);
        soundService.RegisterBackgroundMusics(backgroundMusics);

        // 4. Subscribe game-level events
        EventBus<PlayerLostEvent>.Subscribe(OnPlayerLost);
        EventBus<ShuffleUsedEvent>.Subscribe(_ =>
            currencyService.Spend(CurrencyType.Shuffle, 1));
        EventBus<HintUsedEvent>.Subscribe(_ =>
            currencyService.Spend(CurrencyType.Hint, 1));
        EventBus<UndoUsedEvent>.Subscribe(_ =>
            currencyService.Spend(CurrencyType.Undo, 1));
        EventBus<PowerUpUsedEvent>.Subscribe(_ =>
            currencyService.Spend(CurrencyType.PowerUp, 1));

        print(saveService);
    }
    private async void Start()
    {
        var shop = ServiceLocator.Get<ShopService>(); 
        var iap = ServiceLocator.Get<IAPService>();
        await iap.Initialize(shop);
    }
    private void Update()
    {
        currencyService?.Tick(Time.deltaTime);
        
    }

    private void OnPlayerLost(PlayerLostEvent _)
    {
        currencyService.Spend(CurrencyType.Heart, 1);
    }

    private void OnDestroy()
    {
        EventBus<PlayerLostEvent>.Unsubscribe(OnPlayerLost);
    }
}