using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RocketBombTrigger : MonoBehaviour
{
    [SerializeField] private RocketBombController controller;
    [SerializeField] private bool oneShot = true;

    private bool fired;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
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

        if (controller == null)
        {
            return;
        }

        fired = true;
        controller.StartStrike();
    }

    public void SetController(RocketBombController target)
    {
        controller = target;
    }
}
