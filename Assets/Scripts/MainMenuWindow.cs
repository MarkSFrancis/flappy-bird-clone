using CodeMonkey.Utils;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
    private Button_UI _playBtn;
    private Button_UI _quitBtn;

    private void Awake()
    {
        _playBtn = transform.Find("playBtn").GetComponent<Button_UI>();
        _quitBtn = transform.Find("quitBtn").GetComponent<Button_UI>();

        _playBtn.ClickFunc = Play_Clicked;
        _playBtn.SetupSounds();
        _quitBtn.ClickFunc = Quit_Clicked;
        _quitBtn.SetupSounds();
    }

    private void Play_Clicked()
    {
        SceneLoader.RequestScene(Scene.Game);
    }

    private void Quit_Clicked()
    {
        Application.Quit();
    }
}
