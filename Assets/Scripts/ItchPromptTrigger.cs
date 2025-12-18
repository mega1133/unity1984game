using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItchPromptTrigger : MonoBehaviour
{
    [SerializeField] private bool oneShot = true;
    private bool fired;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fired && oneShot)
        {
            return;
        }

        if (!collision.CompareTag("Player"))
        {
            return;
        }

        PlayerScratch scratch = collision.GetComponent<PlayerScratch>();
        if (scratch == null)
        {
            return;
        }

        scratch.ShowItchPromptOnce();
        fired = true;
    }
}
