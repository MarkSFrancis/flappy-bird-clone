using System;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] Frames;
    public int FramesPerSecond = 1;
    public bool Loop;
    public bool Active = true;
    private float _timeToNextFrame = 0;
    private int _currentFrameIndex = -1;

    private void Start()
    {
        if (FramesPerSecond < 0)
        {
            throw new ArgumentException($"{nameof(FramesPerSecond)} cannot be less than 0", nameof(FramesPerSecond));
        }

        if (Frames.Length > 0)
        {
            NextFrame();
        }
    }

    private void Update()
    {
        if (FramesPerSecond == 0 || Frames.Length == 0 || !Active)
        {
            // Animation is disabled
            return;
        }

        _timeToNextFrame -= Time.deltaTime;

        if (_timeToNextFrame < 0)
        {
            NextFrame();
        }
    }

    private void NextFrame()
    {
        if (_currentFrameIndex == Frames.Length)
        {
            if (!Loop)
            {
                return;
            }

            _currentFrameIndex = 0;
        }

        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();

        sprite.sprite = Frames[_currentFrameIndex++];
        _timeToNextFrame += GetTimePerFrame();
    }

    private float GetTimePerFrame()
    {
        return 1f / FramesPerSecond;
    }
}
