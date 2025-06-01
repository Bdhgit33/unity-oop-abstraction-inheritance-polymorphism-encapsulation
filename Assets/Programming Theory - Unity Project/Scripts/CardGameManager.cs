using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;                // needed for TextMeshProUGUI
using UnityEngine.SceneManagement; // for reloading the scene

#if UNITY_EDITOR
using UnityEditor;         // for stopping play mode in Editor
#endif

/// <summary>
/// Manages card matching logic, a running timer, and UI updates.
/// - Uses a queue to compare pairs in click‐order (with a 1-second delay for non-matches).
/// - Tracks elapsed time, pairs left, and shows a Game Over panel when done.
/// </summary>
public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance { get; private set; }

    // Queue of cards revealed but not yet compared
    private readonly Queue<ImageCard> _revealQueue = new Queue<ImageCard>();

    // UI references (assign these in the Inspector)
    [Header("UI Elements")]
    [Tooltip("TextMeshProUGUI component that displays the timer (e.g. 'Time: 0.00').")]
    public TextMeshProUGUI timerText;

    [Tooltip("TextMeshProUGUI component that displays how many pairs remain (e.g. 'Pairs Left: 8').")]
    public TextMeshProUGUI pairsLeftText;

    [Tooltip("Game Over panel to show when all pairs are found.")]
    public GameObject gameOverPanel;

    [Tooltip("TextMeshProUGUI under GameOverPanel to show final time.")]
    public TextMeshProUGUI finalTimeText;

    [Tooltip("Button under GameOverPanel that restarts the game.")]
    public UnityEngine.UI.Button tryAgainButton;

    [Tooltip("Button under GameOverPanel that quits the game.")]
    public UnityEngine.UI.Button quitButton;

    // Internal state
    private float _elapsedTime = 0f;
    private bool _isTimerRunning = true;
    private int _pairsLeft;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Another CardGameManager already exists! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Ensure Game Over panel is hidden at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Hook up button callbacks
        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(OnTryAgainPressed);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    private void Start()
    {
        // Initialize pairs left (should match pairsCount in CardTableController)
        _pairsLeft = 8;
        UpdatePairsLeftUI();

        // Initialize timer text
        _elapsedTime = 0f;
        _isTimerRunning = true;
        UpdateTimerUI();
    }

    private void Update()
    {
        // Advance the timer if game is not over
        if (_isTimerRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// Called by ImageCard.Reveal() to queue up a newly revealed card.
    /// </summary>
    public void EnqueueRevealedCard(ImageCard card)
    {
        // Add to queue
        _revealQueue.Enqueue(card);
        Debug.Log($"CardGameManager: Enqueued CardID={card.CardID}. Queue size: {_revealQueue.Count}");

        // If two cards in queue, dequeue them and start comparison
        if (_revealQueue.Count >= 2)
        {
            ImageCard first = _revealQueue.Dequeue();
            ImageCard second = _revealQueue.Dequeue();
            StartCoroutine(CheckPairCoroutine(first, second));
        }
    }

    /// <summary>
    /// Compares a pair of cards:
    /// - If they match: decrement pairs left, update UI, and check for game over.
    /// - If they don't match: wait 1 second, then flip them back and re‐enable their buttons.
    /// </summary>
    private IEnumerator CheckPairCoroutine(ImageCard first, ImageCard second)
    {
        Debug.Log($"Comparing CardID={first.CardID} vs CardID={second.CardID}");

        if (first.CardID == second.CardID)
        {
            Debug.Log("CardGameManager: Matching Cards Found!");
            // Decrement pairs left and update UI
            _pairsLeft--;
            UpdatePairsLeftUI();

            // Check for Game Over
            if (_pairsLeft <= 0)
            {
                GameOver();
            }
            // Both cards remain face-up, buttons already disabled in Reveal()
        }
        else
        {
            Debug.Log("CardGameManager: Not a match! Hiding in 1 second...");
            yield return new WaitForSeconds(1f);

            // Hide and re-enable both cards
            first.Hide();
            first.ReenableButton();

            second.Hide();
            second.ReenableButton();

            Debug.Log("CardGameManager: Cards hidden and re-enabled.");
        }
    }

    /// <summary>
    /// Updates the TimerText UI with the current elapsed time in seconds (two decimals).
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {_elapsedTime:0.00}";
        }
    }

    /// <summary>
    /// Updates the PairsLeftText UI with how many pairs remain.
    /// </summary>
    private void UpdatePairsLeftUI()
    {
        if (pairsLeftText != null)
        {
            pairsLeftText.text = $"Pairs Left: {_pairsLeft}";
        }
    }

    /// <summary>
    /// Called once when _pairsLeft reaches 0. Stops the timer and shows Game Over panel.
    /// </summary>
    private void GameOver()
    {
        Debug.Log("CardGameManager: Game Over!");

        // Stop the timer
        _isTimerRunning = false;

        // Show Game Over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Display final time
        if (finalTimeText != null)
            finalTimeText.text = $"Final Time: {_elapsedTime:0.00}";
    }

    /// <summary>
    /// Called by Try Again button. Reloads the current scene to restart the game.
    /// </summary>
    private void OnTryAgainPressed()
    {
        Debug.Log("CardGameManager: Try Again pressed. Reloading scene.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Called by Quit button. Quits the application (and stops Play Mode in Editor).
    /// </summary>
    private void OnQuitPressed()
    {
        Debug.Log("CardGameManager: Quit pressed. Exiting.");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
