using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerScratch : MonoBehaviour
{
    [SerializeField] private float scratchDuration = 1f;
    [SerializeField] private string animatorTrigger = "Scratch";

    private PlayerMovement movement;
    private Animator animator;
    private Coroutine scratchRoutine;
    private bool waitingPrompt;

    public bool IsScratching => scratchRoutine != null;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartScratch();
        }
    }

    public void StartScratch()
    {
        if (IsScratching)
        {
            return;
        }

        if (movement == null || !movement.IsControlEnabled)
        {
            return;
        }

        if ((DiaryUI.Instance != null && DiaryUI.Instance.IsVisible) ||
            (DialogueRunner.Instance != null && DialogueRunner.Instance.IsPlaying) ||
            (CutsceneRunner.Instance != null && CutsceneRunner.Instance.IsRunning))
        {
            return;
        }

        scratchRoutine = StartCoroutine(ScratchRoutine());
    }

    private IEnumerator ScratchRoutine()
    {
        bool previousControl = movement.IsControlEnabled;
        movement.SetControlEnabled(false);
        ZeroHorizontal();

        if (animator != null && !string.IsNullOrEmpty(animatorTrigger))
        {
            animator.SetTrigger(animatorTrigger);
        }

        yield return new WaitForSeconds(Mathf.Max(0.01f, scratchDuration));

        movement.SetControlEnabled(previousControl);
        scratchRoutine = null;

        if (waitingPrompt)
        {
            waitingPrompt = false;
            ItchPromptUI.Instance.Hide();
        }
    }

    public void ShowItchPromptOnce()
    {
        waitingPrompt = true;
        ItchPromptUI.Instance.Show();
    }

    public void ResetScratchState()
    {
        if (scratchRoutine != null)
        {
            StopCoroutine(scratchRoutine);
            scratchRoutine = null;
        }

        if (waitingPrompt)
        {
            ItchPromptUI.Instance.Show();
        }
        else
        {
            ItchPromptUI.Instance.Hide();
        }
    }

    private void ZeroHorizontal()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }
}
