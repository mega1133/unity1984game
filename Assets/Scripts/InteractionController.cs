using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private float detectRadius = 1.6f;
    [SerializeField] private LayerMask interactableLayers = ~0;
    [SerializeField] private int maxResults = 8;

    private readonly Collider2D[] results = new Collider2D[16];
    private PlayerMovement movement;
    private IInteractable current;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (movement != null && !movement.IsControlEnabled)
        {
            ClearCurrent();
            return;
        }

        FindBestInteractable();
        HandleInput();
        UpdatePrompt();
    }

    private void FindBestInteractable()
    {
        int hits = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, results, interactableLayers);
        IInteractable best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < hits && i < maxResults; i++)
        {
            Collider2D col = results[i];
            if (col == null || !col.gameObject.activeInHierarchy)
            {
                continue;
            }

            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null || !interactable.IsEnabled)
            {
                continue;
            }

            float dist = (interactable.GetTransform().position - transform.position).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
            }
        }

        current = best;
    }

    private void HandleInput()
    {
        if (current == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            current.Interact(gameObject);
        }
    }

    private void UpdatePrompt()
    {
        if (current == null)
        {
            ClearCurrent();
            return;
        }

        InteractionPromptUI.Instance.Show(current.GetPromptText());
    }

    private void ClearCurrent()
    {
        current = null;
        InteractionPromptUI.Instance.Hide();
    }
}
