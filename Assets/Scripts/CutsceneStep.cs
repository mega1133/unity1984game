using System;
using UnityEngine;

[Serializable]
public class CutsceneStep
{
    public StepType stepType = StepType.LockPlayer;
    public bool lockPlayer = true;
    public float moveDistance = 0f;
    public float moveSpeed = 2f;
    public float waitSeconds = 1f;
    public DialogueLine[] dialogueLines;
    public bool obligationsVisible = true;

    public enum StepType
    {
        LockPlayer,
        MovePlayer,
        Wait,
        Dialogue,
        ShowObligations,
        End
    }
}
