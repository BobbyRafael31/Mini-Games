using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float ballSpeed = 8f;
    public float maxBounceAngle = 60f;
    public float maxSpeed = 14f;
    public float accelerationRate = 0.5f;

    private float currentSpeed;
    private Rigidbody2D _rb;
    private Collider2D _ballCollider;
    private SpriteRenderer _ballSprite;

    private const float MIN_X_DIR = 0.3f;

    public int lastHitPlayerId = -1; // -1 = none, 0 = left, 1 = right

    private bool smashMode = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ballSprite = GetComponent<SpriteRenderer>();
        _ballCollider = GetComponent<Collider2D>();

        _ballSprite.enabled = false;
        currentSpeed = ballSpeed;
    }

    void FixedUpdate()
    {
        if (currentSpeed < maxSpeed)
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelerationRate * Time.fixedDeltaTime);

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
            _rb.linearVelocity = _rb.linearVelocity.normalized * currentSpeed;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Contains("Left"))
            lastHitPlayerId = 0;
        else if (collision.collider.name.Contains("Right"))
            lastHitPlayerId = 1;

        if (collision.collider.CompareTag("Player"))
            HandlePlayerBounce(collision);
    }

    private void HandlePlayerBounce(Collision2D collision)
    {
        float playerY = collision.collider.transform.position.y;
        float hitPointY = transform.position.y;

        float normalizedOffset = (hitPointY - playerY) / (collision.collider.bounds.size.y / 2f);
        float angleRad = normalizedOffset * maxBounceAngle * Mathf.Deg2Rad;

        float xDir = Mathf.Sign(collision.GetContact(0).normal.x) * -1f;

        float speedBoost = 1f + Mathf.Abs(normalizedOffset) * 0.05f;

        Vector2 direction = new Vector2
            (Mathf.Cos(angleRad) * xDir, Mathf.Sin(angleRad)).normalized;

        if (Mathf.Abs(direction.x) < MIN_X_DIR)
        {
            direction.x = MIN_X_DIR * xDir;
            direction.Normalize();
        }

        float playerX = collision.collider.transform.position.x;

        if (transform.position.x > playerX && direction.x < 0)
        {
            direction.x = Mathf.Abs(direction.x);
        }
        else if (transform.position.x < playerX && direction.x > 0)
        {
            direction.x = -Mathf.Abs(direction.x);
        }
        direction.Normalize();

        if (smashMode)
        {
            speedBoost = 1.8f; // big smash boost
            smashMode = false; // reset after one use
        }

        currentSpeed = Mathf.Min(currentSpeed * speedBoost, maxSpeed);
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

    public void SetSmashMode()
    {
        smashMode = true;
    }
}
