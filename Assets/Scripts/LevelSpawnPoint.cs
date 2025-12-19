using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSpawnPoint : MonoBehaviour
{
    [SerializeField] private bool setOnStart = true;

    private void Start()
    {
        if (!setOnStart)
        {
            return;
        }

        TrySetCheckpoint();
    }

    public void TrySetCheckpoint()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        if (GameManager.Instance.HasCheckpointForScene(currentScene))
        {
            return;
        }

        GameManager.Instance.SetCheckpoint(transform.position);
    }
}
