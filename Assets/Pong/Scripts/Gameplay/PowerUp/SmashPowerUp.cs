using UnityEngine;

public class SmashPowerUp : IPowerUpEffect
{
    public void Apply(Ball ball, GameObject opponent)
    {
        ball.QueueSmash();
        Debug.Log("SMASH applied — ball will launch with boosted speed.");
    }
}
