using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTelescreenDemo : MonoBehaviour
{
    [SerializeField] private Vector2 silentScreenPosition = new Vector2(2f, -1.5f);
    [SerializeField] private Vector2 chatterScreenPosition = new Vector2(8f, -1.5f);
    [SerializeField] private Vector2 triggerPosition = new Vector2(5.5f, -1.5f);
    [SerializeField] private string triggerLine = "6079 SMITH W. NO UNEXPECTED MOVEMENTS!";

    [SerializeField] private string[] idleLines =
    {
        "DO NOT RESIST.",
        "STATE YOUR PURPOSE.",
        "EYES ON THE CORRIDOR."
    };

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

        GameObject obj = new GameObject("LevelTelescreenDemo");
        obj.AddComponent<LevelTelescreenDemo>();
    }

    private void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Telescreen");
        if (prefab == null)
        {
            return;
        }

        CreateTelescreen(prefab, silentScreenPosition, false);
        TelescreenController chatterScreen = CreateTelescreen(prefab, chatterScreenPosition, true);
        if (chatterScreen != null)
        {
            chatterScreen.ConfigureIdleLines(true, idleLines, 4f, 8f);

            CreateLineTrigger(chatterScreen, triggerPosition, triggerLine);
        }
    }

    private TelescreenController CreateTelescreen(GameObject prefab, Vector2 position, bool idleEnabled)
    {
        var instance = Instantiate(prefab, position, Quaternion.identity);
        var controller = instance.GetComponent<TelescreenController>();
        if (controller != null)
        {
            controller.IdleLinesEnabled = idleEnabled;
        }

        return controller;
    }

    private void CreateLineTrigger(TelescreenController target, Vector2 position, string line)
    {
        GameObject triggerObj = new GameObject("TelescreenLineTrigger_Demo");
        triggerObj.transform.position = position;
        BoxCollider2D collider = triggerObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.5f, 2f);

        var trigger = triggerObj.AddComponent<TelescreenLineTrigger>();
        trigger.SetTarget(target);
        trigger.SetLineText(line);
    }
}
