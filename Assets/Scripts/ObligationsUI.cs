using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObligationsUI : MonoBehaviour
{
    private static ObligationsUI instance;

    public static ObligationsUI Instance => instance != null ? instance : GetOrCreate();

    [SerializeField] private float panelWidth = 280f;
    [SerializeField] private float panelPadding = 12f;
    [SerializeField] private float lineSpacing = 6f;

    private Canvas uiCanvas;
    private RectTransform panelRect;
    private TextMeshProUGUI titleText;
    private readonly List<TextMeshProUGUI> lineTexts = new List<TextMeshProUGUI>();
    private readonly List<LineData> lines = new List<LineData>();

    private struct LineData
    {
        public string text;
        public bool completed;
    }

    public static ObligationsUI GetOrCreate()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject obj = new GameObject("ObligationsUI");
        instance = obj.AddComponent<ObligationsUI>();
        return instance;
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
        CreateCanvas();
        CreatePanel();
    }

    private void CreateCanvas()
    {
        uiCanvas = gameObject.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 50;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();
    }

    private void CreatePanel()
    {
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(uiCanvas.transform, false);
        panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-panelPadding, -panelPadding);
        panelRect.sizeDelta = new Vector2(panelWidth, 10f);

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.6f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset((int)panelPadding, (int)panelPadding, (int)panelPadding, (int)panelPadding);
        layout.spacing = lineSpacing;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        titleText = CreateLineText("OBLIGATIONS:");
        titleText.fontStyle = FontStyles.Bold;
        titleText.fontSize = 22f;
    }

    private TextMeshProUGUI CreateLineText(string content)
    {
        GameObject textObj = new GameObject("Line");
        textObj.transform.SetParent(panelRect, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.text = content;
        tmp.raycastTarget = false;

        LayoutElement element = textObj.AddComponent<LayoutElement>();
        element.preferredWidth = panelWidth - (panelPadding * 2f);

        return tmp;
    }

    public void SetObligations(string[] linesInput)
    {
        Clear();

        if (linesInput == null || linesInput.Length == 0)
        {
            return;
        }

        for (int i = 0; i < linesInput.Length; i++)
        {
            SetLine(i, linesInput[i]);
        }
    }

    public void SetLine(int index, string text)
    {
        EnsureLineExists(index);
        lines[index] = new LineData { text = text, completed = false };
        RefreshLine(index);
    }

    public void MarkComplete(int index)
    {
        if (index < 0 || index >= lines.Count)
        {
            return;
        }

        LineData data = lines[index];
        data.completed = true;
        lines[index] = data;
        RefreshLine(index);
    }

    public void Show(bool visible)
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(visible);
        }
    }

    public void Clear()
    {
        lines.Clear();
        for (int i = 0; i < lineTexts.Count; i++)
        {
            Destroy(lineTexts[i].gameObject);
        }

        lineTexts.Clear();
    }

    private void EnsureLineExists(int index)
    {
        while (lines.Count <= index)
        {
            lines.Add(new LineData { text = string.Empty, completed = false });
        }

        while (lineTexts.Count <= index)
        {
            var text = CreateLineText("• ");
            lineTexts.Add(text);
        }
    }

    private void RefreshLine(int index)
    {
        if (index < 0 || index >= lineTexts.Count || index >= lines.Count)
        {
            return;
        }

        LineData data = lines[index];
        TextMeshProUGUI lineText = lineTexts[index];

        string prefix = data.completed ? "✓ " : "• ";
        lineText.text = prefix + data.text;
        lineText.color = data.completed ? new Color(0.6f, 1f, 0.6f, 0.85f) : Color.white;
    }
}
