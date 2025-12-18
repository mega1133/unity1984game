using System.Collections.Generic;
using UnityEngine;

public class CutsceneSequence : MonoBehaviour
{
    [SerializeField] private List<CutsceneStep> steps = new List<CutsceneStep>();

    public IReadOnlyList<CutsceneStep> Steps => steps;

    public void ConfigureSteps(List<CutsceneStep> newSteps)
    {
        steps = newSteps ?? new List<CutsceneStep>();
    }
}
