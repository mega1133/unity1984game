using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRewriteDemo : MonoBehaviour
{
    [SerializeField] private Vector2 corridorOrigin = new Vector2(28f, -2f);
    [SerializeField] private float corridorLength = 8f;
    [SerializeField] private float corridorHeight = 1f;
    [SerializeField] private Color posterColorA = new Color(0.25f, 0.35f, 0.75f, 1f);
    [SerializeField] private Color posterColorB = new Color(0.6f, 0.55f, 0.2f, 1f);
    [SerializeField] private Color symeColor = new Color(0.7f, 0.3f, 0.4f, 1f);

    private const string PosterLineA = "WE WILL NEVER FORGET YOU, SYME!";
    private const string PosterLineB = "NOTICE: DICTIONARY REVISION AUTHORIZED.";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Level0_Test")
        {
            return;
        }

        GameObject obj = new GameObject("LevelRewriteDemo");
        obj.AddComponent<LevelRewriteDemo>();
    }

    private void Start()
    {
        BuildCorridor();
    }

    private void BuildCorridor()
    {
        // Ground
        GameObject ground = new GameObject("RewriteCorridor_Ground");
        ground.transform.position = corridorOrigin;
        ground.transform.localScale = new Vector3(corridorLength, corridorHeight, 1f);
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
        {
            ground.layer = groundLayer;
        }

        var groundSprite = ground.AddComponent<PlaceholderSprite>();
        groundSprite.SetColor(new Color(0.2f, 0.2f, 0.2f, 1f));
        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(1f, 1f);

        // Syme NPC (to be erased)
        GameObject syme = new GameObject("Syme_NPC");
        syme.transform.position = corridorOrigin + new Vector2(0.5f, 1f);
        PlaceholderSprite symeSprite = syme.AddComponent<PlaceholderSprite>();
        symeSprite.SetColor(symeColor);
        BoxCollider2D symeCollider = syme.AddComponent<BoxCollider2D>();
        symeCollider.isTrigger = true;

        // Poster with text swap
        GameObject poster = new GameObject("Poster_Syme");
        poster.transform.position = corridorOrigin + new Vector2(-0.6f, 1.2f);
        SpriteRenderer posterRenderer = poster.AddComponent<SpriteRenderer>();
        posterRenderer.sprite = CreatePosterSprite();
        posterRenderer.color = posterColorA;

        GameObject textObj = new GameObject("PosterText");
        textObj.transform.SetParent(poster.transform, false);
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = PosterLineA;
        tmp.fontSize = 3.5f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.rectTransform.sizeDelta = new Vector2(6f, 2f);

        // Poster that only appears after rewrite
        GameObject posterAfter = new GameObject("Poster_After");
        posterAfter.transform.position = corridorOrigin + new Vector2(2.2f, 1.2f);
        SpriteRenderer posterAfterRenderer = posterAfter.AddComponent<SpriteRenderer>();
        posterAfterRenderer.sprite = CreatePosterSprite();
        posterAfterRenderer.color = new Color(0.45f, 0.6f, 0.35f, 1f);
        posterAfter.SetActive(false);

        GameObject posterAfterTextObj = new GameObject("PosterAfterText");
        posterAfterTextObj.transform.SetParent(posterAfter.transform, false);
        posterAfterTextObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        TextMeshPro afterTmp = posterAfterTextObj.AddComponent<TextMeshPro>();
        afterTmp.text = "GLORY TO OCEANIA.";
        afterTmp.fontSize = 3.5f;
        afterTmp.alignment = TextAlignmentOptions.Center;
        afterTmp.color = Color.white;
        afterTmp.rectTransform.sizeDelta = new Vector2(6f, 2f);

        // Rewrite swap controlling text and color swap on the primary poster
        RewriteSwap posterSwap = poster.AddComponent<RewriteSwap>();
        posterSwap.ConfigureSpriteSwap(posterRenderer, posterRenderer.sprite, posterRenderer.sprite);
        posterSwap.ConfigureColorSwap(true, posterColorA, posterColorB);
        posterSwap.ConfigureTextSwap(tmp, PosterLineA, PosterLineB);
        posterSwap.RefreshState();

        // Swap controlling presence of Syme and the second poster
        GameObject swapToggleHolder = new GameObject("RewriteSwap_Toggles");
        RewriteSwap toggleSwap = swapToggleHolder.AddComponent<RewriteSwap>();
        toggleSwap.ConfigureActiveLists(new GameObject[] { syme }, new GameObject[] { posterAfter });
        toggleSwap.RefreshState();

        // Trigger to flip the world near the end of the corridor
        GameObject triggerObj = CreateTrigger();
        triggerObj.transform.position = corridorOrigin + new Vector2(corridorLength * 0.6f, 0f);
        triggerObj.transform.localScale = new Vector3(1.5f, 2f, 1f);
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

    private GameObject CreateTrigger()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RewriteTrigger");
        GameObject instance;
        if (prefab != null)
        {
            instance = Instantiate(prefab);
        }
        else
        {
            instance = new GameObject("RewriteTrigger");
            BoxCollider2D box = instance.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            instance.AddComponent<RewriteTrigger>();
        }

        return instance;
    }
}
