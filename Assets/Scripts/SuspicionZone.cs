using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SuspicionZone : MonoBehaviour
{
    [SerializeField] private float secondsToMax = 5f;
    [SerializeField] private string failReason = "LOITERING";
    [SerializeField] private Color zoneColor = new Color(0.8f, 0.55f, 0.2f, 0.9f);

    public float SecondsToMax => secondsToMax;
    public string FailReason => failReason;

    public void SetSecondsToMax(float value)
    {
        secondsToMax = Mathf.Max(0.01f, value);
    }

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        var sprite = GetComponent<PlaceholderSprite>();
        if (sprite != null)
        {
            sprite.SetColor(zoneColor);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var suspicion = other.GetComponent<PlayerSuspicionController>();
        if (suspicion != null)
        {
            suspicion.EnterZone(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var suspicion = other.GetComponent<PlayerSuspicionController>();
        if (suspicion != null)
        {
            suspicion.ExitZone(this);
        }
    }
}
