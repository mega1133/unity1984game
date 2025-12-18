using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SafeZone : MonoBehaviour
{
    [SerializeField] private Color zoneColor = new Color(0.3f, 0.8f, 0.3f, 1f);

    private void Reset()
    {
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;

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

        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.EnterSafeZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.ExitSafeZone();
        }
    }
}
