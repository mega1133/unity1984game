using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionUI : MonoBehaviour
{
    private static SuspicionUI instance;
    public static SuspicionUI Instance => instance != null ? instance : CreateInstance();

    [SerializeField] private string title = "SUSPICION";
    [SerializeField] private Color barColor = new Color(0.9f, 0.4f, 0.25f, 0.85f);

    private Canvas canvas;
    private Image fillImage;

    private static SuspicionUI CreateInstance()
    {
        GameObject obj = new GameObject("SuspicionUI");
        return obj.AddComponent<SuspicionUI>();
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

    public void Show(float normalized)
    {
        if (canvas == null)
        {
            return;
        }

        canvas.gameObject.SetActive(true);
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(normalized);
        }
    }

    public void Hide()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
            if (fillImage != null)
            {
                fillImage.fillAmount = 0f;
            }
        }
    }

    private void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform panel = CreatePanel(canvas.transform);
        CreateLabel(panel, title);
        fillImage = CreateBar(panel);
    }

    private RectTransform CreatePanel(Transform parent)
    {
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(parent, false);
        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -18f);
        rect.sizeDelta = new Vector2(280f, 52f);

        Image bg = panelObj.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        VerticalLayoutGroup layout = panelObj.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 8, 10);
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.UpperCenter;

        ContentSizeFitter fitter = panelObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        return rect;
    }

    private void CreateLabel(RectTransform parent, string text)
    {
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 20f;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
    }

    private Image CreateBar(RectTransform parent)
    {
        GameObject barRoot = new GameObject("Bar");
        barRoot.transform.SetParent(parent, false);

        RectTransform rect = barRoot.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(240f, 14f);

        Image bg = barRoot.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(barRoot.transform, false);
        Image img = fillObj.AddComponent<Image>();
        img.color = barColor;
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillOrigin = (int)Image.OriginHorizontal.Left;
        img.fillAmount = 0f;

        RectTransform fillRect = img.rectTransform;
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = new Vector2(0f, 0f);
        fillRect.offsetMax = new Vector2(0f, 0f);

        return img;
    }
}
