using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static bool shouldStartImmediately = false;

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private GameObject hudCanvas;

    [Header("Shield Visual Indicator")]
    [SerializeField] private SpriteRenderer yurtShieldVisual;
    [ColorUsage(true, true)] [SerializeField] private Color oneHitShieldColor = new Color(1f, 0.85f, 0.55f, 1f);
    [ColorUsage(true, true)] [SerializeField] private Color timedShieldColor = new Color(0.6f, 0.88f, 1f, 1f);

    [Header("Post-Processing & Camera Shake")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float damageVignetteIntensity = 0.45f;
    [SerializeField] private float damageFlashDuration = 0.4f;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeMagnitude = 0.2f;

    [Header("Heart UI")]
    [SerializeField] private UnityEngine.UI.Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Lives { get; private set; } = 3;

    public bool HasTimedShield { get; private set; } = false;
    public bool HasOneHitShield { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;

    private Coroutine timedShieldCoroutine;
    private Coroutine damageFlashCoroutine;
    private Coroutine cameraShakeCoroutine;
    private Vignette vignette;
    private Vector3 originalCameraPos;

    private void UpdateHearts(int currentLives)
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
                heartImages[i].sprite = (i < currentLives) ? fullHeartSprite : emptyHeartSprite;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        HighScore = PlayerPrefs.GetInt("SteppeDefenseHighScore", 0);

        if (globalVolume != null && globalVolume.sharedProfile != null)
        {
            globalVolume.sharedProfile.TryGet(out vignette);
        }
    }

    private void Start()
    {
        UpdateUI();
        if (yurtShieldVisual != null) yurtShieldVisual.enabled = false;
        if (vignette != null) vignette.intensity.value = 0f;
        if (Camera.main != null) originalCameraPos = Camera.main.transform.localPosition;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        IsPaused = false;

        if (shouldStartImmediately)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (hudCanvas != null) hudCanvas.SetActive(true);
            Time.timeScale = 1f;
            shouldStartImmediately = false;
        }
        else
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        UpdateHearts(Lives);
    }

    private void Update()
    {
        bool isMenuOpen = (mainMenuPanel != null && mainMenuPanel.activeSelf)
                       || (tutorialPanel != null && tutorialPanel.activeSelf)
                       || (gameOverPanel != null && gameOverPanel.activeSelf);

        if (!isMenuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        IsPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ReturnToMainMenu()
    {
        IsPaused = false;
        shouldStartImmediately = false;
        Time.timeScale = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickStartGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (hudCanvas != null) hudCanvas.SetActive(true);
        Time.timeScale = 1f;
    }

    public void OnClickOpenTutorial()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    public void OnClickBackToMenu()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void RestartGame()
    {
        shouldStartImmediately = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void AddScore(int amount)
    {
        Score += amount;
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("SteppeDefenseHighScore", HighScore);
            PlayerPrefs.Save();
        }
        UpdateUI();
    }

    public bool TakeDamage(int amount)
    {
        if (HasTimedShield) return true;

        if (HasOneHitShield)
        {
            HasOneHitShield = false;
            if (yurtShieldVisual != null) yurtShieldVisual.enabled = false;
            return true;
        }

        Lives -= amount;
        UpdateHearts(Lives);
        UpdateUI();
        TriggerDamageFlash();
        TriggerCameraShake();

        if (Lives <= 0)
        {
            GameOver();
        }
        return false;
    }

    private void TriggerDamageFlash()
    {
        if (vignette == null) return;
        if (damageFlashCoroutine != null) StopCoroutine(damageFlashCoroutine);
        damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (vignette == null) yield break;
        vignette.intensity.value = damageVignetteIntensity;
        float elapsed = 0f;

        while (elapsed < damageFlashDuration)
        {
            if (vignette == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            vignette.intensity.value = Mathf.Lerp(damageVignetteIntensity, 0f, elapsed / damageFlashDuration);
            yield return null;
        }

        if (vignette != null) vignette.intensity.value = 0f;
        damageFlashCoroutine = null;
    }

    private void TriggerCameraShake()
    {
        if (Camera.main == null) return;
        if (cameraShakeCoroutine != null) StopCoroutine(cameraShakeCoroutine);
        cameraShakeCoroutine = StartCoroutine(CameraShakeRoutine());
    }

    private IEnumerator CameraShakeRoutine()
    {
        Transform camTransform = Camera.main.transform;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            if (camTransform == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            float damping = 1f - (elapsed / shakeDuration);
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude * damping;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude * damping;

            camTransform.localPosition = new Vector3(originalCameraPos.x + offsetX, originalCameraPos.y + offsetY, originalCameraPos.z);
            yield return null;
        }

        if (camTransform != null) camTransform.localPosition = originalCameraPos;
        cameraShakeCoroutine = null;
    }

    public void ActivateOneHitShield()
    {
        HasOneHitShield = true;
        if (yurtShieldVisual != null)
        {
            yurtShieldVisual.enabled = true;
            yurtShieldVisual.material.SetColor("_GlowColor", oneHitShieldColor);
        }
    }

    public void ActivateTimedShield(float duration)
    {
        if (timedShieldCoroutine != null) StopCoroutine(timedShieldCoroutine);
        timedShieldCoroutine = StartCoroutine(TimedShieldRoutine(duration));
    }

    private IEnumerator TimedShieldRoutine(float duration)
    {
        HasTimedShield = true;

        if (yurtShieldVisual != null)
        {
            yurtShieldVisual.enabled = true;
            yurtShieldVisual.material.SetColor("_GlowColor", timedShieldColor);
        }

        float solidTime = Mathf.Max(0f, duration - 3.0f);
        yield return new WaitForSecondsRealtime(solidTime);

        for (int i = 0; i < 6; i++)
        {
            if (yurtShieldVisual != null)
                yurtShieldVisual.enabled = !yurtShieldVisual.enabled;

            yield return new WaitForSecondsRealtime(0.25f);
        }

        HasTimedShield = false;
        if (yurtShieldVisual != null) yurtShieldVisual.enabled = false;
        timedShieldCoroutine = null;
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {Score}";
        if (livesText != null) livesText.text = $"Lives: {Lives}";
        if (highScoreText != null) highScoreText.text = $"Highscore: {HighScore}";
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = $"Final Score: {Score}";
            if (gameOverHighScoreText != null) gameOverHighScoreText.text = $"Best: {HighScore}";
        }
        AudioManager.Instance?.StopBGM();
        AudioManager.Instance?.PlayGameOver();
    }
}