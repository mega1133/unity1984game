using TMPro;
using UnityEngine;

public class RewriteSwap : MonoBehaviour
{
    [Header("Active Toggles")]
    [SerializeField] private GameObject[] activeInA;
    [SerializeField] private GameObject[] activeInB;

    [Header("Sprite Swap")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;
    [SerializeField] private bool applyColorSwap = true;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.white;

    [Header("Text Swap")]
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private string textA;
    [SerializeField] private string textB;

    private void OnEnable()
    {
        RewriteManager.Instance.OnRewriteStateChanged += HandleRewriteChanged;
        ApplyState(RewriteManager.Instance.IsRewritten);
    }

    private void OnDisable()
    {
        if (RewriteManager.HasInstance)
        {
            RewriteManager.Instance.OnRewriteStateChanged -= HandleRewriteChanged;
        }
    }

    private void HandleRewriteChanged(bool isRewritten)
    {
        ApplyState(isRewritten);
    }

    private void ApplyState(bool isRewritten)
    {
        ToggleObjects(activeInA, !isRewritten);
        ToggleObjects(activeInB, isRewritten);
        ApplySprite(isRewritten);
        ApplyText(isRewritten);
    }

    public void ConfigureActiveLists(GameObject[] inA, GameObject[] inB)
    {
        activeInA = inA;
        activeInB = inB;
    }

    public void ConfigureSpriteSwap(SpriteRenderer renderer, Sprite inA, Sprite inB)
    {
        targetRenderer = renderer;
        spriteA = inA;
        spriteB = inB;
    }

    public void ConfigureColorSwap(bool enabled, Color inA, Color inB)
    {
        applyColorSwap = enabled;
        colorA = inA;
        colorB = inB;
    }

    public void ConfigureTextSwap(TMP_Text text, string inA, string inB)
    {
        targetText = text;
        textA = inA;
        textB = inB;
    }

    public void RefreshState()
    {
        ApplyState(RewriteManager.Instance.IsRewritten);
    }

    private void ToggleObjects(GameObject[] objects, bool desiredState)
    {
        if (objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null)
            {
                continue;
            }

            objects[i].SetActive(desiredState);
        }
    }

    private void ApplySprite(bool isRewritten)
    {
        if (targetRenderer == null)
        {
            return;
        }

        Sprite chosenSprite = isRewritten ? spriteB : spriteA;
        if (chosenSprite != null)
        {
            targetRenderer.sprite = chosenSprite;
        }

        if (applyColorSwap)
        {
            targetRenderer.color = isRewritten ? colorB : colorA;
        }
    }

    private void ApplyText(bool isRewritten)
    {
        if (targetText == null)
        {
            return;
        }

        targetText.text = isRewritten ? textB : textA;
    }
}
