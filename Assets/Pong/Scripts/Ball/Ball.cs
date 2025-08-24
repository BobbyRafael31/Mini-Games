using UnityEngine;

public struct SmashState
{
    public bool IsQueued;
    public bool IsActive;
    public float SavedSpeed;
    public float TempMaxSpeed;
}

public class Ball : MonoBehaviour
{
    #region -Variables-

    [Header("Ball Settings")]
    public float ballSpeed = 8f;
    public float maxBounceAngle = 60f;
    public float maxSpeed = 14f;
    public float accelerationRate = 0.5f;
    private float currentSpeed;
    public int lastHitPlayerId = -1; // -1 = none, 0 = left, 1 = right
    private const float MIN_X_DIR = 0.3f; // Minimum horizontal direction component

    [Header("Components")]
    private Rigidbody2D _rb;
    private Collider2D _ballCollider;
    private SpriteRenderer _ballSprite;
    private SmashState smash;

    #endregion

    #region Unity Callbacks
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ballSprite = GetComponent<SpriteRenderer>();
        _ballCollider = GetComponent<Collider2D>();

        currentSpeed = ballSpeed;
        _ballSprite.enabled = false;
    }

    void FixedUpdate()
    {
        float effectiveMaxSpeed = smash.TempMaxSpeed > 0f ? smash.TempMaxSpeed : maxSpeed;

        if (currentSpeed < effectiveMaxSpeed)
            currentSpeed = Mathf.MoveTowards(currentSpeed,
                effectiveMaxSpeed, accelerationRate * Time.fixedDeltaTime);

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
            _rb.linearVelocity = _rb.linearVelocity.normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("Left"))
            lastHitPlayerId = 0;
        else if (collision.collider.name.Contains("Right"))
            lastHitPlayerId = 1;

        if (collision.collider.CompareTag("Player"))
            HandlePlayerBounce(collision);
    }

    #endregion

    #region -Ball Control-
    public void LaunchBall(int direction)
    {
        float maxRad = maxBounceAngle * Mathf.Deg2Rad * 0.333f;
        float ang = Random.Range(-maxRad, maxRad);

        float xSign = direction == 0
            ? (Random.value < 0.5f ? 1f : -1f)
            : Mathf.Sign(direction);

        Vector2 dir = new Vector2(Mathf.Cos(ang) * xSign, Mathf.Sin(ang));
        _rb.position = Vector2.zero;
        currentSpeed = ballSpeed;
        _rb.linearVelocity = dir * ballSpeed;
    }

    private (Vector2 direction, float normalizedOffset) CalculateBounceDirection(Collision2D collision)
    {
        float playerY = collision.collider.transform.position.y;
        float hitPointY = transform.position.y;
        float normalizedOffset = (hitPointY - playerY) / (collision.collider.bounds.size.y / 2f);
        float angleRad = normalizedOffset * maxBounceAngle * Mathf.Deg2Rad;

        float xDir = Mathf.Sign(collision.GetContact(0).normal.x) * -1f;
        Vector2 direction = new Vector2(
            Mathf.Cos(angleRad) * xDir, Mathf.Sin(angleRad)).normalized;

        if (Mathf.Abs(direction.x) < MIN_X_DIR)
        {
            direction.x = MIN_X_DIR * xDir;
            direction.Normalize();
        }

        float playerX = collision.collider.transform.position.x;

        if (transform.position.x > playerX && direction.x < 0)
            direction.x = Mathf.Abs(direction.x);
        else if (transform.position.x < playerX && direction.x > 0)
            direction.x = -Mathf.Abs(direction.x);

        return (direction.normalized, normalizedOffset);
    }


    private void HandlePlayerBounce(Collision2D collision)
    {
        var (direction, normalizedOffset) = CalculateBounceDirection(collision);

        if (lastHitPlayerId != -1)
        {
            GameObject opponent = GameManager.Instance.GetOpponentPaddle(lastHitPlayerId);
            GameManager.Instance.GetPlayerManager(lastHitPlayerId)
                .UseNextPowerUp(this, opponent);
        }

        ApplySmashIfNeeded();
        ApplySpeedBoost(normalizedOffset);

        _rb.linearVelocity = direction * currentSpeed;

        Vector2 paddleEdge = collision.collider.ClosestPoint(transform.position);
        transform.position = new Vector2
            (paddleEdge.x + Mathf.Sign(direction.x) * 0.15f, transform.position.y);
    }
    public void ResetAndDisableBall()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.position = Vector2.zero;
        _ballCollider.enabled = false;
        _ballSprite.enabled = false;
    }

    public void EnableColliderBall()
    {
        _ballCollider.enabled = true;
        _ballSprite.enabled = true;
    }

    #endregion

    #region -Smash and Speed Boost-
    private void ApplySmashIfNeeded()
    {
        if (smash.IsQueued && !smash.IsActive)
        {
            smash.TempMaxSpeed = 20f;
            currentSpeed = Mathf.Min(currentSpeed * 2.5f, smash.TempMaxSpeed);
            smash.IsActive = true;
            smash.IsQueued = false;
        }
        else if (smash.IsActive && lastHitPlayerId != -1)
        {
            currentSpeed = smash.SavedSpeed;
            smash.SavedSpeed = 0f;
            smash.IsActive = false;
            smash.TempMaxSpeed = 0f;
        }
    }
    private void ApplySpeedBoost(float normalizedOffset)
    {
        if (!smash.IsActive)
        {
            float speedBoost = 1f + Mathf.Abs(normalizedOffset) * 0.05f;
            currentSpeed = Mathf.Clamp(currentSpeed * speedBoost, ballSpeed, maxSpeed);
        }
    }

    public void QueueSmash()
    {
        if (!smash.IsQueued)
        {
            smash.SavedSpeed = currentSpeed;
            smash.IsQueued = true;
            smash.IsActive = false;
        }
    }
    #endregion
}
