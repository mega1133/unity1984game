using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneSequence sequence;
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

        if (sequence == null)
        {
            return;
        }

        fired = true;
        CutsceneRunner.Instance.Play(sequence);
    }

    public void SetSequence(CutsceneSequence newSequence)
    {
        sequence = newSequence;
    }
}
