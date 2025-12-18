using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerSuspicionController : MonoBehaviour
{
    private readonly List<SuspicionZone> activeZones = new List<SuspicionZone>();

    private PlayerMovement movement;
    private float fillAmount;
    private bool failTriggered;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        SuspicionZone zone = GetActiveZone();
        if (zone == null)
        {
            ResetSuspicionState();
            return;
        }

        bool paused = ShouldPause();
        if (!paused)
        {
            float seconds = Mathf.Max(0.01f, zone.SecondsToMax);
            fillAmount += Time.deltaTime / seconds;
            fillAmount = Mathf.Clamp01(fillAmount);

            if (fillAmount >= 1f && !failTriggered)
            {
                failTriggered = true;
                TriggerFail(zone.FailReason);
            }
        }

        SuspicionUI.Instance.Show(fillAmount);
    }

    public void EnterZone(SuspicionZone zone)
    {
        if (zone == null)
        {
            return;
        }

        if (!activeZones.Contains(zone))
        {
            activeZones.Add(zone);
        }

        if (activeZones.Count == 1)
        {
            fillAmount = 0f;
            failTriggered = false;
        }
    }

    public void ExitZone(SuspicionZone zone)
    {
        if (zone == null)
        {
            return;
        }

        activeZones.Remove(zone);

        if (activeZones.Count == 0)
        {
            ResetSuspicionState();
        }
    }

    public void ResetSuspicionState()
    {
        fillAmount = 0f;
        failTriggered = false;
        activeZones.Clear();
        SuspicionUI.Instance.Hide();
    }

    private SuspicionZone GetActiveZone()
    {
        SuspicionZone best = null;
        float bestTime = float.MaxValue;

        for (int i = 0; i < activeZones.Count; i++)
        {
            SuspicionZone candidate = activeZones[i];
            if (candidate == null)
            {
                continue;
            }

            float time = Mathf.Max(0.01f, candidate.SecondsToMax);
            if (time < bestTime)
            {
                bestTime = time;
                best = candidate;
            }
        }

        return best;
    }

    private bool ShouldPause()
    {
        if (movement != null && !movement.IsControlEnabled)
        {
            return true;
        }

        if (DiaryUI.Instance != null && DiaryUI.Instance.IsVisible)
        {
            return true;
        }

        if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsPlaying)
        {
            return true;
        }

        if (CutsceneRunner.Instance != null && CutsceneRunner.Instance.IsRunning)
        {
            return true;
        }

        return false;
    }

    private void TriggerFail(string reason)
    {
        SuspicionUI.Instance.Hide();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Fail(string.IsNullOrEmpty(reason) ? "LOITERING" : reason);
        }
    }
}
