using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string levelId = "";
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private string[] initialObligations = null;
    [SerializeField] private bool hideObligationsOnStart = false;

    private void Start()
    {
        var ui = ObligationsUI.GetOrCreate();
        if (initialObligations != null && initialObligations.Length > 0)
        {
            ui.SetObligations(initialObligations);
        }
        else
        {
            ui.Clear();
        }

        ui.Show(!hideObligationsOnStart);
    }

    public void CompleteObligation(int index)
    {
        ObligationsUI.Instance.MarkComplete(index);
    }

    public void SetObligationText(int index, string text)
    {
        ObligationsUI.Instance.SetLine(index, text);
    }

    public void ShowObligations(bool visible)
    {
        ObligationsUI.Instance.Show(visible);
    }

    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("LevelController: Next scene name not set.");
            return;
        }

        FadeManager.GetOrCreate()?.LoadSceneWithFade(nextSceneName);
    }

    public string LevelId => levelId;
    public string NextSceneName => nextSceneName;
    public bool HideObligationsOnStart => hideObligationsOnStart;

    public void Configure(string newLevelId, string newNextScene, string[] obligations, bool hideOnStart)
    {
        levelId = newLevelId;
        nextSceneName = newNextScene;
        initialObligations = obligations;
        hideObligationsOnStart = hideOnStart;
    }
}
