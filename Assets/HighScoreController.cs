using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Populates the HighScore ScrollView with rows for each saved high‐score entry,
/// loaded from PlayerPrefs key "HighScores" (JSON).
/// </summary>
public class HighScoreController : MonoBehaviour
{
    [Header("UI & Prefab References")]
    [Tooltip("Drag the Content GameObject of the ScrollView here")]
    public Transform contentParent;

    [Tooltip("Drag your ScoreRowPrefab here (with NameField & TimeField children)")]
    public GameObject scoreRowPrefab;

    private void Start()
    {
        // Clear any existing rows under contentParent
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 1) Read the JSON string from PlayerPrefs
        string json = PlayerPrefs.GetString("HighScores", "");

        HighScoreData data;
        if (string.IsNullOrEmpty(json))
        {
            // No highscores saved yet
            data = new HighScoreData();
        }
        else
        {
            data = JsonUtility.FromJson<HighScoreData>(json);
            if (data == null) data = new HighScoreData();
        }

        // 2) Sort ascending by timeSeconds
        data.entries.Sort((a, b) => a.timeSeconds.CompareTo(b.timeSeconds));

        // 3) Instantiate one row per entry
        foreach (HighScoreEntry entry in data.entries)
        {
            GameObject row = Instantiate(scoreRowPrefab, contentParent);
            row.name = $"HighScore_{entry.playerName}_{entry.timeSeconds:0.00}";

            // Find the child TMP fields under this row:
            TextMeshProUGUI nameText = row.transform.Find("NameField")
                                               .GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = row.transform.Find("TimeField")
                                               .GetComponent<TextMeshProUGUI>();

            nameText.text = entry.playerName;
            timeText.text = $"{entry.timeSeconds:0.00}";
        }
    }
    public void OnBackButtonPressed()
    {
        // Load CardMatch scene (index 0)
        SceneManager.LoadScene(0);
    }
}
