using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlaceholderSprite : MonoBehaviour
{
    [SerializeField] private Color spriteColor = Color.white;

    private static Sprite cachedSprite;

    private void Awake()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (cachedSprite == null)
        {
            cachedSprite = CreateSquareSprite();
        }

        renderer.sprite = cachedSprite;
        renderer.color = spriteColor;
    }

    public void SetColor(Color color)
    {
        spriteColor = color;

        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = spriteColor;
        }
    }

    private Sprite CreateSquareSprite()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size)
        {
            filterMode = FilterMode.Point
        };

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
    }
}
