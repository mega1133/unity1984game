using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSuspicionDemo : MonoBehaviour
{
    [SerializeField] private Vector2 zoneCenter = new Vector2(8f, -2f);
    [SerializeField] private Vector2 zoneSize = new Vector2(3f, 2f);
    [SerializeField] private float demoSecondsToMax = 2.5f;

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

        GameObject obj = new GameObject("LevelSuspicionDemo");
        obj.AddComponent<LevelSuspicionDemo>();
    }

    private void Start()
    {
        BuildZone();
    }

    private void BuildZone()
    {
        GameObject zone = new GameObject("SuspicionZone_Demo");
        zone.transform.position = zoneCenter;
        zone.transform.localScale = zoneSize;

        var sprite = zone.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.75f, 0.35f, 0.15f, 0.85f));

        var collider = zone.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = Vector2.one;

        var suspicion = zone.AddComponent<SuspicionZone>();
        suspicion.SetSecondsToMax(demoSecondsToMax);
    }
}
