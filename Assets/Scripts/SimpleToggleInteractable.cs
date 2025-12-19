using UnityEngine;

public class SimpleToggleInteractable : InteractableBase
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private bool isOn = true;
    [SerializeField] private Color onColor = Color.cyan;
    [SerializeField] private Color offColor = Color.gray;
    [SerializeField] private string onPrompt = "PRESS E TO TURN OFF";
    [SerializeField] private string offPrompt = "PRESS E TO TURN ON";

    private void Awake()
    {
        ApplyState();
    }

    public override void Interact(GameObject interactor)
    {
        isOn = !isOn;
        ApplyState();
    }

    public override string GetPromptText()
    {
        return isOn ? onPrompt : offPrompt;
    }

    private void ApplyState()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        if (targetRenderer != null)
        {
            targetRenderer.color = isOn ? onColor : offColor;
        }
    }
}
