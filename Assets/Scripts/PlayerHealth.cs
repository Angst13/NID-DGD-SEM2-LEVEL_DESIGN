using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 3;
    public float invincibleTime = 1.5f;
    public float deathAnimationDuration = 1.5f;

    // Static variables persist across scene loads without singleton
    private static int hitCount = 0;
    private static bool isDead = false;
    private static int respawnScene = 0;
    private static Vector2 respawnPosition;

    private bool isInvincible = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string HURT_TRIGGER = "Hurt";
    private const string DEAD_TRIGGER = "Dead";

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find SpawnPoint in this scene
        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            respawnPosition = spawnPoint.transform.position;
        }
        else
        {
            respawnPosition = transform.position;
            Debug.LogWarning("No SpawnPoint found in scene: " + gameObject.scene.name);
        }

        respawnScene = SceneManager.GetActiveScene().buildIndex;

        // If returning after death, finish respawn
        if (isDead)
        {
            StartCoroutine(FinishRespawn());
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead || isInvincible) return;

        if (col.gameObject.CompareTag("Boss"))
            StartCoroutine(HandleDeath());
        else if (col.gameObject.CompareTag("Enemy"))
            TakeHit();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || isInvincible) return;

        if (other.CompareTag("Boss"))
            StartCoroutine(HandleDeath());
        else if (other.CompareTag("Enemy"))
            TakeHit();
    }

    void TakeHit()
    {
        hitCount++;
        Debug.Log("Hit! " + hitCount + " / " + maxHits);

        if (hitCount >= maxHits)
            StartCoroutine(HandleDeath());
        else
        {
            animator.SetTrigger(HURT_TRIGGER);
            StartCoroutine(InvincibleCooldown());
        }
    }

    IEnumerator HandleDeath()
    {
        isDead = true;
        isInvincible = true;

        // Save scene and spawn position before reloading
        respawnScene = SceneManager.GetActiveScene().buildIndex;

        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
            respawnPosition = spawnPoint.transform.position;
        else
            respawnPosition = transform.position;

        animator.SetTrigger(DEAD_TRIGGER);

        yield return new WaitForSeconds(deathAnimationDuration);

        SceneManager.LoadScene(respawnScene);
    }

    IEnumerator FinishRespawn()
    {
        yield return null;

        hitCount = 0;
        isDead = false;
        isInvincible = false;

        animator.ResetTrigger(HURT_TRIGGER);
        animator.ResetTrigger(DEAD_TRIGGER);
        animator.Rebind();
        animator.Update(0f);

        spriteRenderer.enabled = true;
        transform.position = respawnPosition;
    }

    IEnumerator InvincibleCooldown()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        spriteRenderer.enabled = true;

        isInvincible = false;
    }

    public void FullReset()
    {
        hitCount = 0;
        isDead = false;
        isInvincible = false;
        respawnScene = 0;
    }

    public int GetHitCount() => hitCount;
    public bool GetIsDead() => isDead;
}