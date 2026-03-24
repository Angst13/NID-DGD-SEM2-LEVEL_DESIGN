using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 3;
    public float invincibleTime = 1.5f;

    private int hitCount = 0;
    private Vector2 startPosition;
    private bool isInvincible = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeHit();
        }
    }

    void TakeHit()
    {
        hitCount++;
        Debug.Log("Hit! " + hitCount + " / " + maxHits);

        if (hitCount >= maxHits)
        {
            GameOver();
        }
        else
        {
            transform.position = startPosition;
            StartCoroutine(InvincibleCooldown());
        }
    }

    System.Collections.IEnumerator InvincibleCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}