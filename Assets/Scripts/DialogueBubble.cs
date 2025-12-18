using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);

    private Transform target;
    private Camera mainCamera;
    private TextMeshProUGUI lineText;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lineText = GetComponentInChildren<TextMeshProUGUI>();
        mainCamera = Camera.main;

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 200;
        }

        if (rectTransform != null && rectTransform.localScale == Vector3.one)
        {
            rectTransform.localScale = Vector3.one * 0.01f;
        }

        if (lineText == null)
        {
            BuildUI();
        }
    }

    public void Initialize(Transform followTarget, string text)
    {
        target = followTarget;
        if (lineText != null)
        {
            lineText.text = text;
        }

        UpdatePosition();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (rectTransform != null)
        {
            rectTransform.position = target.position + worldOffset;
            if (mainCamera != null)
            {
                rectTransform.rotation = mainCamera.transform.rotation;
            }
            else
            {
                rectTransform.rotation = Quaternion.identity;
            }
        }
    }

    private void BuildUI()
    {
        GameObject background = new GameObject("Background");
        background.transform.SetParent(transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0f);

        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.8f);

        HorizontalLayoutGroup layout = background.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 8, 8);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = background.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(background.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.pivot = new Vector2(0.5f, 0.5f);

        lineText = textObj.AddComponent<TextMeshProUGUI>();
        lineText.fontSize = 20f;
        lineText.color = Color.white;
        lineText.alignment = TextAlignmentOptions.Midline;
        lineText.enableWordWrapping = true;
        lineText.text = string.Empty;
        lineText.raycastTarget = false;

        LayoutElement layoutElement = textObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 260f;
    }
}
