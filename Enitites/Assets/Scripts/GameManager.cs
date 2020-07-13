using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singleton reference
    public static GameManager Instance;

    private Transform player;
    private EnemySpawner enemySpawner;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


        player = FindObjectOfType<PlayerManager>().transform;
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // static methods to send messages to Entities
    public static Vector3 GetPlayerPosition()
    {
        if (GameManager.Instance == null)
        {
            return Vector3.zero;
        }

        return (Instance.player != null) ? GameManager.Instance.player.position : Vector3.zero;
    }
}
