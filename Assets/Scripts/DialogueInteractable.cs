using System.Collections.Generic;
using UnityEngine;

public class DialogueInteractable : InteractableBase
{
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();
    [SerializeField] private bool disableAfterUse = false;

    public override void Interact(GameObject interactor)
    {
        if (DialogueRunner.Instance.IsPlaying || lines == null || lines.Count == 0)
        {
            return;
        }

        DialogueRunner.Instance.StartDialogue(lines, OnDialogueComplete);

        if (disableAfterUse)
        {
            SetEnabled(false);
        }
    }

    public void SetLines(List<DialogueLine> newLines)
    {
        lines = newLines ?? new List<DialogueLine>();
    }

    private void OnDialogueComplete()
    {
        // Placeholder for future callbacks.
    }
}
