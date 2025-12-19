using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObligationsDemo : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DemoRoutine());
    }

    private IEnumerator DemoRoutine()
    {
        if (FindObjectOfType<LevelController>() != null)
        {
            yield break;
        }

        var ui = ObligationsUI.GetOrCreate();
        ui.Show(true);
        ui.SetObligations(new[]
        {
            "Locate the hidden poster",
            "Avoid Thought Police",
            "Report back to O'Brien"
        });

        yield return new WaitForSeconds(2f);
        ui.MarkComplete(0);

        yield return new WaitForSeconds(2f);
        ui.SetLine(1, "Stay unseen while moving between rooftops");

        yield return new WaitForSeconds(2f);
        ui.Show(false);

        yield return new WaitForSeconds(0.25f);
        ui.Show(true);
    }

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

        GameObject demo = new GameObject("LevelObligationsDemo");
        demo.AddComponent<LevelObligationsDemo>();
    }
}
