using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    public Text ScoreText { get; private set; }

    private void Awake()
    {
        ScoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    private void Update()
    {
        ScoreText.text = Level.Instance.TotalPipesPassed.ToString();
    }
}
