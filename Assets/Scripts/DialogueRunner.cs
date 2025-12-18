using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    public static DialogueRunner Instance => instance != null ? instance : CreateInstance();

    [SerializeField] private DialogueBubble bubblePrefab;
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float transientLifetime = 2.5f;

    private static DialogueRunner instance;
    private bool isPlaying;
    private Coroutine dialogueRoutine;

    public bool IsPlaying => isPlaying;

    private static DialogueRunner CreateInstance()
    {
        GameObject obj = new GameObject("DialogueRunner");
        return obj.AddComponent<DialogueRunner>();
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

        if (bubblePrefab == null)
        {
            bubblePrefab = Resources.Load<DialogueBubble>("Prefabs/DialogueBubble");
        }
    }

    public void StartDialogue(List<DialogueLine> lines, Action onComplete = null)
    {
        if (isPlaying || lines == null || lines.Count == 0 || bubblePrefab == null)
        {
            return;
        }

        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
        }

        dialogueRoutine = StartCoroutine(RunDialogue(lines, onComplete));
    }

    public DialogueBubble ShowTransientBubble(Transform speaker, string text, float minDisplayTime = 0f, float lifetime = -1f)
    {
        if (bubblePrefab == null || string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (lifetime <= 0f)
        {
            lifetime = transientLifetime;
        }

        DialogueBubble bubble = Instantiate(bubblePrefab, speaker != null ? speaker.position + bubbleOffset : Vector3.zero, Quaternion.identity);
        bubble.Initialize(speaker, text);
        StartCoroutine(DestroyAfterDelay(bubble.gameObject, Mathf.Max(minDisplayTime, lifetime)));
        return bubble;
    }

    private IEnumerator RunDialogue(List<DialogueLine> lines, Action onComplete)
    {
        isPlaying = true;
        PlayerMovement player = FindPlayer();
        bool previousControl = player != null && player.IsControlEnabled;
        if (player != null)
        {
            player.SetControlEnabled(false);
        }

        DialogueBubble bubble = null;

        for (int i = 0; i < lines.Count; i++)
        {
            DialogueLine line = lines[i];
            Transform speaker = line.Speaker != null ? line.Speaker : (player != null ? player.transform : transform);

            if (bubble != null)
            {
                Destroy(bubble.gameObject);
            }

            bubble = Instantiate(bubblePrefab, speaker != null ? speaker.position + bubbleOffset : Vector3.zero, Quaternion.identity);
            bubble.Initialize(speaker, line.Text);

            float elapsed = 0f;
            float minTime = Mathf.Max(0f, line.MinDisplayTime);
            while (elapsed < minTime)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        if (bubble != null)
        {
            Destroy(bubble.gameObject);
        }

        if (player != null)
        {
            player.SetControlEnabled(previousControl);
        }

        isPlaying = false;
        dialogueRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator DestroyAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));
        if (target != null)
        {
            Destroy(target);
        }
    }

    private PlayerMovement FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj != null ? playerObj.GetComponent<PlayerMovement>() : null;
    }
}
