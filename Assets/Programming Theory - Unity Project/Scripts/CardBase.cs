using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract base class for any card type.
/// </summary>
public abstract class CardBase : MonoBehaviour
{
    [SerializeField]
    private int _cardID = -1;
    public int CardID
    {
        get { return _cardID; }
        protected set
        {
            if (value >= 0) _cardID = value;
            else Debug.LogWarning($"CardBase: Invalid CardID {value}");
        }
    }

    // Make _isFaceUp protected so derived classes can set it
    protected bool _isFaceUp = false;
    public bool IsFaceUp => _isFaceUp;

    // The “back” image on the root of the prefab
    protected Image BackImageComponent;

    // Empty child under which we will instantiate the front‐prefab
    protected GameObject FrontContainer;

    // Reference to the instantiated front‐prefab
    private GameObject _frontInstance;

    protected virtual void Awake()
    {
        // Cache the back Image component
        BackImageComponent = GetComponent<Image>();

        // Find the child named “FrontContainer”
        Transform fc = transform.Find("FrontContainer");
        if (fc != null)
        {
            FrontContainer = fc.gameObject;
        }
        else
        {
            Debug.LogError("CardBase: Could not find child named 'FrontContainer'.");
        }
    }

    /// <summary>
    /// Initialize this card’s ID and instantiate its front‐prefab under FrontContainer.
    /// </summary>
    public virtual void InitializeCard(int id, GameObject frontPrefab)
    {
        CardID = id;

        // Destroy any existing front instance (if reinitializing)
        if (_frontInstance != null)
            Destroy(_frontInstance);

        if (FrontContainer != null && frontPrefab != null)
        {
            // Instantiate the entire front prefab under FrontContainer
            _frontInstance = Instantiate(frontPrefab, FrontContainer.transform);
            _frontInstance.name = $"Front_{id}";
        }
        else
        {
            Debug.LogError("CardBase: Missing FrontContainer or frontPrefab is null.");
        }

        // Start face‐down
        Hide();
    }

    /// <summary>
    /// Abstract: derived classes implement how to reveal.
    /// </summary>
    public abstract void Reveal();

    /// <summary>
    /// Hide the front by disabling FrontContainer and show the back.
    /// </summary>
    public virtual void Hide()
    {
        if (BackImageComponent != null)
            BackImageComponent.enabled = true;

        if (FrontContainer != null)
            FrontContainer.SetActive(false);

        _isFaceUp = false;
    }

    /// <summary>
    /// Flip the card: if face‐up, Hide(); otherwise, Reveal().
    /// </summary>
    public void Flip()
    {
        if (_isFaceUp) Hide();
        else Reveal();
    }

    /// <summary>
    /// Called by the root Button’s OnClick. Default: Reveal() if face‐down.
    /// </summary>
    public virtual void OnCardClicked()
    {
        if (IsFaceUp) return;
        Reveal();
    }
}
