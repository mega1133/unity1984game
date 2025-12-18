using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "";

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || string.IsNullOrEmpty(targetSceneName))
        {
            return;
        }

        FadeManager.GetOrCreate()?.LoadSceneWithFade(targetSceneName);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"Exit → {targetSceneName}");
        }
    }
#endif
}
