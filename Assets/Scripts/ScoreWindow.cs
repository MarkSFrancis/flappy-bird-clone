using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    public Text ScoreText { get; private set; }
    public Text HighScoreText { get; private set; }

    private void Awake()
    {
        ScoreText = transform.Find("scoreText").GetComponent<Text>();
        HighScoreText = transform.Find("highScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        HighScoreText.text = "HIGHSCORE: " + SavedData.HighScore;
        gameObject.SetActive(false);

        Bird.Instance.OnStart += Bird_OnStart;
    }

    private void Bird_OnStart(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        ScoreText.text = Level.Instance.TotalPipesPassed.ToString();
    }
}
