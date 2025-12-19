using UnityEngine;

public class ShopCounterInteractable : InteractableBase
{
    [SerializeField] private string prompt = "PRESS E TO BUY DIARY";
    [SerializeField] private string purchasedPrompt = "PURCHASED";

    private bool purchased;

    public override string GetPromptText()
    {
        return purchased ? purchasedPrompt : prompt;
    }

    public override void Interact(GameObject interactor)
    {
        if (purchased)
        {
            return;
        }

        purchased = true;
        Level1DiaryController.Instance?.MarkDiaryPurchased();
    }
}
