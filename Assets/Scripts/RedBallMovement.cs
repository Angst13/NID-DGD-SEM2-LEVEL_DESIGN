using UnityEngine;

public class RedBallMovement : MonoBehaviour
{
    public float speed = 3f;
    public float moveDistance = 3f;
    public bool goLeftFirst = false;
    public float rotationSpeed = 200f; // degrees per second, tune to taste

    private float startX;
    private float fixedY;
    private float targetX;
    private bool goingToSide = true;

    void Start()
    {
        startX = transform.position.x;
        fixedY = transform.position.y;

        if (goLeftFirst)
            targetX = startX - moveDistance;
        else
            targetX = startX + moveDistance;
    }

    void Update()
    {
        Vector2 previousPos = transform.position;

        // Move towards target
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetX, fixedY),
            speed * Time.deltaTime
        );

        // Rotate based on which direction the ball is actually moving
        float moveDir = ((Vector2)transform.position - previousPos).x;

        if (Mathf.Abs(moveDir) > 0.0001f)
        {
            float rotationDirection = moveDir > 0 ? -1f : 1f; // right = clockwise, left = counter-clockwise
            transform.Rotate(0f, 0f, rotationDirection * rotationSpeed * Time.deltaTime);
        }

        // When reached target
        if (Vector2.Distance(transform.position, new Vector2(targetX, fixedY)) < 0.01f)
        {
            if (goingToSide)
            {
                targetX = startX;
                goingToSide = false;
            }
            else
            {
                if (goLeftFirst)
                    targetX = startX - moveDistance;
                else
                    targetX = startX + moveDistance;
                goingToSide = true;
            }
        }
    }
}