using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 200f;
    public float leftLimit = -8f;
    public float rightLimit = 8f;

    private float fixedY;
    private float targetX;

    void Start()
    {
        // detach from realWorld so world switching never disables this object
        // but NO DontDestroyOnLoad — boss resets with each scene
        transform.SetParent(null);

        fixedY = transform.position.y;
        targetX = rightLimit;
    }

    void Update()
    {
        Vector2 previousPos = transform.position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetX, fixedY),
            speed * Time.deltaTime
        );

        float moveDir = ((Vector2)transform.position - previousPos).x;
        if (Mathf.Abs(moveDir) > 0.0001f)
        {
            float rotDir = moveDir > 0 ? -1f : 1f;
            transform.Rotate(0f, 0f, rotDir * rotationSpeed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, new Vector2(targetX, fixedY)) < 0.01f)
            targetX = targetX == rightLimit ? leftLimit : rightLimit;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
            FlipDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            FlipDirection();
    }

    private void FlipDirection()
    {
        targetX = targetX == rightLimit ? leftLimit : rightLimit;
        transform.position = new Vector3(
            targetX == rightLimit ? transform.position.x + 0.2f : transform.position.x - 0.2f,
            fixedY,
            transform.position.z
        );
    }
}