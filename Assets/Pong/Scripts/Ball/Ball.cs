using UnityEngine;

public struct SmashState
{
    public bool IsQueued { get; private set; }
    public bool IsActive { get; private set; }
    public float SavedSpeed { get; private set; }
    public float TempMaxSpeed { get; private set; }

    public void Queue(float currentSpeed)
    {
        if (!IsQueued)
        {
            SavedSpeed = currentSpeed;
            IsQueued = true;
            IsActive = false;
        }
    }

    public void Activate()
    {
        TempMaxSpeed = 20f;
        IsActive = true;
        IsQueued = false;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsQueued = false;
        TempMaxSpeed = 0f;
        SavedSpeed = 0f;
    }

    public void Reset()
    {
        IsQueued = false;
        IsActive = false;
        TempMaxSpeed = 0f;
        SavedSpeed = 0f;
    }
}

public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float ballSpeed = 8f;
    public float maxBounceAngle = 60f;
    public float maxSpeed = 14f;
    public float accelerationRate = 0.5f;
    private float currentSpeed;
    public int lastHitPlayerId = -1; // -1 = none, 0 = left, 1 = right

    [Header("Components")]
    private Rigidbody2D _rb;
    private Collider2D _ballCollider;
    private SpriteRenderer _ballSprite;
    private SmashState smash = new SmashState();

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

    private (Vector2 direction, float normalizedOffset) CalculateBounce(Collision2D collision)
    {
        float offset = BounceUtility.GetNormalizedOffset(collision, transform);

        Vector2 dir = BounceUtility.CalculateBounceDirection(collision, maxBounceAngle, transform);

        return (dir, offset);
    }
    private void HandlePlayerBounce(Collision2D collision)
    {
        var (direction, normalizedOffset) = CalculateBounce(collision);

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

    private void ApplySmashIfNeeded()
    {
        if (smash.IsQueued && !smash.IsActive)
        {
            smash.Activate();
            currentSpeed = Mathf.Min(currentSpeed * 2.5f, smash.TempMaxSpeed);
        }
        else if (smash.IsActive && lastHitPlayerId != -1)
        {
            currentSpeed = smash.SavedSpeed;
            smash.Deactivate();
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
        smash.Queue(currentSpeed);
    }
}
