using UnityEngine;

public class Level1DiaryController : MonoBehaviour
{
    public static Level1DiaryController Instance { get; private set; }

    private LevelController levelController;
    private PlayerMovement player;

    private bool shopReached;
    private bool diaryPurchased;
    private bool safeZoneReached;
    private bool diaryWritten;
    private bool diaryVisiblePrev;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (levelController == null)
        {
            levelController = FindObjectOfType<LevelController>();
        }

        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        bool inSafeZone = player != null && player.IsInSafeZone;
        bool diaryVisible = DiaryUI.Instance != null && DiaryUI.Instance.IsVisible;

        if (!safeZoneReached && diaryPurchased && inSafeZone)
        {
            safeZoneReached = true;
            levelController?.CompleteObligation(2);
        }

        if (!diaryWritten && diaryPurchased && inSafeZone && diaryVisible && !diaryVisiblePrev)
        {
            diaryWritten = true;
            levelController?.CompleteObligation(3);
        }

        diaryVisiblePrev = diaryVisible;
    }

    public void SetLevelController(LevelController controller)
    {
        levelController = controller;
    }

    public void MarkShopReached()
    {
        if (shopReached)
        {
            return;
        }

        shopReached = true;
        levelController?.CompleteObligation(0);
    }

    public void MarkDiaryPurchased()
    {
        if (diaryPurchased)
        {
            return;
        }

        diaryPurchased = true;
        levelController?.CompleteObligation(1);
        if (!shopReached)
        {
            MarkShopReached();
        }
    }
}
