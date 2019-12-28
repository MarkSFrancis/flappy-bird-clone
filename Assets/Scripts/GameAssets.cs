using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public Transform pfPipeHead;
    public Transform pfPipeBody;
    public Transform pfGround;
    public Transform pfCloud1;
    public Transform pfCloud2;
    public Transform pfCloud3;

    public GameSound[] GameSounds;

    private void Awake()
    {
        Instance = this;
    }
}

[Serializable]
public class GameSound
{
    public AudioClip Clip;
    public Sound Sound;
}
