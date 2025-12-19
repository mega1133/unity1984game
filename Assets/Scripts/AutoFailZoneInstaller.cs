using UnityEngine;
using UnityEngine.SceneManagement;

public static class AutoFailZoneInstaller
{
    private const float DefaultDepth = -15f;
    private const float ExtraDepthPadding = 5f;
    private const float DefaultWidth = 120f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureSpawnCheckpoint(scene);
        EnsureFailZone(scene);
    }

    private static void EnsureSpawnCheckpoint(Scene scene)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.HasCheckpointForScene(scene.name))
        {
            return;
        }

        LevelSpawnPoint spawn = Object.FindObjectOfType<LevelSpawnPoint>();
        if (spawn != null)
        {
            spawn.TrySetCheckpoint();
        }
    }

    private static void EnsureFailZone(Scene scene)
    {
        if (Object.FindObjectOfType<FailZone>() != null)
        {
            return;
        }

        Camera cam = Camera.main;
        float y = DefaultDepth;
        float width = DefaultWidth;

        if (cam != null && cam.orthographic)
        {
            y = cam.transform.position.y - (cam.orthographicSize + ExtraDepthPadding);
            width = cam.orthographicSize * cam.aspect * 4f;
        }

        GameObject failObj = new GameObject("AutoFailZone");
        failObj.transform.position = new Vector3(0f, y, 0f);

        BoxCollider2D collider = failObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(width, 5f);

        FailZone failZone = failObj.AddComponent<FailZone>();
        failZone.SetReason("FALL");
    }
}
