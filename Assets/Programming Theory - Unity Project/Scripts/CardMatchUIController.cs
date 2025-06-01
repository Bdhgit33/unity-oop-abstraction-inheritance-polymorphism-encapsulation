using UnityEngine;
using TMPro;

/// <summary>
/// Reads the saved player name from PlayerPrefs and writes it into a TMP_Text field
/// on the CardMatch scene. Attach this to a GameObject in CardMatch.
/// </summary>
public class CardMatchUIController : MonoBehaviour
{
    [Header("Hook up in Inspector:")]
    [Tooltip("Drag the TextMeshProUGUI component here that should display the player’s name.")]
    public TextMeshProUGUI playerNameDisplay;

    private void Start()
    {
        // Retrieve the name
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrEmpty(savedName))
        {
            playerNameDisplay.text = "Player: ???";
            Debug.LogWarning("CardMatchUIController: No player name found in PlayerPrefs.");
        }
        else
        {
            playerNameDisplay.text = $"Player: {savedName}";
        }
    }
}
