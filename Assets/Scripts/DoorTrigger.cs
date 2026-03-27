using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string nextSceneName = "Scene2";
    [Range(0.1f, 1.0f)]
    public float centerThreshold = 0.3f; // How close to the center the player must be

    private BoxCollider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Check if the door is currently Unlocked (isTrigger will be true)
            if (doorCollider != null && doorCollider.isTrigger)
            {
                // 2. Check if the player is standing in the center of the door
                float playerX = other.transform.position.x;
                float doorX = transform.position.x;
                
                // Calculate distance between player and door center
                if (Mathf.Abs(playerX - doorX) < centerThreshold)
                {
                    Debug.Log("Player in center and door unlocked! Loading next scene...");
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    }
}