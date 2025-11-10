using UnityEngine;

// Attach this script to any GameObject that has a SpriteRenderer.
// On mouse click it toggles between the original color and the highlight color.
public class ClickHighlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;
    private bool isHighlighted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"ClickHighlight on '{gameObject.name}' expects a SpriteRenderer component. Disabling script.");
            enabled = false;
            return;
        }
        originalColor = spriteRenderer.color;
    }

    void OnMouseDown()
    {
        // Guard in case the renderer was removed at runtime
        if (spriteRenderer == null) return;

        isHighlighted = !isHighlighted;
        spriteRenderer.color = isHighlighted ? highlightColor : originalColor;
    }
}
