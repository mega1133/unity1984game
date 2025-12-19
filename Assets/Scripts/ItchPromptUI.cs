using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItchPromptUI : MonoBehaviour
{
    private static ItchPromptUI instance;
    public static ItchPromptUI Instance => instance != null ? instance : CreateInstance();

    [SerializeField] private string promptText = "YOU ARE ITCHY. PRESS R TO SCRATCH.";

    private Canvas canvas;
    private TextMeshProUGUI text;

    public bool IsVisible => canvas != null && canvas.gameObject.activeSelf;

    private static ItchPromptUI CreateInstance()
    {
        GameObject obj = new GameObject("ItchPromptUI");
        return obj.AddComponent<ItchPromptUI>();
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
        Hide();
    }

    public void Show()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            text.text = promptText;
        }
    }

    public void Hide()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 62;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        GameObject textObj = new GameObject("Prompt");
        textObj.transform.SetParent(canvas.transform, false);
        text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 24f;
        text.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        text.enableWordWrapping = false;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -28f);
        rect.sizeDelta = new Vector2(520f, 40f);
    }
}
