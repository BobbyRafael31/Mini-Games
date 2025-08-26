using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Assigned Power-Up Data")]
    public PowerUpData data;

    [HideInInspector] public PowerUpSpawner spawner;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Ball ball = col.GetComponent<Ball>();
        if (ball != null && ball.lastHitPlayerId != -1)
        {
            var mgr = GameManager.Instance.GetPlayerManager(ball.lastHitPlayerId);
            IPowerUpEffect effect = data.CreateEffect();
            mgr.AddPowerUp(effect, data.icon);
        }

        spawner.NotifyTaken(this);
        Destroy(gameObject);
    }
}
