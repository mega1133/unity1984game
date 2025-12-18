using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1DiarySetup : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPosition = new Vector2(-14f, -1f);
    [SerializeField] private Color groundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color wallColor = new Color(0.15f, 0.16f, 0.16f, 1f);
    [SerializeField] private Color posterColor = new Color(0.32f, 0.33f, 0.33f, 1f);
    [SerializeField] private Color posterTextColor = new Color(0.9f, 0.9f, 0.88f, 1f);
    [SerializeField] private Color safeZoneColor = new Color(0.35f, 0.65f, 0.35f, 0.9f);

    private Transform player;
    private LevelController levelController;
    private Level1DiaryController diaryController;

    private const string PosterBigBrother = "BIG BROTHER IS WATCHING YOU.";
    private const string PosterIngsoc = "INGSOC";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Level1_Diary")
        {
            return;
        }

        GameObject obj = new GameObject("Level1_Diary_Setup");
        obj.AddComponent<Level1DiarySetup>();
    }

    private void Start()
    {
        StartCoroutine(BuildRoutine());
    }

    private IEnumerator BuildRoutine()
    {
        yield return null;

        EnsurePlayer();
        EnsureCamera();
        EnsureLevelController();
        EnsureDiaryController();
        CreateSpawnPoint();

        BuildStreetAndPit();
        BuildShop();
        BuildCheckpoint();
        BuildSafeZone();
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
            existing.backgroundColor = new Color(0.08f, 0.09f, 0.1f, 1f);
            existing.orthographic = true;
            return;
        }

        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.1f, 1f);
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
        }
        else
        {
            GameObject controllerObj = new GameObject("LevelController");
            levelController = controllerObj.AddComponent<LevelController>();
        }

        levelController.Configure("Level1", "Level2_HateHall", new[]
        {
            "Go to the shop",
            "Buy the diary",
            "Find a safe zone",
            "Write in the diary"
        }, false);
    }

    private void EnsureDiaryController()
    {
        diaryController = FindObjectOfType<Level1DiaryController>();
        if (diaryController == null)
        {
            GameObject obj = new GameObject("Level1DiaryController");
            diaryController = obj.AddComponent<Level1DiaryController>();
        }

        diaryController.SetLevelController(levelController);
    }

    private void CreateSpawnPoint()
    {
        GameObject spawn = new GameObject("LevelSpawnPoint");
        spawn.transform.position = spawnPosition;
        spawn.AddComponent<LevelSpawnPoint>();
    }

    private void BuildStreetAndPit()
    {
        CreateGround(new Vector2(-12f, -2f), 10f);
        CreateGround(new Vector2(-2f, -2f), 6f);
        CreateGround(new Vector2(7f, -2f), 12f);

        GameObject fail = new GameObject("FailZone_Pit");
        fail.transform.position = new Vector2(-6f, -4f);
        var col = fail.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 3.5f);
        var sprite = fail.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.2f, 0.05f, 0.05f, 0.4f));
        var zone = fail.AddComponent<FailZone>();
        zone.SetReason("FALL");

        for (int i = -14; i <= 14; i += 4)
        {
            CreateWallColumn(new Vector2(i, 0.75f));
        }
    }

    private void BuildShop()
    {
        GameObject counter = new GameObject("ShopCounter");
        counter.transform.position = new Vector2(-0.5f, -1f);
        counter.transform.localScale = new Vector3(2.8f, 1.6f, 1f);
        var sprite = counter.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.32f, 0.28f, 0.26f, 1f));

        BoxCollider2D col = counter.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1f, 1f);

        var interact = counter.AddComponent<ShopCounterInteractable>();

        GameObject shopSign = CreatePoster(new Vector2(-2.5f, 0.6f), PosterBigBrother);
        shopSign.name = "Poster_Shop";

        GameObject shopWatch = CreatePoster(new Vector2(2.5f, 0.6f), PosterIngsoc);
        shopWatch.name = "Poster_Shop_Ingsoc";

        GameObject signInteract = new GameObject("ShopSignInteractable");
        signInteract.transform.position = new Vector2(1.2f, -0.8f);
        BoxCollider2D signCol = signInteract.AddComponent<BoxCollider2D>();
        signCol.isTrigger = true;
        signCol.size = new Vector2(1f, 1.6f);
        var signSprite = signInteract.AddComponent<PlaceholderSprite>();
        signSprite.SetColor(new Color(0.22f, 0.22f, 0.22f, 0.75f));

        DialogueInteractable dialogue = signInteract.AddComponent<DialogueInteractable>();
        dialogue.SetLines(new List<DialogueLine>
        {
            new DialogueLine(signInteract.transform, "PURCHASES ARE OBSERVED.")
        });
    }

    private void BuildCheckpoint()
    {
        GameObject checkpoint = new GameObject("Checkpoint_AfterShop");
        checkpoint.transform.position = new Vector2(3.5f, -1.5f);
        BoxCollider2D col = checkpoint.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 1.6f);
        var sprite = checkpoint.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.9f, 0.8f, 0.2f, 1f));
        checkpoint.AddComponent<Checkpoint>();
    }

    private void BuildSafeZone()
    {
        GameObject safe = new GameObject("SafeZone_HomeCorner");
        safe.transform.position = new Vector2(9f, -1.8f);
        safe.transform.localScale = new Vector3(3f, 1f, 1f);
        BoxCollider2D col = safe.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = Vector2.one;
        var sprite = safe.AddComponent<PlaceholderSprite>();
        sprite.SetColor(safeZoneColor);
        safe.AddComponent<SafeZone>();
    }

    private void BuildPostersAndProps()
    {
        CreatePoster(new Vector2(-9.5f, 0.7f), PosterBigBrother);
        CreatePoster(new Vector2(6.5f, 0.7f), PosterIngsoc);
    }

    private void BuildTelescreens()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Telescreen");
        if (prefab == null)
        {
            return;
        }

        Vector2[] positions = { new Vector2(-11f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(8.5f, 0.5f) };
        for (int i = 0; i < positions.Length; i++)
        {
            Instantiate(prefab, positions[i], Quaternion.identity);
        }
    }

    private void BuildObligationTriggers()
    {
        CreateObligationTrigger(new Vector2(-1.5f, -1.5f), 0, 2.8f, 2.4f);
    }

    private void BuildExit()
    {
        GameObject exit = new GameObject("LevelExit");
        exit.transform.position = new Vector2(13.5f, -1.5f);
        var sprite = exit.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.35f, 0.65f, 0.7f, 0.65f));
        BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.4f, 2.4f);

        LevelExit exitComp = exit.AddComponent<LevelExit>();
        exitComp.SetMode(LevelExit.ExitMode.UseLevelControllerNextScene);
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

    private GameObject CreatePoster(Vector2 position, string text)
    {
        GameObject poster = new GameObject("Poster");
        poster.transform.position = position;
        SpriteRenderer posterRenderer = poster.AddComponent<SpriteRenderer>();
        posterRenderer.sprite = CreatePosterSprite();
        posterRenderer.color = posterColor;

        GameObject textObj = new GameObject("PosterText");
        textObj.transform.SetParent(poster.transform, false);
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 3.5f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = posterTextColor;
        tmp.rectTransform.sizeDelta = new Vector2(6f, 2f);

        return poster;
    }

    private Sprite CreatePosterSprite()
    {
        Texture2D texture = new Texture2D(16, 24)
        {
            filterMode = FilterMode.Point
        };

        Color[] pixels = new Color[16 * 24];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16f);
    }

    private void CreateObligationTrigger(Vector2 center, int obligationIndex, float width = 1f, float height = 2f)
    {
        GameObject trigger = new GameObject($"ObligationTrigger_{obligationIndex}");
        trigger.transform.position = center;
        trigger.transform.localScale = new Vector3(width, height, 1f);
        BoxCollider2D col = trigger.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = Vector2.one;
        var sprite = trigger.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0f, 0f, 0f, 0f));

        LevelObligationCompleteTrigger comp = trigger.AddComponent<LevelObligationCompleteTrigger>();
        comp.SetObligationIndex(obligationIndex);
    }
}
