using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Attached to your TitleScreen Canvas (or another GameObject).
/// Grabs the player’s name from a TMP_InputField, saves it to PlayerPrefs, 
/// and loads the CardMatch scene (index 0) as long as the field is nonempty.
/// </summary>
public class TitleScreenController : MonoBehaviour
{
    [Header("Hook up in Inspector:")]
    [Tooltip("Drag your TMP_InputField here where the player types their name.")]
    public TMP_InputField nameInputField;

    /// <summary>
    /// Called when the Start button is clicked.
    /// </summary>
    public void OnStartButtonPressed()
    {
        string enteredName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(enteredName))
        {
            // Optionally you could show an on‐screen warning here.
            Debug.LogWarning("TitleScreen: No name entered, cannot proceed.");
            return;
        }

        // Save to PlayerPrefs so CardMatch scene can read it
        PlayerPrefs.SetString("PlayerName", enteredName);
        PlayerPrefs.Save();

        // Load CardMatch scene (build index 0)
        SceneManager.LoadScene(0);
    }
}
