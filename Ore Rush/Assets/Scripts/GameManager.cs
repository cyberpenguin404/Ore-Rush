using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI CountdownText;
    [SerializeField]
    private TextMeshProUGUI WinnerText;

    [SerializeField]
    private List<PlayerHandler> Players = new List<PlayerHandler>();
    [SerializeField]
    private float GameTime;
    private double _remainingTime;
    private bool _gameRunning = true;
    void Start()
    {
        _remainingTime = GameTime;
    }
    void Update()
    {
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

    private void EndGame()
    {
        PlayerHandler winningplayer = Players.OrderByDescending(i => i.Score).FirstOrDefault();
        WinnerText.text = winningplayer.PlayerName + " has won!";
        Time.timeScale = 0;
        _gameRunning = false;
    }
}
