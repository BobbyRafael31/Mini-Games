using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Smash, Freeze, Reduce }

    public PowerUpType type;
    [HideInInspector] public PowerUpSpawner spawner;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Ball ball = col.GetComponent<Ball>();
        if (ball != null)
        {
            if (ball.lastHitPlayerId != -1)
            {
                PlayerPowerUpManager mgr = GameManager.Instance.GetPlayerManager(ball.lastHitPlayerId);
                mgr.AddPowerUp(type);
            }

            spawner.NotifyTaken(this);
            Destroy(gameObject);
        }
    }
}
