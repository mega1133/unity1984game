using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewriteManager : MonoBehaviour
{
    private static RewriteManager instance;
    public static RewriteManager Instance => instance != null ? instance : CreateInstance();
    public static bool HasInstance => instance != null;

    public event Action<bool> OnRewriteStateChanged;

    [SerializeField] private float noticeDuration = 2f;

    private bool isRewritten;
    private Coroutine noticeRoutine;
    private Canvas noticeCanvas;
    private TextMeshProUGUI noticeText;

    public bool IsRewritten => isRewritten;

    private static RewriteManager CreateInstance()
    {
        GameObject obj = new GameObject("RewriteManager");
        return obj.AddComponent<RewriteManager>();
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
        SceneManager.sceneLoaded += HandleSceneLoaded;
        BuildNoticeUI();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Keep notice canvas on top across scenes.
        if (noticeCanvas != null)
        {
            noticeCanvas.sortingOrder = 95;
        }

        // Re-apply current state for any listeners that may query the property on enable.
        OnRewriteStateChanged?.Invoke(isRewritten);
    }

    public void SetRewritten(bool value)
    {
        if (isRewritten == value)
        {
            return;
        }

        isRewritten = value;
        OnRewriteStateChanged?.Invoke(isRewritten);

        if (isRewritten)
        {
            ShowNotice();
        }
    }

    public void TriggerRewrite()
    {
        SetRewritten(true);
    }

    public void ResetRewrite()
    {
        SetRewritten(false);
    }

    private void BuildNoticeUI()
    {
        GameObject canvasObj = new GameObject("RewriteNoticeCanvas");
        noticeCanvas = canvasObj.AddComponent<Canvas>();
        noticeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        noticeCanvas.sortingOrder = 95;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasObj);

        GameObject textObj = new GameObject("RewriteNoticeText");
        textObj.transform.SetParent(canvasObj.transform, false);
        noticeText = textObj.AddComponent<TextMeshProUGUI>();
        noticeText.text = "CORRECTION ISSUED.";
        noticeText.alignment = TextAlignmentOptions.Center;
        noticeText.fontSize = 32f;
        noticeText.color = new Color(0.9f, 0.9f, 0.9f, 0.95f);
        noticeText.raycastTarget = false;

        RectTransform rect = noticeText.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        canvasObj.SetActive(false);
    }

    private void ShowNotice()
    {
        if (noticeRoutine != null)
        {
            StopCoroutine(noticeRoutine);
        }

        noticeRoutine = StartCoroutine(NoticeRoutine());
    }

    private IEnumerator NoticeRoutine()
    {
        if (noticeCanvas != null)
        {
            noticeCanvas.gameObject.SetActive(true);
        }

        float timer = noticeDuration;
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (noticeCanvas != null)
        {
            noticeCanvas.gameObject.SetActive(false);
        }

        noticeRoutine = null;
    }
}
