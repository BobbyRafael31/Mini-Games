using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform ball;
    public float reactionTime = 0.1f;
    public float errorMargin = 0.2f;

    private float timer;
    private float targetY;
    public float velocityY;

    private AIDifficulty currentDifficulty;

    void Update()
    {
        if (ball == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = reactionTime;

            if (currentDifficulty == AIDifficulty.Insane)
            {
                float predicted = PredictBallYPosition();
                targetY = Mathf.Lerp(targetY, predicted, 0.5f);

            }
            else
            {
                targetY = ball.position.y + Random.Range(-errorMargin, errorMargin);
            }
        }

        Vector3 newPos = transform.position;
        float distance = Mathf.Abs(transform.position.y - targetY);
        if (distance > 0.02f)
        {
            newPos.y = Mathf.SmoothDamp(newPos.y, targetY, ref velocityY, 0.05f, moveSpeed);
        }
        else
        {
            velocityY = 0f;
        }

        newPos.y = ClampPositionY(newPos.y);
        transform.position = newPos;
    }

    private float ClampPositionY(float y)
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float paddleHalf = transform.localScale.y / 2f;

        return Mathf.Clamp(y, -halfHeight + paddleHalf, halfHeight - paddleHalf);
    }

    public void SetDifficulty(AIDifficulty difficulty)
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                moveSpeed = 6f;
                reactionTime = 0.2f;
                errorMargin = 1f;
                break;

            case AIDifficulty.Medium:
                moveSpeed = 9f;
                reactionTime = 0.1f;
                errorMargin = 0.5f;
                break;

            case AIDifficulty.Hard:
                moveSpeed = 12f;
                reactionTime = 0.05f;
                errorMargin = 0.1f;
                break;
            case AIDifficulty.Insane:
                moveSpeed = 15f;
                reactionTime = 0.01f;
                errorMargin = 0f;
                break;
        }

        currentDifficulty = difficulty;
    }

    private float PredictBallYPosition()
    {
        if (ball == null) return transform.position.y;

        Vector2 simulatedPos = ball.position;
        Vector2 simulatedVel = ball.GetComponent<Rigidbody2D>().linearVelocity;

        float halfHeight = Camera.main.orthographicSize;

        while ((simulatedVel.x > 0 && simulatedPos.x < transform.position.x) ||
               (simulatedVel.x < 0 && simulatedPos.x > transform.position.x))
        {
            simulatedPos += simulatedVel * Time.fixedDeltaTime;

            if (simulatedPos.y > halfHeight || simulatedPos.y < -halfHeight)
            {
                simulatedVel.y = -simulatedVel.y;
                simulatedPos.y = Mathf.Clamp(simulatedPos.y, -halfHeight, halfHeight);
            }
        }

        return simulatedPos.y;
    }
}
