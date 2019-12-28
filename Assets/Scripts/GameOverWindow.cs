using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    public Text ScoreText { get; private set; }
    public Text HighScoreText { get; private set; }

    private Button_UI _retryBtn;
    private Button_UI _mainMenuBtn;

    private void Awake()
    {
        ScoreText = transform.Find("scoreText").GetComponent<Text>();
        HighScoreText = transform.Find("highScoreText").GetComponent<Text>();
        _retryBtn = transform.Find("retryBtn").GetComponent<Button_UI>();
        _mainMenuBtn = transform.Find("mainMenuBtn").GetComponent<Button_UI>();

        _retryBtn.ClickFunc += Retry_Clicked;
        _retryBtn.SetupSounds();
        _mainMenuBtn.ClickFunc += MainMenu_Clicked;
        _mainMenuBtn.SetupSounds();
    }

    private void Start()
    {
        Bird.Instance.OnDie += Bird_OnDie;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Retry_Clicked();
        }
    }

    private void Bird_OnDie(object sender, System.EventArgs e)
    {
        ScoreText.text = Level.Instance.TotalPipesPassed.ToString();

        if (Score.HighScore < Level.Instance.TotalPipesPassed)
        {
            HighScoreText.text = "NEW HIGHSCORE!";
        }
        else
        {
            HighScoreText.text = "HIGHSCORE: " + Score.HighScore;
        }

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Retry_Clicked()
    {
        SceneLoader.RequestScene(Scene.Game);
    }

    private void MainMenu_Clicked()
    {
        SceneLoader.RequestScene(Scene.MainMenu);
    }
}
