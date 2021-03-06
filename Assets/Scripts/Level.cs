﻿using CodeMonkey;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float _cameraOrthoSizeY = 50f;
    private const float _pipeWidth = 7.8f;
    private const float _pipeHeadHeight = 3.75f;
    private const float _worldSpeed = 30f;
    private const float _cameraOrthoSizeX = 100f;
    private const float _cloudWidth = 60f;
    private const float _playerPosition = 0f;
    private const float _groundWidth = 100.5f;
    private const float _cloudY = 30f;

    public float PipeSpawnDelay { get; private set; }
    public float CloudSpawnDelay { get; private set; }
    public List<PipePair> Pipes { get; private set; }
    public List<Transform> Grounds { get; private set; }
    public List<Transform> Clouds { get; private set; }
    public float TimeToNextPipeSpawn { get; private set; }
    public float TimeToNextCloudSpawn { get; set; }
    public float PipeGapSize { get; set; }
    public int TotalPipesPassed { get; private set; }
    public static Level Instance { get; private set; }
    public LevelState State { get; set; }

    private void Awake()
    {
        Instance = this;
        Pipes = new List<PipePair>();
        Grounds = new List<Transform>();
        Clouds = new List<Transform>();
        PipeSpawnDelay = 1.2f;
        CloudSpawnDelay = 7f;
        PipeGapSize = 50f;
        State = LevelState.WaitingForStart;
    }

    private void Start()
    {
        Bird.Instance.OnDie += Bird_OnDie;
        Bird.Instance.OnStart += Bird_OnStart;

        Score.Setup();
        SpawnInitialGround();
        SpawnInitialClouds();
    }

    private void Bird_OnStart(object sender, EventArgs e)
    {
        State = LevelState.Playing;
    }

    private void Bird_OnDie(object sender, EventArgs e)
    {
        State = LevelState.GameOver;

        FunctionTimer.Create(() =>
        {
            State = LevelState.GameOver;
        }, 1f);
    }

    private void Update()
    {
        if (State != LevelState.Playing)
        {
            return;
        }

        MovePipes();
        HandlePipeSpawning();
        HandleGround();
        HandleClouds();
    }

    private Transform GetNextCloud()
    {
        int randomCloudId = UnityEngine.Random.Range(0, 3);

        switch(randomCloudId)
        {
            case 0:
                return GameAssets.Instance.pfCloud1;
            case 1:
                return GameAssets.Instance.pfCloud2;
            case 2:
                return GameAssets.Instance.pfCloud3;
            default:
                throw new ArgumentOutOfRangeException(nameof(randomCloudId), $"Cannot render cloud with ID {randomCloudId}");
        }
    }

    private void SpawnInitialClouds()
    {
        Transform cloud;

        cloud = Instantiate(GetNextCloud(), new Vector3(0, _cloudY, 0), Quaternion.identity);
        Clouds.Add(cloud);
    }

    private void SpawnInitialGround()
    {
        Transform ground;
        const float groundY = -49f;

        ground = Instantiate(GameAssets.Instance.pfGround, new Vector3(-_groundWidth, groundY, 0), Quaternion.identity);
        Grounds.Add(ground);

        ground = Instantiate(GameAssets.Instance.pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        Grounds.Add(ground);

        ground = Instantiate(GameAssets.Instance.pfGround, new Vector3(_groundWidth, groundY, 0), Quaternion.identity);
        Grounds.Add(ground);
    }

    private void HandleClouds()
    {
        TimeToNextCloudSpawn -= Time.deltaTime;

        while (TimeToNextCloudSpawn < 0)
        {
            TimeToNextCloudSpawn += CloudSpawnDelay;

            var cloud = Instantiate(GetNextCloud(), new Vector3(_cameraOrthoSizeX + _cloudWidth, _cloudY, 0), Quaternion.identity);
            Clouds.Add(cloud);
        }

        for(int i = 0; i < Clouds.Count; i++)
        {
            var cloud = Clouds[i];

            cloud.position += new Vector3(-1, 0) * _worldSpeed * Time.deltaTime * .5f;

            if (cloud.position.x < -(_cameraOrthoSizeX + _cloudWidth))
            {
                Destroy(cloud.gameObject);
                Clouds.RemoveAt(i--);
            }
        }
    }

    private void HandleGround()
    {
        var halfGroundWidth = _groundWidth / 2;

        foreach (var ground in Grounds)
        {
            ground.position += new Vector3(-1, 0) * _worldSpeed * Time.deltaTime;

            if (ground.position.x < -_cameraOrthoSizeX - halfGroundWidth)
            {
                var rightMostXPosition = Grounds.Max(g => g.position.x);

                ground.position = new Vector3(rightMostXPosition + _groundWidth, ground.position.y, ground.position.z);
            }
        }
    }

    private void HandlePipeSpawning()
    {
        TimeToNextPipeSpawn -= Time.deltaTime;

        while (TimeToNextPipeSpawn < 0)
        {
            TimeToNextPipeSpawn += PipeSpawnDelay;

            var heightEdgeLimit = 10f;
            var minHeight = PipeGapSize * .5f + heightEdgeLimit;
            var maxHeight = _cameraOrthoSizeY * 2f - minHeight;

            var height = UnityEngine.Random.Range(minHeight, maxHeight);
            var newPipePair = PipePair.Create(height, PipeGapSize, _cameraOrthoSizeX);
            Pipes.Add(newPipePair);

            PipeGapSize = Math.Max(PipeGapSize - 5f, 27f);
        }
    }

    private void MovePipes()
    {
        for (var i = 0; i < Pipes.Count; i++)
        {
            PipePair pipe = Pipes[i];
            var wasAheadOfPlayer = pipe.IsAheadOfPlayer;

            pipe.ShiftLeft(_worldSpeed * Time.deltaTime);

            if (wasAheadOfPlayer && !pipe.IsAheadOfPlayer)
            {
                TotalPipesPassed++;
                Sounds.Play(Sound.Score);
            }

            if (pipe.IsOffscreen)
            {
                pipe.Destroy();
                Pipes.RemoveAt(i--);
            }
        }
    }

    public class PipePair
    {
        public Pipe Top { get; set; }

        public Pipe Bottom { get; set; }

        public bool IsOffscreen => Top.Head.position.x < -_cameraOrthoSizeX;

        public bool IsAheadOfPlayer => Top.Head.position.x > _playerPosition;

        public PipePair(Pipe top, Pipe bottom)
        {
            Top = top;
            Bottom = bottom;
        }

        public void ShiftLeft(float amount)
        {
            Top.ShiftLeft(amount);
            Bottom.ShiftLeft(amount);
        }

        public void Destroy()
        {
            Top.Destroy();
            Bottom.Destroy();
        }

        public static PipePair Create(float gapY, float gapSize, float xPosition)
        {
            var topPipeHeight = (_cameraOrthoSizeY * 2) - gapY - (gapSize / 2);
            var bottomPipeHeight = gapY - (gapSize / 2);

            var top = Pipe.Create(topPipeHeight, xPosition, false);
            var bottom = Pipe.Create(bottomPipeHeight, xPosition, true);

            return new PipePair(top, bottom);
        }
    }

    public class Pipe
    {
        public Transform Head { get; set; }

        public Transform Body { get; set; }

        public Pipe(Transform head, Transform body)
        {
            Head = head;
            Body = body;
        }

        public void ShiftLeft(float amount)
        {
            Head.position += new Vector3(-1, 0) * amount;
            Body.position += new Vector3(-1, 0) * amount;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(Head.gameObject);
            UnityEngine.Object.Destroy(Body.gameObject);
        }

        public static Pipe Create(float height, float xPosition, bool openAtTop)
        {
            Transform body = Instantiate(GameAssets.Instance.pfPipeBody);

            var pipeBodyYPosition = _cameraOrthoSizeY;
            if (openAtTop)
            {
                pipeBodyYPosition = -pipeBodyYPosition;
                body.localScale = new Vector3(1, -1, 1);
            }
            body.position = new Vector2(xPosition, pipeBodyYPosition);
            SpriteRenderer bodyRenderer = body.GetComponent<SpriteRenderer>();
            bodyRenderer.size = new Vector2(_pipeWidth, height);
            BoxCollider2D bodyCollider = body.GetComponent<BoxCollider2D>();
            bodyCollider.size = new Vector2(_pipeWidth, height);
            bodyCollider.offset = new Vector2(0, -height / 2);

            Transform head = Instantiate(GameAssets.Instance.pfPipeHead);

            var pipeHeadYPosition = _cameraOrthoSizeY - height + _pipeHeadHeight / 2;
            if (openAtTop)
            {
                pipeHeadYPosition = -pipeHeadYPosition;
            }

            head.position = new Vector2(xPosition, pipeHeadYPosition);

            return new Pipe(head, body);
        }
    }
}
