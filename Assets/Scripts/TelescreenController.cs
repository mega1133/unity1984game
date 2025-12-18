using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class TelescreenController : MonoBehaviour
{
    [Header("Hum")]
    [SerializeField] private AudioClip humClip;
    [SerializeField] private float humVolume = 0.25f;
    [SerializeField] private bool playOnStart = true;

    [Header("Lines")]
    [SerializeField] private float bubbleLifetime = 2.5f;

    [Header("Idle Lines (optional)")]
    [SerializeField] private bool idleLinesEnabled = false;
    [SerializeField] private string[] idleLines;
    [SerializeField] private float idleIntervalMin = 6f;
    [SerializeField] private float idleIntervalMax = 10f;

    private AudioSource audioSource;
    private Coroutine idleRoutine;
    private Coroutine bubbleRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();
        EnsureHumClip();
    }

    private void OnEnable()
    {
        if (playOnStart)
        {
            StartHum();
        }

        StartIdleRoutine();
    }

    private void OnDisable()
    {
        StopHum();
        StopIdleRoutine();
    }

    public void PlayLine(string text, float minTime = 0f)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (bubbleRoutine != null)
        {
            StopCoroutine(bubbleRoutine);
            bubbleRoutine = null;
        }

        bubbleRoutine = StartCoroutine(PlayLineRoutine(text, Mathf.Max(0f, minTime)));
    }

    private IEnumerator PlayLineRoutine(string text, float minTime)
    {
        DialogueRunner runner = DialogueRunner.Instance;
        if (runner == null)
        {
            yield break;
        }

        DialogueBubble bubble = runner.ShowTransientBubble(transform, text, minTime, bubbleLifetime);
        float duration = Mathf.Max(minTime, bubbleLifetime);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        bubbleRoutine = null;
    }

    private void StartHum()
    {
        if (audioSource == null)
        {
            return;
        }

        if (humClip == null)
        {
            EnsureHumClip();
        }

        audioSource.clip = humClip;
        audioSource.volume = humVolume;
        audioSource.loop = true;

        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void StopHum()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void StartIdleRoutine()
    {
        if (!idleLinesEnabled || idleLines == null || idleLines.Length == 0)
        {
            return;
        }

        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
        }

        idleRoutine = StartCoroutine(IdleRoutine());
    }

    private void StopIdleRoutine()
    {
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }
    }

    private IEnumerator IdleRoutine()
    {
        while (idleLinesEnabled && idleLines != null && idleLines.Length > 0)
        {
            float wait = Random.Range(idleIntervalMin, idleIntervalMax);
            yield return new WaitForSeconds(wait);

            if (!idleLinesEnabled)
            {
                break;
            }

            string line = idleLines[Random.Range(0, idleLines.Length)];
            PlayLine(line);
        }

        idleRoutine = null;
    }

    private void ConfigureAudioSource()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void EnsureHumClip()
    {
        if (humClip != null)
        {
            return;
        }

        const int sampleRate = 44100;
        const float freq = 110f;
        const float duration = 1f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.1f;
        }

        humClip = AudioClip.Create("HumTone", samples, 1, sampleRate, false);
        humClip.SetData(data, 0);
    }
}
