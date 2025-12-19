using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSafeZoneDemo : MonoBehaviour
{
    [SerializeField] private Vector2 safeZoneCenter = new Vector2(-2f, -2f);
    [SerializeField] private Vector2 safeZoneSize = new Vector2(3f, 1f);

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

        GameObject obj = new GameObject("LevelSafeZoneDemo");
        obj.AddComponent<LevelSafeZoneDemo>();
    }

    private void Start()
    {
        BuildSafeZone();
    }

    private void BuildSafeZone()
    {
        GameObject zone = new GameObject("SafeZone_Patch");
        zone.transform.position = safeZoneCenter;
        zone.transform.localScale = safeZoneSize;

        var sprite = zone.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.25f, 0.85f, 0.25f, 0.9f));

        var collider = zone.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = Vector2.one;

        zone.AddComponent<SafeZone>();
    }
}
