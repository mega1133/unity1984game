using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRocketBombDemo : MonoBehaviour
{
    [SerializeField] private Vector2 triggerPosition = new Vector2(16f, -2f);
    [SerializeField] private Vector2 triggerSize = new Vector2(1.2f, 2.5f);
    [SerializeField] private Vector2 strikePosition = new Vector2(17.2f, -2f);
    [SerializeField] private float warningDelay = 1.2f;
    [SerializeField] private float markerRadius = 0.7f;

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

        GameObject obj = new GameObject("LevelRocketBombDemo");
        obj.AddComponent<LevelRocketBombDemo>();
    }

    private void Start()
    {
        BuildRocketBomb();
    }

    private void BuildRocketBomb()
    {
        GameObject strikePoint = new GameObject("RocketStrikePoint");
        strikePoint.transform.position = strikePosition;

        RocketBombController controller = CreateController();
        controller.transform.position = strikePosition;
        controller.SetStrikePoints(new Transform[] { strikePoint.transform });
        controller.SetMarkerRadius(markerRadius);
        controller.SetWarningDelay(warningDelay);
        controller.SetMarkerDuration(warningDelay);

        GameObject trigger = CreateTrigger(controller);
        trigger.transform.position = triggerPosition;
        trigger.transform.localScale = triggerSize;
    }

    private RocketBombController CreateController()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RocketBomb");
        GameObject instance;
        if (prefab != null)
        {
            instance = Instantiate(prefab);
        }
        else
        {
            instance = new GameObject("RocketBomb");
            instance.AddComponent<AudioSource>();
            instance.AddComponent<RocketBombController>();
        }

        return instance.GetComponent<RocketBombController>();
    }

    private GameObject CreateTrigger(RocketBombController controller)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RocketBombTrigger");
        GameObject instance;
        if (prefab != null)
        {
            instance = Instantiate(prefab);
        }
        else
        {
            instance = new GameObject("RocketBombTrigger");
            BoxCollider2D box = instance.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            instance.AddComponent<RocketBombTrigger>();
        }

        RocketBombTrigger trigger = instance.GetComponent<RocketBombTrigger>();
        trigger.SetController(controller);
        return instance;
    }
}
