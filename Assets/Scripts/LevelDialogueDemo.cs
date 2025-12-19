using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDialogueDemo : MonoBehaviour
{
    [SerializeField] private Vector2 npcPosition = new Vector2(6f, 1f);
    [SerializeField] private Color npcColor = new Color(0.5f, 0.5f, 1f, 1f);

    private void Start()
    {
        SpawnNpcWithDialogue();
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

        GameObject demo = new GameObject("LevelDialogueDemo");
        demo.AddComponent<LevelDialogueDemo>();
    }

    private void SpawnNpcWithDialogue()
    {
        GameObject npc = new GameObject("NPC_DialogueDemo");
        npc.transform.position = npcPosition;

        PlaceholderSprite sprite = npc.AddComponent<PlaceholderSprite>();
        sprite.SetColor(npcColor);

        BoxCollider2D collider = npc.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        DialogueInteractable interactable = npc.AddComponent<DialogueInteractable>();
        interactable.SetLines(BuildLines(npc.transform));
    }

    private List<DialogueLine> BuildLines(Transform npcTransform)
    {
        List<DialogueLine> list = new List<DialogueLine>();
        Transform player = FindPlayer();

        list.Add(new DialogueLine(npcTransform, "MOVE ALONG."));
        list.Add(new DialogueLine(player, "..."));
        list.Add(new DialogueLine(npcTransform, "NO UNEXPECTED MOVEMENTS."));
        list.Add(new DialogueLine(player, "UNDERSTOOD."));

        return list;
    }

    private Transform FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }
}
