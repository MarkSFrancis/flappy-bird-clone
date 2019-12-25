using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    public Text ScoreText { get; set; }

    private void Awake()
    {
        ScoreText = transform.Find("scoreText").GetComponent<Text>();
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc += RetryClicked;
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

    private void RetryClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}
