using UnityEngine;
using UnityEngine.InputSystem.XR;

public enum DifficultyMode { Easy, Hard, Progressive }
public enum GameState { StartMenu, Playing, GameOver }

[System.Serializable]
public class DifficultyConfig
{
    public string name;
    public float startObstacleSpeed = 4f;
    public float startSpawnRate = 1.0f; // seconds between spawns (lower = harder)
    public float speedIncreasePerSecond = 0f;      // Progressive only
    public float spawnRateDecreasePerSecond = 0f;  // Progressive only
    public float minSpawnRate = 0.25f;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public Spawner spawner;
    public UIController ui;

    [Header("Difficulty Presets")]
    public DifficultyConfig easy;
    public DifficultyConfig hard;
    public DifficultyConfig progressive;

    [Header("Player & Lives")]
    public int maxLives = 3;
    public Vector3 spawnPosition = new Vector3(0f, -3f, 0f);

    // Runtime
    public bool IsPlaying => State == GameState.Playing;
    public GameState State { get; private set; } = GameState.StartMenu;
    public DifficultyMode CurrentMode { get; private set; } = DifficultyMode.Easy;

    public float CurrentObstacleSpeed { get; private set; }
    public float CurrentSpawnRate { get; private set; }
    public float SurvivalTime { get; private set; }
    public float BestTimeEasy { get; private set; }
    public float BestTimeHard { get; private set; }
    public float BestTimeProg { get; private set; }

    int lives;
    PlayerAbility ability;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Application.targetFrameRate = 120;
        Time.timeScale = 1f;

        LoadBestTimes();
        GoToStartMenu();
    }

    void Update()
    {
        if (!IsPlaying) return;

        SurvivalTime += Time.deltaTime;
        ui.UpdateHUD(SurvivalTime, CurrentMode.ToString());

        // Update cooldown UI if we have ability
        if (ability != null)
        {
            ui.UpdateCooldown(ability.CooldownRemaining, ability.CooldownDuration, ability.IsInvulnerable);
        }

        if (CurrentMode == DifficultyMode.Progressive)
        {
            CurrentObstacleSpeed += progressive.speedIncreasePerSecond * Time.deltaTime;
            CurrentSpawnRate -= progressive.spawnRateDecreasePerSecond * Time.deltaTime;
            CurrentSpawnRate = Mathf.Max(CurrentSpawnRate, progressive.minSpawnRate);
        }
    }

    // === State handling ===
    public void GoToStartMenu()
    {
        State = GameState.StartMenu;
        Time.timeScale = 1f;
        ClearObstacles();
        ResetPlayerPosition();
        ui.ShowStart(BestTimeEasy, BestTimeHard, BestTimeProg);
        ui.UpdateLives(maxLives, maxLives);
        ui.UpdateCooldown(0f, 1f, false);
        spawner.StopSpawning();
    }

    public void StartGame(DifficultyMode mode)
    {
        CurrentMode = mode;
        ApplyDifficulty(mode);
        SurvivalTime = 0f;
        lives = maxLives;
        ui.UpdateLives(lives, maxLives);

        State = GameState.Playing;
        ui.ShowHUD();
        spawner.StartSpawning();
    }

    public void GameOver()
    {
        if (!IsPlaying) return;

        State = GameState.GameOver;
        spawner.StopSpawning();
        SaveBestTimeIfNeeded(CurrentMode, SurvivalTime);
        ui.ShowGameOver(SurvivalTime, CurrentMode.ToString(), GetBestTimeFor(CurrentMode));
    }

    public void RetrySameMode()
    {
        ClearObstacles();
        ResetPlayerPosition();
        StartGame(CurrentMode);
    }

    public void ChangeDifficultyAndStart(DifficultyMode mode)
    {
        ClearObstacles();
        ResetPlayerPosition();
        StartGame(mode);
    }

    public void TryHitPlayer()
    {
        // Lazy fetch ability if missing
        if (ability == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) ability = player.GetComponent<PlayerAbility>();
        }

        if (ability != null && ability.IsInvulnerable)
        {
            // ignore hit
            return;
        }

        // normal hit
        lives = Mathf.Max(0, lives - 1);
        ui.UpdateLives(lives, maxLives);

        if (lives <= 0) GameOver();
    }

    // === Helpers ===
    void ApplyDifficulty(DifficultyMode mode)
    {
        DifficultyConfig cfg = GetConfig(mode);
        CurrentObstacleSpeed = cfg.startObstacleSpeed;
        CurrentSpawnRate = cfg.startSpawnRate;
    }

    DifficultyConfig GetConfig(DifficultyMode mode)
    {
        switch (mode)
        {
            case DifficultyMode.Easy: return easy;
            case DifficultyMode.Hard: return hard;
            case DifficultyMode.Progressive: return progressive;
        }
        return easy;
    }

    void ClearObstacles()
    {
        var all = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var ob in all) Destroy(ob);
    }

    void ResetPlayerPosition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.transform.position = spawnPosition;
            ability = player.GetComponent<PlayerAbility>();
        }
    }

    void LoadBestTimes()
    {
        BestTimeEasy = PlayerPrefs.GetFloat("Best_Easy", 0f);
        BestTimeHard = PlayerPrefs.GetFloat("Best_Hard", 0f);
        BestTimeProg = PlayerPrefs.GetFloat("Best_Prog", 0f);
    }

    void SaveBestTimeIfNeeded(DifficultyMode mode, float time)
    {
        string key = mode == DifficultyMode.Easy ? "Best_Easy" :
                     mode == DifficultyMode.Hard ? "Best_Hard" : "Best_Prog";

        float prev = PlayerPrefs.GetFloat(key, 0f);
        if (time > prev)
        {
            PlayerPrefs.SetFloat(key, time);
            PlayerPrefs.Save();
            if (mode == DifficultyMode.Easy) BestTimeEasy = time;
            else if (mode == DifficultyMode.Hard) BestTimeHard = time;
            else BestTimeProg = time;
        }
    }

    float GetBestTimeFor(DifficultyMode mode)
    {
        return mode == DifficultyMode.Easy ? BestTimeEasy :
               mode == DifficultyMode.Hard ? BestTimeHard : BestTimeProg;
    }
}






