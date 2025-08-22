using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public bool isLeftGoal = true;
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ball")) return;

        if (isLeftGoal) gameManager.RightScores();
        else gameManager.LeftScores();

    }
}
