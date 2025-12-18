using UnityEngine;

public class SimplePickupInteractable : InteractableBase
{
    [SerializeField] private string pickupName = "Item";

    public override void Interact(GameObject interactor)
    {
        Debug.Log($"Picked up {pickupName}");
        Destroy(gameObject);
    }
}
