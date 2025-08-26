using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerPowerUpManager : MonoBehaviour
{
    [Header("Player Info")]
    public int playerId;

    [Header("PowerUp Sprites Slots")]
    public Image[] powerUpSlots;

    [Header("Default Sprites")]
    public Sprite defaultSprite;

    private Queue<IPowerUpEffect> powerUpQueue = new Queue<IPowerUpEffect>();
    private Queue<Sprite> iconQueue = new Queue<Sprite>();

    void Start()
    {
        if (powerUpSlots.Length > 0 && defaultSprite == null)
            defaultSprite = powerUpSlots[0].sprite;
    }

    public void AddPowerUp(IPowerUpEffect effect, Sprite icon)
    {
        if (powerUpQueue.Count >= powerUpSlots.Length) return;

        powerUpQueue.Enqueue(effect);
        iconQueue.Enqueue(icon);

        int index = powerUpQueue.Count - 1;
        powerUpSlots[index].sprite = icon;
    }

    public void UseNextPowerUp(Ball ball, GameObject opponent)
    {
        if (powerUpQueue.Count == 0 || iconQueue.Count == 0) return;

        IPowerUpEffect effect = powerUpQueue.Dequeue();
        Sprite usedIcon = iconQueue.Dequeue();

        effect.Apply(ball, opponent);

        for (int i = 1; i < powerUpSlots.Length; i++)
        {
            powerUpSlots[i - 1].sprite = powerUpSlots[i].sprite;
        }

        powerUpSlots[powerUpSlots.Length - 1].sprite = defaultSprite;
    }
}
