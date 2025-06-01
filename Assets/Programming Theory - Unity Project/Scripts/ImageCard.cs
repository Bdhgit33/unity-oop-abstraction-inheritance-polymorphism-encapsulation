using UnityEngine;
using UnityEngine.UI;

public class ImageCard : CardBase
{
    private Button _button;

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<Button>();
        if (_button == null)
            Debug.LogError("ImageCard: No Button component found on the card root!");
    }

    /// <summary>
    /// Reveal: hide the back, show FrontContainer, set _isFaceUp = true, disable the button,
    /// then enqueue ourselves in the CardGameManager.
    /// </summary>
    public override void Reveal()
    {
        // Hide the back image
        if (BackImageComponent != null)
            BackImageComponent.enabled = false;

        // Show the front container (with instantiated front prefab inside)
        if (FrontContainer != null)
            FrontContainer.SetActive(true);

        // Mark as face‐up
        _isFaceUp = true;

        // Disable this card's button so it cannot be clicked again until it's hidden or matched
        if (_button != null)
            _button.interactable = false;

        // Enqueue ourselves in the manager
        if (CardGameManager.Instance != null)
        {
            CardGameManager.Instance.EnqueueRevealedCard(this);
        }
        else
        {
            Debug.LogError("ImageCard: CardGameManager.Instance is null! Did you forget to add a CardGameManager to the scene?");
        }
    }

    /// <summary>
    /// Called by the root Button’s OnClick: if still face‐down, reveal.
    /// </summary>
    public override void OnCardClicked()
    {
        if (_isFaceUp) return;
        Reveal();
    }

    /// <summary>
    /// Re‐enable this card’s button (used when flipping back a non‐match).
    /// </summary>
    public void ReenableButton()
    {
        if (_button != null)
            _button.interactable = true;
    }
}
