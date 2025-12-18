using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    private static InteractionPromptUI instance;
    public static InteractionPromptUI Instance => instance != null ? instance : CreateInstance();

    [SerializeField] private Vector2 anchoredPosition = new Vector2(0f, 140f);
    [SerializeField] private int fontSize = 26;

    private RectTransform promptRoot;
    private TextMeshProUGUI promptText;

    private static InteractionPromptUI CreateInstance()
    {
        GameObject obj = new GameObject("InteractionPromptUI");
        return obj.AddComponent<InteractionPromptUI>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
    }

    private void BuildUI()
    {
        Canvas canvas = GetOrCreateCanvas();

        GameObject holder = new GameObject("Prompt");
        holder.transform.SetParent(canvas.transform, false);
        promptRoot = holder.AddComponent<RectTransform>();
        promptRoot.anchorMin = new Vector2(0.5f, 0f);
        promptRoot.anchorMax = new Vector2(0.5f, 0f);
        promptRoot.pivot = new Vector2(0.5f, 0f);
        promptRoot.anchoredPosition = anchoredPosition;
        promptRoot.sizeDelta = new Vector2(260f, 56f);

        Image backdrop = holder.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.5f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(holder.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);

        promptText = textObj.AddComponent<TextMeshProUGUI>();
        promptText.fontSize = fontSize;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.text = "PRESS E";
        promptText.color = Color.white;
        promptText.enableWordWrapping = false;
        promptText.raycastTarget = false;

        LayoutElement layout = holder.AddComponent<LayoutElement>();
        layout.preferredWidth = 220f;
        layout.preferredHeight = 50f;

        Hide();
    }

    private Canvas GetOrCreateCanvas()
    {
        Canvas canvas = null;
        if (ObligationsUI.Instance != null)
        {
            canvas = ObligationsUI.Instance.GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("UIRoot");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 75;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
        }

        transform.SetParent(canvas.transform, false);
        return canvas;
    }

    public void Show(string text)
    {
        if (promptRoot == null || promptText == null)
        {
            return;
        }

        promptText.text = text;
        promptRoot.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (promptRoot != null)
        {
            promptRoot.gameObject.SetActive(false);
        }
    }
}
