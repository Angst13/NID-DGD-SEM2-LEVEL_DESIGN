using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}