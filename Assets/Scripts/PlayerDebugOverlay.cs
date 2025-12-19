using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDebugOverlay : MonoBehaviour
{
    private static PlayerDebugOverlay instance;
    public static PlayerDebugOverlay Instance => instance;

    [SerializeField] private KeyCode toggleKey = KeyCode.F1;

    private Canvas canvas;
    private TextMeshProUGUI text;
    private bool visible;
    private PlayerMovement player;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("PlayerDebugOverlay");
            instance = obj.AddComponent<PlayerDebugOverlay>();
            DontDestroyOnLoad(obj);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            visible = !visible;
            if (canvas != null)
            {
                canvas.gameObject.SetActive(visible);
            }
        }

        if (!visible)
        {
            return;
        }

        if (player == null)
        {
            player = FindPlayer();
        }

        if (player == null || text == null)
        {
            return;
        }

        text.text = $"Grounded: {player.DebugIsGrounded}\nVelY: {player.VelocityY:F2}\nLastJump: {player.LastJumpPressedTime:F2}";
    }

    private PlayerMovement FindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        return obj != null ? obj.GetComponent<PlayerMovement>() : null;
    }

    private void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 110;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(transform, false);
        text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(12f, -12f);

        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
