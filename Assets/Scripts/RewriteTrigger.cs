using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RewriteTrigger : MonoBehaviour
{
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool setToRewritten = true;

    private bool fired;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fired && triggerOnce)
        {
            return;
        }

        if (!collision.CompareTag("Player"))
        {
            return;
        }

        fired = true;
        RewriteManager.Instance.SetRewritten(setToRewritten);
    }
}
