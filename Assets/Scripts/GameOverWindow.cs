using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    public Text ScoreText { get; set; }

    private Button_UI _retryBtn;
    private Button_UI _mainMenuBtn;

    private void Awake()
    {
        ScoreText = transform.Find("scoreText").GetComponent<Text>();
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

    private void Bird_OnDie(object sender, System.EventArgs e)
    {
        ScoreText.text = Level.Instance.TotalPipesPassed.ToString();
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
