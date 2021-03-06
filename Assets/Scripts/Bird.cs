﻿using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float _jumpVelocity = 100f;

    private Rigidbody2D BirdRigidbody2D { get; set; }

    public event EventHandler OnDie;
    public event EventHandler OnStart;

    public static Bird Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        BirdRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Level.Instance.State == LevelState.Playing || Level.Instance.State == LevelState.WaitingForStart)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Jump();
            }

            transform.eulerAngles = new Vector3(0, 0, BirdRigidbody2D.velocity.y * .35f);
        }
    }

    private void Jump()
    {
        if (Level.Instance.State == LevelState.WaitingForStart)
        {
            BirdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            OnStart?.Invoke(this, EventArgs.Empty);
        }

        BirdRigidbody2D.velocity = Vector2.up * _jumpVelocity;
        Sounds.Play(Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        BirdRigidbody2D.bodyType = RigidbodyType2D.Static;
        Sounds.Play(Sound.Lose);
        OnDie?.Invoke(this, EventArgs.Empty);
    }
}
