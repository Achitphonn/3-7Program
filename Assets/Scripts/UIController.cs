using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("HUD")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI modeText;

    [Header("Start Panel")]
    public TextMeshProUGUI bestEasyText;
    public TextMeshProUGUI bestHardText;
    public TextMeshProUGUI bestProgText;

    [Header("Game Over Panel")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI bestThisModeText;

    [Header("Lives (Hearts)")]
    public Image[] heartImages; // assign 3 heart Images in order

    [Header("Cooldown (Bottom Right)")]
    public Image cooldownFill;  // Image set to Filled/Radial360 using the smoke cloud icon
    public Image cooldownIcon;  // Optional: base icon under the fill (static)

    Color readyColor = Color.white;
    Color coolingColor = new Color(1f, 1f, 1f, 0.35f);

    // ======= Show/Hide =======
    public void ShowStart(float bestEasy, float bestHard, float bestProg)
    {
        if (startPanel) startPanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (bestEasyText) bestEasyText.text = $"Best time (Easy): {bestEasy:F2}s";
        if (bestHardText) bestHardText.text = $"Best time (Hard): {bestHard:F2}s";
        if (bestProgText) bestProgText.text = $"Best time (Progressive): {bestProg:F2}s";

        UpdateCooldown(0f, 1f, false);
        UpdateLives(3, 3);
    }

    public void ShowHUD()
    {
        if (startPanel) startPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        UpdateHUD(0f, "—");
    }

    public void ShowGameOver(float lastTime, string lastMode, float bestThisMode)
    {
        if (startPanel) startPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);

        if (resultText) resultText.text = $"You survived {lastTime:F2} s ({lastMode}).";
        if (bestThisModeText) bestThisModeText.text = $"Best time in {lastMode}: {bestThisMode:F2}s";
    }

    
    public void UpdateHUD(float time, string mode)
    {
        if (timeText) timeText.text = $"Time: {time:F2}s";
        if (modeText) modeText.text = $"Mode: {mode}";
    }

    public void UpdateLives(int lives, int maxLives)
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (!heartImages[i]) continue;
            heartImages[i].enabled = i < lives;
        }
    }

    // remaining = seconds left on cooldown, duration = total cooldown seconds
    public void UpdateCooldown(float remaining, float duration, bool isActiveNow)
    {
        if (!cooldownFill) return;
        float frac = 0f;
        if (duration > 0f) frac = Mathf.Clamp01(remaining / duration);
        cooldownFill.fillAmount = frac;

        // dim icon when cooling down
        if (cooldownIcon)
        {
            cooldownIcon.color = (remaining > 0f) ? coolingColor : readyColor;
        }
    }

   
   
    public void OnStartEasy() => GameManager.Instance.StartGame(DifficultyMode.Easy);
    public void OnStartHard() => GameManager.Instance.StartGame(DifficultyMode.Hard);
    public void OnStartProgressive() => GameManager.Instance.StartGame(DifficultyMode.Progressive);

    
    public void OnRetry() => GameManager.Instance.RetrySameMode();
    public void OnBackToStart() => GameManager.Instance.GoToStartMenu();
    public void OnGoEasy() => GameManager.Instance.ChangeDifficultyAndStart(DifficultyMode.Easy);
    public void OnGoHard() => GameManager.Instance.ChangeDifficultyAndStart(DifficultyMode.Hard);
    public void OnGoProgressive() => GameManager.Instance.ChangeDifficultyAndStart(DifficultyMode.Progressive);
}
