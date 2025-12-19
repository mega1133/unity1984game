using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level0VictoryMansionsSetup : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPosition = new Vector2(-9f, -1f);
    [SerializeField] private Color groundColor = new Color(0.22f, 0.24f, 0.23f, 1f);
    [SerializeField] private Color wallColor = new Color(0.18f, 0.2f, 0.19f, 1f);
    [SerializeField] private Color posterColor = new Color(0.35f, 0.36f, 0.34f, 1f);
    [SerializeField] private Color posterTextColor = new Color(0.92f, 0.9f, 0.88f, 1f);

    private Transform player;
    private LevelController levelController;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Level0_VictoryMansions")
        {
            return;
        }

        GameObject obj = new GameObject("Level0_VictoryMansionsSetup");
        obj.AddComponent<Level0VictoryMansionsSetup>();
    }

    private void Start()
    {
        StartCoroutine(BuildRoutine());
    }

    private IEnumerator BuildRoutine()
    {
        yield return null; // allow other singletons to initialize

        EnsurePlayer();
        EnsureCamera();
        EnsureLevelController();
        CreateSpawnPoint();

        BuildHallway();
        BuildGapAndFailZone();
        BuildCheckpoint();
        BuildStepPlatform();
        BuildPostersAndProps();
        BuildTelescreens();
        BuildObligationTriggers();
        BuildExit();
    }

    private void EnsurePlayer()
    {
        GameObject existing = GameObject.FindGameObjectWithTag("Player");
        if (existing != null)
        {
            player = existing.transform;
            player.position = spawnPosition;
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            player = instance.transform;
            return;
        }

        // Minimal fallback if prefab is missing.
        GameObject fallback = new GameObject("Player_Fallback");
        fallback.tag = "Player";
        player = fallback.transform;
        var rb = fallback.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        BoxCollider2D col = fallback.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.9f, 1.8f);
        var sprite = fallback.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.75f, 0.8f, 0.82f, 1f));
    }

    private void EnsureCamera()
    {
        Camera existing = Camera.main;
        if (existing != null)
        {
            var follow = existing.GetComponent<CameraFollow>();
            if (follow == null)
            {
                follow = existing.gameObject.AddComponent<CameraFollow>();
            }
            follow.SetTarget(player);
            existing.backgroundColor = new Color(0.09f, 0.1f, 0.11f, 1f);
            existing.orthographic = true;
            return;
        }

        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.backgroundColor = new Color(0.09f, 0.1f, 0.11f, 1f);
        camObj.AddComponent<AudioListener>();
        var followComponent = camObj.AddComponent<CameraFollow>();
        followComponent.SetTarget(player);
    }

    private void EnsureLevelController()
    {
        LevelController existing = FindObjectOfType<LevelController>();
        if (existing != null)
        {
            levelController = existing;
            levelController.Configure("Level0", "Level1_Diary", new[]
            {
                "Move to the exit",
                "Jump the gap"
            }, false);
            return;
        }

        GameObject controllerObj = new GameObject("LevelController");
        levelController = controllerObj.AddComponent<LevelController>();
        levelController.Configure("Level0", "Level1_Diary", new[]
        {
            "Move to the exit",
            "Jump the gap"
        }, false);
    }

    private void CreateSpawnPoint()
    {
        GameObject spawn = new GameObject("LevelSpawnPoint");
        spawn.transform.position = spawnPosition;
        spawn.AddComponent<LevelSpawnPoint>();
    }

    private void BuildHallway()
    {
        // Two floor slabs with a forgiving single-jump gap between them.
        CreateGround(new Vector2(-7f, -2f), 10f);
        CreateGround(new Vector2(7f, -2f), 16f);

        // Background strips to hint walls/doors.
        for (int i = -12; i <= 16; i += 4)
        {
            CreateWallColumn(new Vector2(i, 0.5f));
        }
    }

    private void BuildGapAndFailZone()
    {
        // Gap roughly between x=-2 and x=-1 (single jump).
        GameObject fail = new GameObject("FailZone_Gap");
        fail.transform.position = new Vector2(-1.5f, -3f);
        var col = fail.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.2f, 3f);
        var sprite = fail.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.2f, 0.05f, 0.05f, 0.4f));
        var zone = fail.AddComponent<FailZone>();
        zone.SetReason("FALL");
    }

    private void BuildCheckpoint()
    {
        GameObject checkpoint = new GameObject("Checkpoint_Mid");
        checkpoint.transform.position = new Vector2(2.5f, -1.5f);
        BoxCollider2D col = checkpoint.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 1.6f);
        var sprite = checkpoint.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.9f, 0.8f, 0.2f, 1f));
        checkpoint.AddComponent<Checkpoint>();
    }

    private void BuildStepPlatform()
    {
        GameObject platform = CreateGround(new Vector2(12f, -1.2f), 3f, 0.8f);
        platform.name = "Platform_Step";
    }

    private void BuildPostersAndProps()
    {
        CreatePoster(new Vector2(-6.5f, 0.6f), "BIG BROTHER IS WATCHING YOU.");
        CreatePoster(new Vector2(6f, 0.6f), "INGSOC");
    }

    private void BuildTelescreens()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Telescreen");
        if (prefab == null)
        {
            return;
        }

        Vector2[] positions = { new Vector2(-5.5f, -0.3f), new Vector2(9.5f, -0.3f) };
        for (int i = 0; i < positions.Length; i++)
        {
            Instantiate(prefab, positions[i], Quaternion.identity);
        }
    }

    private void BuildObligationTriggers()
    {
        // Complete obligation 2 (index 1) after the gap.
        CreateObligationTrigger(new Vector2(1.2f, -1.5f), 1);
    }

    private void BuildExit()
    {
        GameObject exit = new GameObject("LevelExit");
        exit.transform.position = new Vector2(15f, -1.5f);
        var sprite = exit.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.35f, 0.65f, 0.7f, 0.65f));
        BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.4f, 2.4f);

        LevelExit exitComp = exit.AddComponent<LevelExit>();
        exitComp.SetMode(LevelExit.ExitMode.UseLevelControllerNextScene);

        // Mark obligation 0 complete at the exit.
        CreateObligationTrigger(exit.transform.position + new Vector3(0f, 0f, 0f), 0, 1.6f, 2.6f);
    }

    private GameObject CreateGround(Vector2 center, float width, float height = 1f)
    {
        GameObject ground = new GameObject("Ground");
        ground.transform.position = center;
        ground.transform.localScale = new Vector3(width, height, 1f);
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
        {
            ground.layer = groundLayer;
        }

        var sprite = ground.AddComponent<PlaceholderSprite>();
        sprite.SetColor(groundColor);

        BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.offset = Vector2.zero;
        return ground;
    }

    private void CreateWallColumn(Vector2 position)
    {
        GameObject wall = new GameObject("WallStrip");
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(0.8f, 3.2f, 1f);
        var sprite = wall.AddComponent<PlaceholderSprite>();
        sprite.SetColor(wallColor);
    }

    private void CreatePoster(Vector2 position, string text)
    {
        GameObject poster = new GameObject("Poster");
        poster.transform.position = position;
        poster.transform.localScale = new Vector3(2.8f, 1.6f, 1f);
        var sprite = poster.AddComponent<PlaceholderSprite>();
        sprite.SetColor(posterColor);

        GameObject textObj = new GameObject("PosterText");
        textObj.transform.SetParent(poster.transform, false);
        var tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 4f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = posterTextColor;
        tmp.rectTransform.sizeDelta = new Vector2(3f, 1.2f);
        MeshRenderer renderer = tmp.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 2;
        }
    }

    private void CreateObligationTrigger(Vector2 position, int obligationIndex, float width = 1.2f, float height = 2f)
    {
        GameObject trigger = new GameObject($"ObligationTrigger_{obligationIndex}");
        trigger.transform.position = position;
        BoxCollider2D col = trigger.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(width, height);
        var complete = trigger.AddComponent<LevelObligationCompleteTrigger>();
        complete.SetObligationIndex(obligationIndex);
    }
}
