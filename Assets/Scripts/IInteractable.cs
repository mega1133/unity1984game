using UnityEngine;

public interface IInteractable
{
    string GetPromptText();
    Transform GetTransform();
    void Interact(GameObject interactor);
    bool IsEnabled { get; }
}
