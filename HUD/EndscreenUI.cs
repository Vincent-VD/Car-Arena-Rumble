using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndscreenUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _player1KillScore = null;

    [SerializeField]
    private TextMeshProUGUI _player2KillScore = null;

    [SerializeField]
    private TextMeshProUGUI _player3KillScore = null;

    [SerializeField]
    private TextMeshProUGUI _player4KillScore = null;



    [SerializeField]
    private TextMeshProUGUI _player1RammingScore = null;

    [SerializeField]
    private TextMeshProUGUI _player2RammingScore = null;

    [SerializeField]
    private TextMeshProUGUI _player3RammingScore = null;

    [SerializeField]
    private TextMeshProUGUI _player4RammingScore = null;


    [SerializeField]
    private TextMeshProUGUI _player1YeetSpeed = null;

    [SerializeField]
    private TextMeshProUGUI _player2YeetSpeed = null;

    [SerializeField]
    private TextMeshProUGUI _player3YeetSpeed = null;

    [SerializeField]
    private TextMeshProUGUI _player4YeetSpeed = null;

    private PlayersScoreManager _playersScoreManager = null;
    // Start is called before the first frame update
    void Start()
    {
        _playersScoreManager = PlayersScoreManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateKillScore();
        UpdateRammingScore();
        UpdateYeetSpeed();
    }

   private void UpdateRammingScore()
    {
        if (_player1RammingScore && _playersScoreManager.PlayerScores.Length > 0)
        {
            _player1RammingScore.text = _playersScoreManager.PlayerScores[0].rammingScore.ToString();
        }
        if (_player2RammingScore && _playersScoreManager.PlayerScores.Length > 1)
        {
            _player2RammingScore.text = _playersScoreManager.PlayerScores[1].rammingScore.ToString();
        }
        if (_player3RammingScore && _playersScoreManager.PlayerScores.Length > 2)
        {
            _player3RammingScore.text = _playersScoreManager.PlayerScores[2].rammingScore.ToString();
        }
        if (_player4RammingScore && _playersScoreManager.PlayerScores.Length > 3)
        {
            _player4RammingScore.text = _playersScoreManager.PlayerScores[3].rammingScore.ToString();
        }
    }

    private void UpdateKillScore()
    {
        if (_player1KillScore && _playersScoreManager.PlayerScores.Length > 0)
        {
            _player1KillScore.text = _playersScoreManager.PlayerScores[0].killsAmount.ToString();
        }
        if (_player2KillScore && _playersScoreManager.PlayerScores.Length > 1)
        {
            _player2KillScore.text = _playersScoreManager.PlayerScores[1].killsAmount.ToString();
        }
        if (_player3KillScore && _playersScoreManager.PlayerScores.Length > 2)
        {
            _player3KillScore.text = _playersScoreManager.PlayerScores[2].killsAmount.ToString();
        }
        if (_player4KillScore && _playersScoreManager.PlayerScores.Length > 3)
        {
            _player4KillScore.text = _playersScoreManager.PlayerScores[3].killsAmount.ToString();
        }
    }
   private void UpdateYeetSpeed()
   {
       if (_player1YeetSpeed && _playersScoreManager.PlayerScores.Length > 0)
       {
           _player1YeetSpeed.text = _playersScoreManager.PlayerScores[0].velocity.ToString();
       }
       if (_player2YeetSpeed && _playersScoreManager.PlayerScores.Length > 1)
       {
           _player2YeetSpeed.text = _playersScoreManager.PlayerScores[1].velocity.ToString();
       }
       if (_player3YeetSpeed && _playersScoreManager.PlayerScores.Length > 2)
       {
           _player3YeetSpeed.text = _playersScoreManager.PlayerScores[2].velocity.ToString();
       }
       if (_player4YeetSpeed && _playersScoreManager.PlayerScores.Length > 3)
       {
           _player4YeetSpeed.text = _playersScoreManager.PlayerScores[3].velocity.ToString();
       }
    }
}
