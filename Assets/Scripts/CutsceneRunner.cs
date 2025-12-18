using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneRunner : MonoBehaviour
{
    private static CutsceneRunner instance;
    public static CutsceneRunner Instance => instance != null ? instance : CreateInstance();

    private bool isRunning;
    private Coroutine runRoutine;

    private static CutsceneRunner CreateInstance()
    {
        GameObject obj = new GameObject("CutsceneRunner");
        return obj.AddComponent<CutsceneRunner>();
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
    }

    public bool IsRunning => isRunning;

    public void Play(CutsceneSequence sequence)
    {
        if (sequence == null || isRunning)
        {
            return;
        }

        if (runRoutine != null)
        {
            StopCoroutine(runRoutine);
        }

        runRoutine = StartCoroutine(RunSequence(sequence));
    }

    private IEnumerator RunSequence(CutsceneSequence sequence)
    {
        isRunning = true;
        IReadOnlyList<CutsceneStep> steps = sequence.Steps;

        for (int i = 0; i < steps.Count; i++)
        {
            CutsceneStep step = steps[i];
            switch (step.stepType)
            {
                case CutsceneStep.StepType.LockPlayer:
                    HandleLock(step.lockPlayer);
                    break;
                case CutsceneStep.StepType.MovePlayer:
                    yield return MovePlayer(step.moveDistance, Mathf.Max(0.01f, step.moveSpeed));
                    break;
                case CutsceneStep.StepType.Wait:
                    yield return new WaitForSeconds(Mathf.Max(0f, step.waitSeconds));
                    break;
                case CutsceneStep.StepType.Dialogue:
                    yield return RunDialogue(step.dialogueLines);
                    break;
                case CutsceneStep.StepType.ShowObligations:
                    HandleObligations(step.obligationsVisible);
                    break;
                case CutsceneStep.StepType.End:
                    HandleLock(false);
                    i = steps.Count;
                    break;
            }
        }

        HandleLock(false);
        isRunning = false;
        runRoutine = null;
    }

    private void HandleLock(bool locked)
    {
        PlayerMovement player = FindPlayer();
        if (player != null)
        {
            player.SetControlEnabled(!locked);
            if (locked)
            {
                ZeroHorizontal(player);
            }
        }
    }

    private IEnumerator MovePlayer(float dx, float speed)
    {
        PlayerMovement player = FindPlayer();
        if (player == null || Mathf.Approximately(dx, 0f))
        {
            yield break;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        float distanceRemaining = Mathf.Abs(dx);
        float direction = Mathf.Sign(dx);

        while (distanceRemaining > 0f)
        {
            float step = speed * Time.deltaTime;
            float move = Mathf.Min(step, distanceRemaining);
            distanceRemaining -= move;

            if (rb != null)
            {
                rb.velocity = new Vector2(direction * speed, rb.velocity.y);
            }
            else
            {
                player.transform.position += new Vector3(direction * move, 0f, 0f);
            }

            yield return null;
        }

        if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private IEnumerator RunDialogue(IReadOnlyList<DialogueLine> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            yield break;
        }

        DialogueRunner runner = DialogueRunner.Instance;
        bool finished = false;
        runner.StartDialogue(new List<DialogueLine>(lines), () => finished = true);

        while (!finished)
        {
            yield return null;
        }
    }

    private void HandleObligations(bool visible)
    {
        var ui = ObligationsUI.Instance;
        if (ui != null)
        {
            ui.Show(visible);
        }
    }

    private PlayerMovement FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj != null ? playerObj.GetComponent<PlayerMovement>() : null;
    }

    private void ZeroHorizontal(PlayerMovement player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }
}
