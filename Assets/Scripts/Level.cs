using CodeMonkey;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private const float _cameraOrthoSizeY = 50f;
    private const float _pipeWidth = 7.8f;
    private const float _pipeHeadHeight = 3.75f;
    private const float _worldSpeed = 30f;
    private const float _cameraOrthoSizeX = 100f;
    private const float _playerPosition = 0f;

    public float PipeSpawnDelay { get; private set; }
    public List<PipePair> Pipes { get; private set; }
    public float TimeToNextPipeSpawn { get; private set; }
    public float PipeGapSize { get; set; }
    public int TotalPipesPassed { get; private set; }
    public static Level Instance { get; private set; }
    public LevelState State { get; set; }

    private void Awake()
    {
        Instance = this;
        Pipes = new List<PipePair>();
        PipeSpawnDelay = 1.2f;
        PipeGapSize = 50f;
        State = LevelState.WaitingForStart;
    }

    private void Start()
    {
        Bird.Instance.OnDie += Bird_OnDie;
        Bird.Instance.OnStart += Bird_OnStart;
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
    }

    private void HandlePipeSpawning()
    {
        TimeToNextPipeSpawn -= Time.deltaTime;

        while (TimeToNextPipeSpawn < 0)
        {
            TimeToNextPipeSpawn += PipeSpawnDelay;

            float heightEdgeLimit = 10f;
            float minHeight = PipeGapSize * .5f + heightEdgeLimit;
            float maxHeight = _cameraOrthoSizeY * 2f - minHeight;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            var newPipePair = PipePair.Create(height, PipeGapSize, _cameraOrthoSizeX);
            Pipes.Add(newPipePair);

            PipeGapSize = Math.Max(PipeGapSize - 5f, 27f);
        }
    }

    private void MovePipes()
    {
        for (var i = 0; i < Pipes.Count; i++)
        {
            var pipe = Pipes[i];
            bool wasAheadOfPlayer = pipe.IsAheadOfPlayer;

            pipe.ShiftLeft(_worldSpeed * Time.deltaTime);

            if (wasAheadOfPlayer && !pipe.IsAheadOfPlayer)
            {
                TotalPipesPassed++;
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
            var body = Instantiate(GameAssets.Instance.pfPipeBody);

            var pipeBodyYPosition = _cameraOrthoSizeY;
            if (openAtTop)
            {
                pipeBodyYPosition = -pipeBodyYPosition;
                body.localScale = new Vector3(1, -1, 1);
            }
            body.position = new Vector2(xPosition, pipeBodyYPosition);
            var bodyRenderer = body.GetComponent<SpriteRenderer>();
            bodyRenderer.size = new Vector2(_pipeWidth, height);
            var bodyCollider = body.GetComponent<BoxCollider2D>();
            bodyCollider.size = new Vector2(_pipeWidth, height);
            bodyCollider.offset = new Vector2(0, -height / 2);

            var head = Instantiate(GameAssets.Instance.pfPipeHead);

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
