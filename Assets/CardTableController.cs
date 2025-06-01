using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates 16 cards (pairsCount * 2) under this GameObject (which should have FlexibleGridLayout).
/// Each clone is initialized with a unique ID (0..pairsCount-1) and a corresponding front‐prefab (Image + Text).
/// </summary>
public class CardTableController : MonoBehaviour
{
    [Header("Card Prefab & Front Prefabs")]
    [Tooltip("Drag your Card Prefab here. The prefab must have:\n" +
             " • An Image component (back of card)\n" +
             " • A Button component (OnClick → ImageCard.OnCardClicked)\n" +
             " • The ImageCard (subclass of CardBase) script attached\n" +
             " • A child GameObject named 'FrontContainer' (initially disabled)\n")]
    public GameObject cardPrefab;

    [Tooltip("List of exactly 8 front‐prefab GameObjects.\n" +
             "Each front‐prefab should contain:\n" +
             " • A UI Image (front sprite)\n" +
             " • A child UI Text (displaying the number)\n")]
    public List<GameObject> frontCards = new List<GameObject>();

    [Tooltip("Number of matching pairs (e.g. 8 for a 4×4 grid → 16 total cards).")]
    public int pairsCount = 8;

    // (Optional) Keep references to instantiated cards
    private List<GameObject> instantiatedCards = new List<GameObject>();

    private void Start()
    {
        SetupCards();
    }

    /// <summary>
    /// Builds a shuffled ID list [0,0,1,1,2,2,..., pairsCount-1, pairsCount-1],
    /// then instantiates one card clone per entry under this GameObject.
    /// Each clone is given its ID and front‐prefab.
    /// </summary>
    private void SetupCards()
    {
        // 1) Build a list of IDs (two of each from 0 to pairsCount-1)
        List<int> ids = new List<int>();
        for (int i = 0; i < pairsCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // 2) Shuffle the ID list in place
        for (int i = 0; i < ids.Count; i++)
        {
            int swapIndex = Random.Range(i, ids.Count);
            int temp = ids[i];
            ids[i] = ids[swapIndex];
            ids[swapIndex] = temp;
        }

        // 3) Instantiate a clone for each ID, parented to this GameObject (CardTable)
        for (int i = 0; i < ids.Count; i++)
        {
            int cardID = ids[i];
            GameObject cardGO = Instantiate(cardPrefab, transform);
            cardGO.name = $"Card_{i}_ID{cardID}";

            // Get the ImageCard component (subclass of CardBase) on the cloned prefab
            ImageCard cardScript = cardGO.GetComponent<ImageCard>();
            if (cardScript == null)
            {
                Debug.LogError("CardTableController: cardPrefab is missing an ImageCard component!");
                continue;
            }

            // Safely pick the correct front‐prefab (which itself has Image + Text children)
            if (cardID < frontCards.Count && frontCards[cardID] != null)
            {
                cardScript.InitializeCard(cardID, frontCards[cardID]);
            }
            else
            {
                Debug.LogError($"CardTableController: frontCards does not contain an entry for index {cardID}.");
            }

            instantiatedCards.Add(cardGO);
        }
    }
}
