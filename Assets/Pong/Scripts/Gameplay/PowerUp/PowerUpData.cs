using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public Sprite icon;

    public PowerUpType type;

    public enum PowerUpType { Smash, Freeze, Reduce }

    public IPowerUpEffect CreateEffect()
    {
        switch (type)
        {
            case PowerUpType.Smash: return new SmashPowerUp();
            case PowerUpType.Freeze: return new FreezePowerUp();
            case PowerUpType.Reduce: return new ReducePowerUp();
            default: return null;
        }
    }
}
