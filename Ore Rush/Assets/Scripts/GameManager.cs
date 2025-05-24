using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GemManager GemManager;
    public GridGenerate GridGenerate;

    public bool StartGameManually;

    [Header("UI References")]

    public TextMeshProUGUI ScoreTextPlayer1;
    public TextMeshProUGUI ScoreTextPlayer2;
    public TextMeshProUGUI PickaxeCooldownText1;
    public Slider PickaxeCooldownSlider1;
    public TextMeshProUGUI PickaxeCooldownText2;
    public Slider PickaxeCooldownSlider2;
    public TextMeshProUGUI DynamiteCooldownText1;
    public TextMeshProUGUI DynamiteCooldownText2;
    public TextMeshProUGUI playerCountStartScreen;
    public GameObject startScreen;
    [SerializeField]
    private TextMeshProUGUI _countdownText;
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
    


    [Header("Game Settings")]
    [SerializeField]
    private float GameTime;
    [field: SerializeField] public int Width { get; private set; } = 25;
    [field: SerializeField] public int Height { get; private set; } = 25;
    private double _remainingTime;
    private bool _gameRunning = true;

    void Start()
    {
        _remainingTime = GameTime;
        Time.timeScale = 0;
    }
    void Update()
    {
        if (StartGameManually)
        {
            StartGame();
        }
        if ( _playerCount == 2)
        {
            StartGame();
        }
        else
        {
            playerCountStartScreen.text = _playerCount.ToString() + "/2 players are ready";
        }
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;

            _countdownText.text = ((int)(_remainingTime / 60)) + ":" + (int)(_remainingTime % 60);
        }
        else if (_gameRunning && _remainingTime <= 0)
        {
            EndGame();
        }
    }
    public void DropWall(Vector3 position)
    {
        Instantiate(_fallingWallPrefab, position, Quaternion.identity);
    }
    public void DropDeathWall(Vector3 position)
    {
        Instantiate(_deathFallingWallPrefab, position, Quaternion.identity);
    }

    private void StartGame()
    {
        startScreen.SetActive(false);
        Time.timeScale = 1;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void EndGame()
    {
        _winnerScreen.SetActive(true);
        PlayerHandler winningplayer = Players.OrderByDescending(i => i.Score).FirstOrDefault();
        foreach (PlayerHandler player in Players)
        {
            Debug.Log($"{player.PlayerName}: {player.Score}");
        }
        _winnerText.text = winningplayer.PlayerName + " has won!";
        Time.timeScale = 0;
        _gameRunning = false;
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
