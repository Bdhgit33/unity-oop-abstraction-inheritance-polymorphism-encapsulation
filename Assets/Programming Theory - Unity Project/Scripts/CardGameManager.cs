using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // for stopping play mode in Editor
#endif

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance { get; private set; }

    private readonly Queue<ImageCard> _revealQueue = new Queue<ImageCard>();

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pairsLeftText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalTimeText;
    public UnityEngine.UI.Button tryAgainButton;
    public UnityEngine.UI.Button quitButton;

    private float _elapsedTime = 0f;
    private bool _isTimerRunning = true;
    private int _pairsLeft;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(OnTryAgainPressed);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    private void Start()
    {
        _pairsLeft = 8;
        UpdatePairsLeftUI();
        _elapsedTime = 0f;
        _isTimerRunning = true;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void EnqueueRevealedCard(ImageCard card)
    {
        _revealQueue.Enqueue(card);
        Debug.Log($"Enqueued CardID={card.CardID}. Queue size: {_revealQueue.Count}");

        if (_revealQueue.Count >= 2)
        {
            ImageCard first = _revealQueue.Dequeue();
            ImageCard second = _revealQueue.Dequeue();
            StartCoroutine(CheckPairCoroutine(first, second));
        }
    }

    private IEnumerator CheckPairCoroutine(ImageCard first, ImageCard second)
    {
        Debug.Log($"Comparing {first.CardID} vs {second.CardID}");

        if (first.CardID == second.CardID)
        {
            Debug.Log("Matching Cards Found!");
            _pairsLeft--;
            UpdatePairsLeftUI();

            if (_pairsLeft <= 0)
            {
                GameOver();
            }
        }
        else
        {
            Debug.Log("Not a match. Hiding in 1 second...");
            yield return new WaitForSeconds(1f);

            first.Hide();
            first.ReenableButton();
            second.Hide();
            second.ReenableButton();

            Debug.Log("Cards hidden and re-enabled.");
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {_elapsedTime:0.00}";
    }

    private void UpdatePairsLeftUI()
    {
        if (pairsLeftText != null)
            pairsLeftText.text = $"Pairs Left: {_pairsLeft}";
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

        _isTimerRunning = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalTimeText != null)
            finalTimeText.text = $"Final Time: {_elapsedTime:0.00}";
    }

    private void OnTryAgainPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Called by the “Add Score” button on the Game Over panel.
    /// Saves the player’s name & final time into PlayerPrefs ("HighScores"),
    /// then loads the HighScore scene (index 1).
    /// </summary>
    public void OnAddScoreButtonPressed()
    {
        // 1) Get player name
        string playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        // 2) Read existing JSON from PlayerPrefs
        string json = PlayerPrefs.GetString("HighScores", "");
        HighScoreData data;
        if (string.IsNullOrEmpty(json))
        {
            data = new HighScoreData();
        }
        else
        {
            data = JsonUtility.FromJson<HighScoreData>(json);
            if (data == null)
                data = new HighScoreData();
        }

        // 3) Append new entry
        data.entries.Add(new HighScoreEntry(playerName, _elapsedTime));

        // 4) Serialize back to JSON and save
        string newJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("HighScores", newJson);
        PlayerPrefs.Save();

        Debug.Log($"Saved HighScore: {playerName} - {_elapsedTime:0.00}");

        // 5) Load HighScore scene (index 1)
        SceneManager.LoadScene(1);
    }
}
