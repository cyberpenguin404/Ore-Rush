using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private float _screenShakeDurationWall = 0.2f;
    [SerializeField]
    private float _screenShakeMagnitudeWall = 1;
    [SerializeField]
    private float _screenShakeDurationDeathWall = 0.2f;
    [SerializeField]
    private float _screenShakeMagnitudeDeathWall = 1;
    [SerializeField]
    private int _countdownTime = 5;
    public SpawnManager SpawnManager;
    public GridGenerate GridGenerate;

    public int Stage;

    [SerializeField]
    private ScreenShake _screenShake;

    public bool StartGameManually;

    [Header("UI References")]

    public TextMeshProUGUI ScoreTextPlayer1;
    public TextMeshProUGUI ScoreTextPlayer2;
    public Slider PickaxeCooldownSlider1;
    public Slider PickaxeCooldownSlider2;
    public Image DynamiteIcon1;
    public Image DynamiteIcon2;
    public GameObject RJoystickIcon1;
    public GameObject RJoystickIcon2;
    public Slider DynamiteCooldownSlider1;
    public Slider DynamiteCooldownSlider2;
    public GameObject startScreen;
    public Vector3 SpawnPointPlayer1;
    public Vector3 SpawnPointPlayer2;

    public GameObject player1Connected;
    public GameObject player2Connected;

    public Animator StartScreenAnimator;

    public RectTransform RedBar;
    public RectTransform BlueBar;
    public RectTransform ClashSymbol; 
    
    [SerializeField]
    private float _scoreBarLerpSpeed = 3f;

    private float _targetScoreRatioPlayer1 = 0.5f;
    private float _targetScoreRatioPlayer2 = 0.5f;

    private float _currentScoreRatioPlayer1 = 0.5f;
    private float _currentScoreRatioPlayer2 = 0.5f;

    [SerializeField]
    private TextMeshProUGUI _countdownText;
    [SerializeField]
    private TextMeshProUGUI _startCountdownText;
    [SerializeField]
    private GameObject _winnerScreen;
    [SerializeField]
    private TextMeshProUGUI _winnerText;
    [SerializeField]
    private GameObject _fallingWallPrefab;
    [SerializeField]
    private GameObject _deathFallingWallPrefab;


    [Header("Multiplayer")]

    public List<PlayerHandler> Players = new List<PlayerHandler>();
    public int _playerCount = 0;
    public Dictionary<int, int> PlayerScores = new Dictionary<int, int>();


    [Header("Game Settings")]
    [SerializeField]
    private float GameTime;
    [field: SerializeField] public int Width { get; private set; } = 25;
    [field: SerializeField] public int Height { get; private set; } = 25;
    private double _remainingTime;
    public bool MainGameRunning = false; 
    public bool _hasStartedGame = false;

    void Start()
    {
        _remainingTime = GameTime;
        _countdownText.text = ((int)(_remainingTime / 60)) + ":" + (int)(_remainingTime % 60);
    }
    void Update()
    {
        if (StartGameManually)
        {
            StartGame();
            _countdownTime = 0;
        }
        if (_playerCount == 2)
        {
            StartGame();
        }
        else
        {
            HandleStartScreen();
        }
        if (MainGameRunning)
        HandleGame();

    }
    public void ConnectPlayer(PlayerHandler player)
    {
        _playerCount++;
        Players.Add(player);
        if (_playerCount == 1)
        {
            player1Connected.SetActive(true);
        }
        if (_playerCount == 2)
        {
            player2Connected.SetActive(true);
        }
    }

    private void HandleStartScreen()
    {
    }

    public void ChangeScore(int amount, int playerIndex)
    {
        if (!PlayerScores.ContainsKey(playerIndex))
        {
            PlayerScores[playerIndex] = 0;
        }

        PlayerScores[playerIndex] = amount;
        Debug.Log($"Player {playerIndex} score changed by {amount}. New score: {PlayerScores[playerIndex]}");

        UpdateTargetBarRatios();
    }

    private void UpdateTargetBarRatios()
    {
        _targetScoreRatioPlayer1 = GetPlayerWinRatio(1);
        _targetScoreRatioPlayer2 = GetPlayerWinRatio(2);
    }

    private void UpdateScoresBar()
    {
        _currentScoreRatioPlayer1 = Mathf.Lerp(_currentScoreRatioPlayer1, _targetScoreRatioPlayer1, Time.deltaTime * _scoreBarLerpSpeed);
        _currentScoreRatioPlayer2 = Mathf.Lerp(_currentScoreRatioPlayer2, _targetScoreRatioPlayer2, Time.deltaTime * _scoreBarLerpSpeed);

        RedBar.anchorMax = new Vector2(_currentScoreRatioPlayer1, RedBar.anchorMax.y);
        BlueBar.anchorMax = new Vector2(_currentScoreRatioPlayer2, BlueBar.anchorMax.y);
    }

    public float GetPlayerWinRatio(int playerIndex)
    {
        int totalScore = PlayerScores.Values.Sum();
        if (totalScore == 0) return 0.5f; // Neutral bar
        float playerScorePercentage = (float)PlayerScores[playerIndex] / totalScore;

        playerScorePercentage = MathF.Min(0.9f, playerScorePercentage);
        playerScorePercentage = MathF.Max(0.1f, playerScorePercentage);


        return playerScorePercentage;
    }
    private void HandleGame()
    {
        UpdateScoresBar();
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;

            _countdownText.text = ((int)(_remainingTime / 60)) + ":" + (int)(_remainingTime % 60);
        }
        else if (MainGameRunning && _remainingTime <= 0)
        {
            EndGame();
        }
    }

    public void DropWall(Vector3 position)
    {
        Instantiate(_fallingWallPrefab, position, Quaternion.identity);
        _screenShake.Shake(_screenShakeDurationWall, _screenShakeMagnitudeWall);
    }
    public void DropDeathWall(Vector3 position)
    {
        Instantiate(_deathFallingWallPrefab, position, Quaternion.identity);
        _screenShake.Shake(_screenShakeDurationDeathWall, _screenShakeMagnitudeDeathWall);
    }

    private void StartGame()
    {
        if (!_hasStartedGame)
        {
            Debug.Log("Starting game");
            _hasStartedGame = true;
            StartCoroutine(StartCountdownRoutine());
        }
    }

    private IEnumerator StartCountdownRoutine()
    {
        StartScreenAnimator.SetTrigger("StartGame");
        _startCountdownText.gameObject.SetActive(true);

        while (_countdownTime > 1)
        {
            _startCountdownText.text = (_countdownTime-1).ToString();
            yield return new WaitForSeconds(1f);
            _countdownTime--;
        }

        _startCountdownText.text = "Collect!";
        yield return new WaitForSeconds(1f);
        MainGameRunning = true;

        _startCountdownText.gameObject.SetActive(false);
    }
    public void RestartGame()
    {
        _hasStartedGame = false;
        SceneManager.LoadScene(0);
    }

    private void EndGame()
    {
        _winnerScreen.SetActive(true);

        int highestScore = Players.Max(p => p.Score);

        List<PlayerHandler> topPlayers = Players.Where(p => p.Score == highestScore).ToList();

        foreach (PlayerHandler player in Players)
        {
            Debug.Log($"{player.PlayerName}: {player.Score}");
        }

        if (topPlayers.Count > 1)
        {
            string names = string.Join(", ", topPlayers.Select(p => p.PlayerName));
            _winnerText.text = $"Draw between: {names}!";
        }
        else
        {
            PlayerHandler winningPlayer = topPlayers[0];
            _winnerText.text = $"{winningPlayer.PlayerName} has won!";
        }

        _playerCount = 0;
        MainGameRunning = false;
        _hasStartedGame = false;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
