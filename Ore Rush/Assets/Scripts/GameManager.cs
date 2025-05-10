using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]

    public TextMeshProUGUI ScoreTextPlayer1;
    public TextMeshProUGUI ScoreTextPlayer2;
    public TextMeshProUGUI PickaxeCooldownText1;
    public TextMeshProUGUI PickaxeCooldownText2;
    public TextMeshProUGUI DynamiteCooldownText1;
    public TextMeshProUGUI DynamiteCooldownText2;
    public TextMeshProUGUI playerCountStartScreen;
    public GameObject startScreen;
    [SerializeField]
    private TextMeshProUGUI CountdownText;
    [SerializeField]
    private TextMeshProUGUI WinnerText;


    [Header("Multiplayer")]

    private List<PlayerHandler> Players = new List<PlayerHandler>();
    public int _playerCount = 0;
    


    [Header("Game Settings")]
    [SerializeField]
    private float GameTime;
    private double _remainingTime;
    private bool _gameRunning = true;

    void Start()
    {
        _remainingTime = GameTime;
        Time.timeScale = 0;
    }
    void Update()
    {
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

            CountdownText.text = ((int)(_remainingTime / 60)) + ":" + (int)(_remainingTime % 60);
        }
        else if (_gameRunning && _remainingTime <= 0)
        {
            EndGame();
        }
    }

    private void StartGame()
    {
        startScreen.SetActive(false);
        Time.timeScale = 1;
    }

    private void EndGame()
    {
        PlayerHandler winningplayer = Players.OrderByDescending(i => i.Score).FirstOrDefault();
        WinnerText.text = winningplayer.PlayerName + " has won!";
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
        DontDestroyOnLoad(gameObject);
    }
}
