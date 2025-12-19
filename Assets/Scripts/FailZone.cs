using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FailZone : MonoBehaviour
{
    [SerializeField] private string reason = "CAPTURED";

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Fail(reason);
        }
    }

    public void SetReason(string newReason)
    {
        reason = newReason;
    }
}
