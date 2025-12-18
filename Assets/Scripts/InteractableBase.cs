using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private string promptText = "PRESS E";
    [SerializeField] private bool isEnabled = true;

    public bool IsEnabled => isEnabled && enabled && gameObject.activeInHierarchy;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    public virtual string GetPromptText()
    {
        return string.IsNullOrWhiteSpace(promptText) ? "PRESS E" : promptText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public abstract void Interact(GameObject interactor);

    public void SetEnabled(bool value)
    {
        isEnabled = value;
    }
}
