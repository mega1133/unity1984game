using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFrameworkDemo : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPosition = new Vector2(0f, -2f);
    [SerializeField] private Vector2 exitOffset = new Vector2(6f, 0f);
    [SerializeField] private Vector2 completeTriggerOffset = new Vector2(3f, 0f);

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

        GameObject obj = new GameObject("LevelFrameworkDemo");
        obj.AddComponent<LevelFrameworkDemo>();
    }

    private void Start()
    {
        Vector2 playerPos = FindPlayerPosition(spawnPosition);
        CreateLevelController(playerPos);
        CreateSpawnPoint(playerPos);
        CreateObligationCompleteTrigger(playerPos + completeTriggerOffset);
        CreateExit(playerPos + exitOffset);
    }

    private Vector2 FindPlayerPosition(Vector2 fallback)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.transform.position;
        }

        return fallback;
    }

    private void CreateLevelController(Vector2 atPosition)
    {
        GameObject controllerObj = new GameObject("LevelController");
        controllerObj.transform.position = atPosition;

        LevelController controller = controllerObj.AddComponent<LevelController>();

        var initialLines = new[]
        {
            "Meet Parsons near the canteen",
            "Avoid telescreen glare",
            "Report to O'Brien"
        };

        controller.Configure("Level0", "Level0_TestB", initialLines, false);
    }

    private void CreateSpawnPoint(Vector2 position)
    {
        GameObject spawn = new GameObject("LevelSpawnPoint");
        spawn.transform.position = position;
        spawn.AddComponent<LevelSpawnPoint>();
    }

    private void CreateObligationCompleteTrigger(Vector2 position)
    {
        GameObject trigger = new GameObject("CompleteObligationTrigger");
        trigger.transform.position = position;
        BoxCollider2D col = trigger.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.2f, 2f);

        var sprite = trigger.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.35f, 0.8f, 0.4f, 0.6f));

        trigger.AddComponent<LevelObligationCompleteTrigger>();
    }

    private void CreateExit(Vector2 position)
    {
        GameObject exit = new GameObject("LevelExit_LevelController");
        exit.transform.position = position;

        var sprite = exit.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.3f, 0.9f, 0.9f, 0.7f));

        BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.5f, 2f);

        LevelExit exitComp = exit.AddComponent<LevelExit>();
        exitComp.SetMode(LevelExit.ExitMode.UseLevelControllerNextScene);
    }
}
