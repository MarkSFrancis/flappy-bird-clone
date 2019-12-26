using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Sounds
{
    private static Dictionary<Scene, GameObject> _soundObjects;

    public static void Play(Sound sound, float volume = 1f)
    {
        if (_soundObjects is null)
        {
            _soundObjects = new GameObject("Sound", typeof(AudioSource));
            var activeScene = SceneManager.GetActiveScene();
        }

        var audioSource = _soundObjects.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetClip(sound), volume);
    }

    private static AudioClip GetClip(Sound sound)
    {
        var clip = GameAssets.Instance.GameSounds.FirstOrDefault(gs => gs.Sound == sound)?.Clip;

        if (clip is null)
        {
            Debug.LogWarning($"Sound not found for {sound}");
        }

        return clip;
    }

    public static void SetupSounds(this Button_UI button)
    {
        button.MouseOverOnceFunc += () => Play(Sound.ButtonOver, 0.3f);
        button.ClickFunc += () => Play(Sound.ButtonClick, 0.3f);
    }
}

public enum Sound
{
    BirdJump,
    ButtonClick,
    ButtonOver,
    Lose,
    Score
}
