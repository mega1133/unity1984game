using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryUI : MonoBehaviour
{
    private static DiaryUI instance;
    public static DiaryUI Instance => instance != null ? instance : CreateInstance();

    [SerializeField] private string title = "DIARY";
    [SerializeField] private string body = "WRITING...";
    [SerializeField] private string hint = "SPACE to close";

    private Canvas canvas;
    private RectTransform panel;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI bodyText;
    private TextMeshProUGUI hintText;

    public bool IsVisible => canvas != null && canvas.gameObject.activeSelf;

    private static DiaryUI CreateInstance()
    {
        GameObject obj = new GameObject("DiaryUI");
        return obj.AddComponent<DiaryUI>();
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
        BuildCanvas();
        BuildPanel();
        Hide();
    }

    private void BuildCanvas()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 75;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();
    }

    private void BuildPanel()
    {
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvas.transform, false);
        panel = panelObj.AddComponent<RectTransform>();
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(360f, 200f);

        Image bg = panelObj.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.8f);

        VerticalLayoutGroup layout = panelObj.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 16, 16);
        layout.spacing = 12f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        titleText = CreateText(panel, 24f, FontStyles.Bold);
        bodyText = CreateText(panel, 20f, FontStyles.Normal);
        hintText = CreateText(panel, 16f, FontStyles.Italic);

        RefreshText();
    }

    private TextMeshProUGUI CreateText(RectTransform parent, float size, FontStyles style)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;
        return tmp;
    }

    private void RefreshText()
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = body;
        }

        if (hintText != null)
        {
            hintText.text = hint;
        }
    }

    public void Show()
    {
        RefreshText();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
