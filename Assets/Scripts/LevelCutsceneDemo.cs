using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCutsceneDemo : MonoBehaviour
{
    [SerializeField] private Vector2 corridorOrigin = new Vector2(14f, -2f);
    [SerializeField] private float corridorLength = 6f;
    [SerializeField] private Color npcColor = new Color(0.8f, 0.4f, 0.4f, 1f);

    private Transform npc;
    private Transform player;

    private void Start()
    {
        player = FindPlayer();
        BuildCorridor();
        SpawnNpc();
        BuildCutsceneObjects();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Level0_Test")
        {
            return;
        }

        GameObject obj = new GameObject("LevelCutsceneDemo");
        obj.AddComponent<LevelCutsceneDemo>();
    }

    private void BuildCorridor()
    {
        GameObject ground = new GameObject("CutsceneCorridor_Ground");
        ground.transform.position = corridorOrigin;
        ground.transform.localScale = new Vector3(corridorLength, 1f, 1f);

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
        {
            ground.layer = groundLayer;
        }

        PlaceholderSprite sprite = ground.AddComponent<PlaceholderSprite>();
        sprite.SetColor(new Color(0.25f, 0.25f, 0.25f, 1f));

        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(corridorLength, 1f);
        collider.offset = new Vector2(0f, 0f);
    }

    private void SpawnNpc()
    {
        GameObject npcObj = new GameObject("CutsceneDemo_NPC");
        npcObj.transform.position = corridorOrigin + new Vector2(corridorLength * 0.5f, 1f);

        PlaceholderSprite sprite = npcObj.AddComponent<PlaceholderSprite>();
        sprite.SetColor(npcColor);

        BoxCollider2D collider = npcObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        npc = npcObj.transform;
    }

    private void BuildCutsceneObjects()
    {
        GameObject triggerObj = new GameObject("CutsceneDemo_Trigger");
        triggerObj.transform.position = corridorOrigin + new Vector2(1.5f, 0f);

        BoxCollider2D collider = triggerObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.5f, 2f);

        CutsceneSequence sequence = triggerObj.AddComponent<CutsceneSequence>();
        sequence.ConfigureSteps(BuildSteps());

        CutsceneTrigger trigger = triggerObj.AddComponent<CutsceneTrigger>();
        trigger.SetSequence(sequence);
    }

    private List<CutsceneStep> BuildSteps()
    {
        List<CutsceneStep> steps = new List<CutsceneStep>();

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.LockPlayer,
            lockPlayer = true
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.ShowObligations,
            obligationsVisible = false
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.MovePlayer,
            moveDistance = 1.5f,
            moveSpeed = 2.5f
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.Dialogue,
            dialogueLines = BuildLines()
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.Wait,
            waitSeconds = 0.5f
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.ShowObligations,
            obligationsVisible = true
        });

        steps.Add(new CutsceneStep
        {
            stepType = CutsceneStep.StepType.End
        });

        return steps;
    }

    private DialogueLine[] BuildLines()
    {
        if (player == null)
        {
            player = FindPlayer();
        }

        List<DialogueLine> lines = new List<DialogueLine>
        {
            new DialogueLine(npc, "KEEP MOVING."),
            new DialogueLine(player != null ? player : npc, "..."),
            new DialogueLine(npc, "NO LOITERING IN THIS CORRIDOR."),
            new DialogueLine(player != null ? player : npc, "UNDERSTOOD.")
        };

        return lines.ToArray();
    }

    private Transform FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }
}
