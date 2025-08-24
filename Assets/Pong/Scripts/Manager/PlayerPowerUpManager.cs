using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerPowerUpManager : MonoBehaviour
{
    public int playerId; // left player = 0, right player = 1
    public Image[] powerUpSlots; // 3 for each player
    public Sprite smashSprite, freezeSprite, reduceSprite;

    private Queue<PowerUp.PowerUpType> queue = new Queue<PowerUp.PowerUpType>();

    private Sprite defaultSprite;
    void Start()
    {
        if (powerUpSlots.Length > 0)
            defaultSprite = powerUpSlots[0].sprite;
    }

    public void AddPowerUp(PowerUp.PowerUpType type)
    {
        if (queue.Count >= powerUpSlots.Length) return;

        queue.Enqueue(type);

        int index = queue.Count - 1;
        switch (type)
        {
            case PowerUp.PowerUpType.Smash: powerUpSlots[index].sprite = smashSprite; break;
            case PowerUp.PowerUpType.Freeze: powerUpSlots[index].sprite = freezeSprite; break;
            case PowerUp.PowerUpType.Reduce: powerUpSlots[index].sprite = reduceSprite; break;
        }
    }

    public void UseNextPowerUp(Ball ball, GameObject opponent)
    {
        if (queue.Count == 0) return;

        PowerUp.PowerUpType type = queue.Dequeue();

        string opponentName = opponent.name;
        string source = playerId == 0 ? "Left Paddle" : "Right Paddle";

        switch (type)
        {
            case PowerUp.PowerUpType.Smash:
                ball.QueueSmash();
                Debug.Log($"{source} activated SMASH — ball speed boosted.");
                break;

            case PowerUp.PowerUpType.Freeze:
                var playerFreeze = opponent.GetComponent<PlayerMovement>();
                if (playerFreeze != null)
                {
                    playerFreeze.Freeze(1f);
                    Debug.Log($"{source} applied FREEZE to {opponentName}.");
                }
                else
                {
                    var aiFreeze = opponent.GetComponent<AIEnemy>();
                    if (aiFreeze != null)
                    {
                        aiFreeze.Freeze(1f);
                        Debug.Log($"{source} applied FREEZE to AI ({opponentName}).");
                    }
                }
                break;

            case PowerUp.PowerUpType.Reduce:
                var playerReduce = opponent.GetComponent<PlayerMovement>();
                if (playerReduce != null)
                {
                    playerReduce.Reduce(2f);
                    Debug.Log($"{source} applied REDUCE to {opponentName}.");
                }
                else
                {
                    var aiReduce = opponent.GetComponent<AIEnemy>();
                    if (aiReduce != null)
                    {
                        aiReduce.Reduce(2f);
                        Debug.Log($"{source} applied REDUCE to AI ({opponentName}).");
                    }
                }
                break;
        }

        powerUpSlots[0].sprite = defaultSprite;

        for (int i = 1; i < powerUpSlots.Length; i++)
        {
            powerUpSlots[i - 1].sprite = powerUpSlots[i].sprite;
        }
    }

}
