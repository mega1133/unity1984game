using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelObligationCompleteTrigger : MonoBehaviour
{
    [SerializeField] private int obligationIndex = 0;
    [SerializeField] private bool oneShot = true;

    private bool fired;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (fired && oneShot)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        LevelController controller = FindObjectOfType<LevelController>();
        if (controller == null)
        {
            Debug.LogWarning("LevelObligationCompleteTrigger: No LevelController found.");
            return;
        }

        fired = true;
        controller.CompleteObligation(obligationIndex);
    }

    public void SetObligationIndex(int index)
    {
        obligationIndex = index;
    }
}
