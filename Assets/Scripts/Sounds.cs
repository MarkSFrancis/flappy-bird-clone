using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityScene = UnityEngine.SceneManagement.Scene;

public static class Sounds
{
    private static readonly Dictionary<UnityScene, GameObject> _soundObjects = new Dictionary<UnityScene, GameObject>();

    public static void Play(Sound sound, float volume = 1f)
    {
        var audioSource = GetAudioSource();

        audioSource.PlayOneShot(GetClip(sound), volume);
    }

    private static AudioSource GetAudioSource()
    {
        var activeScene = SceneManager.GetActiveScene();

        if (!_soundObjects.TryGetValue(activeScene, out var soundObject))
        {
            soundObject = new GameObject("Sound", typeof(AudioSource));

            _soundObjects.Add(activeScene, soundObject);
        }

        return soundObject.GetComponent<AudioSource>();
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
