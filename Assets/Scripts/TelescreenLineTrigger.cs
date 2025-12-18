using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TelescreenLineTrigger : MonoBehaviour
{
    [SerializeField] private TelescreenController target;
    [SerializeField] [TextArea] private string lineText = "6079 SMITH W. NO UNEXPECTED MOVEMENTS!";
    [SerializeField] private bool oneShot = true;

    private bool fired;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (oneShot && fired)
        {
            return;
        }

        fired = true;
        if (target != null)
        {
            target.PlayLine(lineText);
        }
    }

    public void SetTarget(TelescreenController controller)
    {
        target = controller;
    }

    public void SetLineText(string line)
    {
        lineText = line;
    }
}
