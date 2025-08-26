using UnityEngine;

public class FreezePowerUp : IPowerUpEffect
{
    public void Apply(Ball ball, GameObject opponent)
    {
        var player = opponent.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.Freeze(1f);
            Debug.Log("FREEZE applied to player.");
            return;
        }

        var ai = opponent.GetComponent<AIEnemy>();
        if (ai != null)
        {
            ai.Freeze(1f);
            Debug.Log("FREEZE applied to AI.");
        }
    }
}

