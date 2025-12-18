using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelItchDemo : MonoBehaviour
{
    [SerializeField] private Vector2 triggerPosition = new Vector2(5f, -2f);
    [SerializeField] private Vector2 triggerSize = new Vector2(2f, 2f);

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

        GameObject obj = new GameObject("LevelItchDemo");
        obj.AddComponent<LevelItchDemo>();
    }

    private void Start()
    {
        BuildTrigger();
    }

    private void BuildTrigger()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/ItchTrigger");
        GameObject trigger;

        if (prefab != null)
        {
            trigger = Instantiate(prefab);
        }
        else
        {
            trigger = new GameObject("ItchTrigger");
            BoxCollider2D box = trigger.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            trigger.AddComponent<ItchPromptTrigger>();
        }

        trigger.transform.position = triggerPosition;
        trigger.transform.localScale = triggerSize;
    }
}
