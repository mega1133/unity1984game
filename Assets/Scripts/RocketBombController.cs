using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RocketBombController : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float warningDelay = 1.5f;
    [SerializeField] private float markerDuration = 1.5f;

    [Header("Strike")]
    [SerializeField] private float markerRadius = 0.6f;
    [SerializeField] private string failReason = "ROCKET BOMB";
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private Transform[] strikePoints;
    [SerializeField] private bool randomizeStrikePoint = false;

    [Header("Grounding")]
    [SerializeField] private bool useGroundRaycast = true;
    [SerializeField] private float groundRayDistance = 6f;
    [SerializeField] private LayerMask groundMask;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip whistleClip;
    [SerializeField] private float whistleVolume = 0.75f;
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private float impactVolume = 0.95f;

    [Header("Marker")]
    [SerializeField] private Transform markerTransform;
    [SerializeField] private SpriteRenderer markerRenderer;
    [SerializeField] private Color markerColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private Coroutine strikeRoutine;
    private bool hasFired;
    private bool isStriking;
    private bool failSent;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        ConfigureAudioSource();
        EnsureMarker();
        HideMarker();
        EnsureAudioClips();
    }

    private void OnEnable()
    {
        if (autoStart)
        {
            StartStrike();
        }
    }

    public void StartStrike()
    {
        if (isStriking)
        {
            return;
        }

        if (triggerOnce && hasFired)
        {
            return;
        }

        strikeRoutine = StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        isStriking = true;
        hasFired = true;
        failSent = false;

        Transform targetPoint = ChooseStrikePoint();
        Vector3 strikePosition = GetStrikePosition(targetPoint);

        ShowMarker(strikePosition);
        PlayWhistle();

        float delay = Mathf.Max(0.01f, warningDelay);
        float showTime = Mathf.Max(delay, markerDuration);

        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return Impact(strikePosition);

        float remaining = showTime - delay;
        if (remaining > 0f)
        {
            yield return new WaitForSeconds(remaining);
        }

        HideMarker();
        strikeRoutine = null;
        isStriking = false;
    }

    private Transform ChooseStrikePoint()
    {
        if (strikePoints == null || strikePoints.Length == 0)
        {
            return transform;
        }

        if (!randomizeStrikePoint || strikePoints.Length == 1)
        {
            return strikePoints[0] != null ? strikePoints[0] : transform;
        }

        int index = Random.Range(0, strikePoints.Length);
        return strikePoints[index] != null ? strikePoints[index] : transform;
    }

    private Vector3 GetStrikePosition(Transform strikePoint)
    {
        Vector3 position = strikePoint != null ? strikePoint.position : transform.position;

        if (!useGroundRaycast)
        {
            return position;
        }

        RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up * groundRayDistance * 0.5f, Vector2.down, groundRayDistance, groundMask);
        if (hit.collider != null)
        {
            position.y = hit.point.y;
        }

        return position;
    }

    private IEnumerator Impact(Vector3 strikePosition)
    {
        PlayImpact();
        yield return FlashMarker();
        CheckHit(strikePosition);
    }

    private void CheckHit(Vector3 strikePosition)
    {
        if (failSent)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(player.transform.position, strikePosition);
        if (distance <= markerRadius)
        {
            failSent = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Fail(string.IsNullOrEmpty(failReason) ? "ROCKET BOMB" : failReason);
            }
        }
    }

    private void ConfigureAudioSource()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void PlayWhistle()
    {
        if (audioSource == null || whistleClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(whistleClip, whistleVolume);
    }

    private void PlayImpact()
    {
        if (audioSource == null || impactClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(impactClip, impactVolume);
    }

    private void EnsureAudioClips()
    {
        if (whistleClip == null)
        {
            whistleClip = CreateWhistleClip();
        }

        if (impactClip == null)
        {
            impactClip = CreateImpactClip();
        }
    }

    private AudioClip CreateWhistleClip()
    {
        const int sampleRate = 44100;
        float duration = Mathf.Max(0.4f, warningDelay * 0.6f);
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float freq = Mathf.Lerp(880f, 440f, t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.2f;
        }

        AudioClip clip = AudioClip.Create("Whistle", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateImpactClip()
    {
        const int sampleRate = 44100;
        const float duration = 0.3f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float decay = Mathf.Exp(-12f * t);
            data[i] = (Random.value * 2f - 1f) * decay * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Impact", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private void ShowMarker(Vector3 position)
    {
        EnsureMarker();
        if (markerTransform != null)
        {
            markerTransform.position = position;
            markerTransform.localScale = Vector3.one * markerRadius * 2f;
        }

        if (markerRenderer != null)
        {
            markerRenderer.color = markerColor;
            markerRenderer.enabled = true;
        }
    }

    private void HideMarker()
    {
        if (markerRenderer != null)
        {
            markerRenderer.enabled = false;
        }
    }

    public void SetStrikePoints(Transform[] points)
    {
        strikePoints = points;
    }

    public void SetMarkerRadius(float radius)
    {
        markerRadius = Mathf.Max(0.05f, radius);
    }

    public void SetWarningDelay(float delay)
    {
        warningDelay = Mathf.Max(0.01f, delay);
        markerDuration = Mathf.Max(markerDuration, warningDelay);
    }

    public void SetMarkerDuration(float duration)
    {
        markerDuration = Mathf.Max(0.01f, duration);
    }

    private IEnumerator FlashMarker()
    {
        if (markerRenderer == null)
        {
            yield break;
        }

        Color original = markerRenderer.color;
        markerRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        markerRenderer.color = original;
    }

    private void EnsureMarker()
    {
        if (markerTransform != null && markerRenderer != null)
        {
            return;
        }

        GameObject marker = new GameObject("ShadowMarker");
        marker.transform.SetParent(transform);
        markerTransform = marker.transform;

        markerRenderer = marker.AddComponent<SpriteRenderer>();
        PlaceholderSprite sprite = marker.AddComponent<PlaceholderSprite>();
        sprite.SetColor(markerColor);
        markerRenderer.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Transform point = ChooseStrikePoint();
        Vector3 position = GetStrikePosition(point);
        Gizmos.DrawWireSphere(position, markerRadius);
    }
}
