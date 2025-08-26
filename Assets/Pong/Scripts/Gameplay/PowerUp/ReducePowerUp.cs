using UnityEngine;

public class ReducePowerUp : IPowerUpEffect
{
    public void Apply(Ball ball, GameObject opponent)
    {
        var player = opponent.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.Reduce(2f);
            Debug.Log("REDUCE applied to player.");
            return;
        }

        var ai = opponent.GetComponent<AIEnemy>();
        if (ai != null)
        {
            ai.Reduce(2f);
            Debug.Log("REDUCE applied to AI.");
        }
    }
}
