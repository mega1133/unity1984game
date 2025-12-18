using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Transform Speaker;
    [TextArea]
    public string Text;
    public float MinDisplayTime;

    public DialogueLine()
    {
    }

    public DialogueLine(Transform speaker, string text, float minDisplayTime = 0f)
    {
        Speaker = speaker;
        Text = text;
        MinDisplayTime = minDisplayTime;
    }
}
