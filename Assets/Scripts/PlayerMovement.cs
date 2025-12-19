using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundedCheckExtraDistance = 0.05f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool controlEnabled = true;
    private bool diaryOpen;
    private bool previousControlBeforeDiary;
    private int safeZoneCount;
    private bool jumpDeniedLogged;
    private PlayerSuspicionController suspicion;
    private PlayerScratch scratch;
    private Collider2D col;
    private ContactFilter2D groundContactFilter;
    private readonly Collider2D[] contactResults = new Collider2D[4];
    private float lastJumpPressedTime;

    public bool IsControlEnabled => controlEnabled;
    public bool IsInSafeZone => safeZoneCount > 0;
    public float VelocityY => rb != null ? rb.velocity.y : 0f;
    public float LastJumpPressedTime => lastJumpPressedTime;
    public bool DebugIsGrounded => IsGrounded();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        suspicion = GetComponent<PlayerSuspicionController>();
        if (suspicion == null)
        {
            suspicion = gameObject.AddComponent<PlayerSuspicionController>();
        }

        scratch = GetComponent<PlayerScratch>();
        if (scratch == null)
        {
            scratch = gameObject.AddComponent<PlayerScratch>();
        }

        groundContactFilter.useLayerMask = true;
        groundContactFilter.layerMask = groundLayer;
        groundContactFilter.useTriggers = false;
    }

    private void Update()
    {
        if (diaryOpen)
        {
            HandleDiaryCloseInput();
            return;
        }

        HandleDiaryOpenInput();

        if (!controlEnabled)
        {
            return;
        }

        bool grounded = IsGrounded();
        if (grounded)
        {
            jumpDeniedLogged = false;
        }

        HandleMovement();
        HandleJump(grounded);
    }

    private void HandleMovement()
    {
        bool leftPressed = InputHelper.IsLeftHeld();
        bool rightPressed = InputHelper.IsRightHeld();

        int direction = (rightPressed ? 1 : 0) - (leftPressed ? 1 : 0);
        direction = Mathf.Clamp(direction, -1, 1);

        Vector2 velocity = rb.velocity;
        velocity.x = direction * moveSpeed;
        rb.velocity = velocity;
    }

    private void HandleJump(bool grounded)
    {
        bool jumpPressed = InputHelper.IsJumpPressedDown();
        if (!jumpPressed)
        {
            return;
        }

        lastJumpPressedTime = Time.time;

        if (grounded)
        {
            Vector2 velocity = rb.velocity;
            velocity.y = jumpForce;
            rb.velocity = velocity;
        }
        else if (!jumpDeniedLogged)
        {
            Debug.Log("Jump pressed while not grounded - check ground setup");
            jumpDeniedLogged = true;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return CheckContacts();
        }

        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null)
        {
            return true;
        }

        if (CheckContacts())
        {
            return true;
        }

        return CheckBoxCast();
    }

    private bool CheckContacts()
    {
        if (rb == null)
        {
            return false;
        }

        int count = rb.OverlapCollider(groundContactFilter, contactResults);
        for (int i = 0; i < count; i++)
        {
            if (contactResults[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckBoxCast()
    {
        if (col == null)
        {
            return false;
        }

        Bounds bounds = col.bounds;
        Vector2 size = new Vector2(bounds.size.x * 0.95f, groundedCheckExtraDistance * 2f);
        float distance = groundedCheckExtraDistance;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, size, 0f, Vector2.down, distance, groundLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void SetControlEnabled(bool isEnabled)
    {
        controlEnabled = isEnabled;

        if (!controlEnabled)
        {
            if (rb != null)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }

    public void ResetMotion()
    {
        if (rb == null)
        {
            return;
        }

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void ClearTemporaryState()
    {
        safeZoneCount = 0;
        diaryOpen = false;
        DiaryUI.Instance.Hide();
        if (suspicion != null)
        {
            suspicion.ResetSuspicionState();
        }

        if (scratch != null)
        {
            scratch.ResetScratchState();
        }
    }

    public void EnterSafeZone()
    {
        safeZoneCount++;
    }

    public void ExitSafeZone()
    {
        safeZoneCount = Mathf.Max(0, safeZoneCount - 1);
    }

    private void HandleDiaryOpenInput()
    {
        if (!InputHelper.IsDiaryPressedDown())
        {
            return;
        }

        if (!controlEnabled)
        {
            return;
        }

        if ((DialogueRunner.Instance != null && DialogueRunner.Instance.IsPlaying) ||
            (CutsceneRunner.Instance != null && CutsceneRunner.Instance.IsRunning))
        {
            return;
        }

        if (!IsInSafeZone)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Fail("THOUGHTCRIME");
            }
            return;
        }

        previousControlBeforeDiary = controlEnabled;
        diaryOpen = true;
        SetControlEnabled(false);
        DiaryUI.Instance.Show();
    }

    private void HandleDiaryCloseInput()
    {
        if (!diaryOpen)
        {
            return;
        }

        bool closePressed = InputHelper.IsConfirmPressedDown();
        if (!closePressed)
        {
            return;
        }

        DiaryUI.Instance.Hide();
        diaryOpen = false;
        SetControlEnabled(previousControlBeforeDiary);
    }
}
