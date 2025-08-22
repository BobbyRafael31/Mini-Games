using System.Collections;
using UnityEngine;
using TMPro;

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard,
    Insane
}
public class GameManager : MonoBehaviour
{
    public Ball ball;
    public GameObject startPanel;
    public AIEnemy enemy;
    private AIDifficulty selectedDifficulty;

    [Header("Game Values")]
    public int leftScore, rightScore, scoreToWin = 11;
    public float serveDelay = 5;

    public TextMeshProUGUI leftText, rightText, winnerText;

    Coroutine serveRoutine;

    void Start()
    {
        //UpdateUI();
        //serveRoutine = StartCoroutine(Serve(0));

        Time.timeScale = 0f;
        startPanel.SetActive(true);
    }

    public void SetDifficulty(int difficultyIndex)
    {
        selectedDifficulty = (AIDifficulty)difficultyIndex;

        enemy.SetDifficulty(selectedDifficulty);

        startPanel.SetActive(false);
        Time.timeScale = 1f;

        StartCoroutine(Serve(0));
    }

    public void LeftScores()
    {
        leftScore++;
        UpdateUI();

        if (!CheckWin())
        {
            if (serveRoutine != null) StopCoroutine(serveRoutine);
            serveRoutine = StartCoroutine(Serve(1));
        }


    }

    public void RightScores()
    {
        rightScore++;
        UpdateUI();

        if (!CheckWin())
        {
            if (serveRoutine != null) StopCoroutine(serveRoutine);
            serveRoutine = StartCoroutine(Serve(-1));
        }

    }

    void UpdateUI()
    {
        if (leftText) leftText.text = leftScore.ToString();
        if (rightText) rightText.text = rightScore.ToString();
    }

    bool CheckWin()
    {
        if (leftScore >= scoreToWin || rightScore >= scoreToWin)
        {
            if (winnerText)
                winnerText.text = (leftScore > rightScore) ? "Left Wins!" : "Right Wins!";
            winnerText.gameObject.SetActive(true);

            Time.timeScale = 0f; // Stop the game
            return true;
        }
        return false;
    }

    IEnumerator Serve(int direction)
    {
        ball.ResetAndDisableBall();

        float timer = serveDelay;
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        ball.EnableColliderBall();
        ball.LaunchBall(direction);

        serveRoutine = null;

    }

}
