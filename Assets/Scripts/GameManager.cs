using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float failDisplayDuration = 1f;

    private Vector2 respawnPosition;
    private string respawnSceneName;
    private bool hasCheckpoint;
    private Coroutine failRoutine;

    private Canvas failCanvas;
    private Text failText;

    public bool HasCheckpoint => hasCheckpoint;
    public string RespawnSceneName => respawnSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateFailUI();
    }

    public void SetCheckpoint(Vector2 position)
    {
        respawnPosition = position;
        respawnSceneName = SceneManager.GetActiveScene().name;
        hasCheckpoint = true;
    }

    public bool HasCheckpointForScene(string sceneName)
    {
        return hasCheckpoint && respawnSceneName == sceneName;
    }

    public void Fail(string reason)
    {
        if (!hasCheckpoint)
        {
            var player = FindPlayer();
            if (player != null)
            {
                SetCheckpoint(player.transform.position);
            }
        }

        if (failRoutine != null)
        {
            StopCoroutine(failRoutine);
        }

        failRoutine = StartCoroutine(FailFlow(reason));
    }

    public void Respawn()
    {
        if (failRoutine != null)
        {
            StopCoroutine(failRoutine);
            failRoutine = null;
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator FailFlow(string reason)
    {
        var player = FindPlayer();
        if (player != null)
        {
            player.SetControlEnabled(false);
            player.ResetMotion();
        }

        ShowReason(reason);
        yield return new WaitForSeconds(failDisplayDuration);

        yield return RespawnRoutine();
        failRoutine = null;
    }

    private IEnumerator RespawnRoutine()
    {
        if (!hasCheckpoint)
        {
            yield break;
        }

        string targetScene = respawnSceneName;
        if (SceneManager.GetActiveScene().name != targetScene)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene);
            while (!loadOp.isDone)
            {
                yield return null;
            }
        }

        yield return null;

        var player = FindPlayer();
        if (player != null)
        {
            player.ResetMotion();
            player.ClearTemporaryState();
            player.transform.position = respawnPosition;
            player.SetControlEnabled(true);
        }

        HideReason();
    }

    private void CreateFailUI()
    {
        GameObject canvasObject = new GameObject("FailCanvas");
        failCanvas = canvasObject.AddComponent<Canvas>();
        failCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasObject);

        GameObject textObject = new GameObject("FailText");
        textObject.transform.SetParent(canvasObject.transform, false);
        failText = textObject.AddComponent<Text>();
        failText.alignment = TextAnchor.MiddleCenter;
        failText.fontSize = 28;
        failText.color = Color.red;
        failText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        RectTransform rectTransform = failText.rectTransform;
        rectTransform.anchorMin = new Vector2(0.25f, 0.45f);
        rectTransform.anchorMax = new Vector2(0.75f, 0.55f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        HideReason();
    }

    private void ShowReason(string reason)
    {
        if (failCanvas == null || failText == null)
        {
            return;
        }

        failText.text = reason;
        failCanvas.gameObject.SetActive(true);
    }

    private void HideReason()
    {
        if (failCanvas != null)
        {
            failCanvas.gameObject.SetActive(false);
        }
    }

    private PlayerMovement FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.GetComponent<PlayerMovement>() : null;
    }
}
