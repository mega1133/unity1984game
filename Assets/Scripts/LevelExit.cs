using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    public enum ExitMode
    {
        DirectSceneName,
        UseLevelControllerNextScene
    }

    [SerializeField] private ExitMode mode = ExitMode.DirectSceneName;
    [SerializeField] private string targetSceneName = "";

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

        string sceneToLoad = ResolveSceneName();
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            return;
        }

        FadeManager.GetOrCreate()?.LoadSceneWithFade(sceneToLoad);
    }

    private string ResolveSceneName()
    {
        switch (mode)
        {
            case ExitMode.DirectSceneName:
                return targetSceneName;
            case ExitMode.UseLevelControllerNextScene:
                LevelController controller = FindObjectOfType<LevelController>();
                if (controller == null)
                {
                    Debug.LogError("LevelExit: No LevelController found in scene for next-scene mode.");
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(controller.NextSceneName))
                {
                    Debug.LogError("LevelExit: LevelController nextSceneName is empty.");
                    return string.Empty;
                }

                return controller.NextSceneName;
            default:
                return string.Empty;
        }
    }

    public void SetMode(ExitMode newMode)
    {
        mode = newMode;
    }

    public void SetTargetScene(string sceneName)
    {
        targetSceneName = sceneName;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string label = mode == ExitMode.UseLevelControllerNextScene
            ? "Exit → (LevelController)"
            : targetSceneName;

        if (!string.IsNullOrEmpty(label))
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"Exit → {label}");
        }
    }
#endif
}
