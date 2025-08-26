using UnityEngine;

public static class BounceUtility
{
    private const float MIN_X_DIR = 0.3f;

    public static Vector2 CalculateBounceDirection(Collision2D collision,
        float maxBounceAngle, Transform ballTransform)
    {
        float playerY = collision.collider.transform.position.y;
        float hitPointY = ballTransform.position.y;
        float normalizedOffset = (hitPointY - playerY) / (collision.collider.bounds.size.y / 2f);
        float angleRad = normalizedOffset * maxBounceAngle * Mathf.Deg2Rad;

        float xDir = Mathf.Sign(collision.GetContact(0).normal.x) * -1f;

        Vector2 direction = new Vector2(Mathf.Cos(angleRad) * xDir,
            Mathf.Sin(angleRad)).normalized;

        if (Mathf.Abs(direction.x) < MIN_X_DIR)
        {
            direction.x = MIN_X_DIR * xDir;
        }

        float playerX = collision.collider.transform.position.x;
        float ballX = ballTransform.position.x;

        if (ballX > playerX && direction.x < 0)
            direction.x = Mathf.Abs(direction.x);
        else if (ballX < playerX && direction.x > 0)
            direction.x = -Mathf.Abs(direction.x);

        return direction.normalized;
    }

    public static float GetNormalizedOffset(Collision2D collision, Transform ballTransform)
    {
        float playerY = collision.collider.transform.position.y;
        float hitPointY = ballTransform.position.y;
        return (hitPointY - playerY) / (collision.collider.bounds.size.y / 2f);
    }
}
