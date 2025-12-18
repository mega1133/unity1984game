using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool controlEnabled = true;
    private bool diaryOpen;
    private bool previousControlBeforeDiary;
    private int safeZoneCount;
    private PlayerSuspicionController suspicion;
    private PlayerScratch scratch;

    public bool IsControlEnabled => controlEnabled;
    public bool IsInSafeZone => safeZoneCount > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        bool leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        int direction = (rightPressed ? 1 : 0) - (leftPressed ? 1 : 0);
        direction = Mathf.Clamp(direction, -1, 1);

        Vector2 velocity = rb.velocity;
        velocity.x = direction * moveSpeed;
        rb.velocity = velocity;
    }

    private void HandleJump()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (!jumpPressed)
        {
            return;
        }

        if (IsGrounded())
        {
            Vector2 velocity = rb.velocity;
            velocity.y = jumpForce;
            rb.velocity = velocity;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
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
        if (!Input.GetKeyDown(KeyCode.Q))
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

        bool closePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
        if (!closePressed)
        {
            return;
        }

        DiaryUI.Instance.Hide();
        diaryOpen = false;
        SetControlEnabled(previousControlBeforeDiary);
    }
}
