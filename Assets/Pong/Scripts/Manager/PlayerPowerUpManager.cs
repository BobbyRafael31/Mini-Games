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
        switch (type)
        {
            case PowerUp.PowerUpType.Smash:
                ball.SetSmashMode();
                break;
            case PowerUp.PowerUpType.Freeze:
                opponent.GetComponent<PlayerMovement>().Freeze(1f);
                break;
            case PowerUp.PowerUpType.Reduce:
                opponent.GetComponent<PlayerMovement>().Reduce(2f);
                break;
        }

        powerUpSlots[0].sprite = defaultSprite;

        for (int i = 1; i < powerUpSlots.Length; i++)
        {
            powerUpSlots[i - 1].sprite = powerUpSlots[i].sprite;
        }
    }

}
