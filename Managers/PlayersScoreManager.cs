using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersScoreManager : MonoBehaviour
{
    private int _winningScore = 0;
    private bool _endAchieved = false;

    public int WinningScore
    {
        get
        {
            return _winningScore;
        }
        set
        {
            _winningScore = value;
        }
    }

    public bool EndAchieved
    {
        get
        {
            return _endAchieved;
        }
        set
        {
            _endAchieved = value;
        }
    }

    public struct playerScore
    {
        public int playerIdx;
        public int playerID;
        public int rammingScore;
        public int killsAmount;
        public float velocity;
        public bool hasMostKills;
    }

    private playerScore[] _playersScores = null;
    public playerScore[] PlayerScores
    {
        get
        {
            return _playersScores;
        }
    }

    #region SINGLETON
    private static PlayersScoreManager _instance;
    public static PlayersScoreManager Instance
    {
        get
        {
            if (_instance == null && !_applicationQuiting)
            {
                //find it in case it was placed in the scene
                _instance = FindObjectOfType<PlayersScoreManager>();
                if (_instance == null)
                {
                    //none was found in the scene, create a new instance
                    GameObject newObject = new GameObject("Singleton_PlayersScoreManager");
                    _instance = newObject.AddComponent<PlayersScoreManager>();
                }
            }

            return _instance;
        }
    }

    private static bool _applicationQuiting = false;
    public void OnApplicationQuit()
    {
        _applicationQuiting = true;
    }

    void Awake()
    {
        //we want this object to persist when a scene changes
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion


    //Initialize Players via getting all the players from an array
    public void InitializePlayerScores()
    {
        _playersScores = new playerScore[PlayerManager.Instance.Players.Count];
        for (int i = 0; i < PlayerManager.Instance.Players.Count; i++)
        {
            _playersScores[i].playerIdx = PlayerManager.Instance.Players[i].GetComponent<BasicCarCharachter>().PlayerID;
            _playersScores[i].playerID = PlayerManager.Instance.Players[i].GetInstanceID();
            _playersScores[i].rammingScore = 0;
            _playersScores[i].killsAmount = 0;
            _playersScores[i].velocity = 0f;
        }
    }

    //When rammed, it will get an ID through and look who it is on the array.
    // If found, it increases the score by one
    public void RammingScoreBonus(int ID)
    {
        for (int i = 0; i < _playersScores.Length; i++)
        {
            if (_playersScores[i].playerID == ID)
            {
                ++_playersScores[i].rammingScore;
                return;
            }
        }
    }

    public void IncreaseKillAmnount(int ID)
    {
        for (int i = 0; i < _playersScores.Length; i++)
        {
            if (_playersScores[i].playerID == ID)
            {
                //increase kills amount
                ++_playersScores[i].killsAmount;
                //updates hud
                HUDManager.Instance.HandleKillsIncrease(_playersScores[i].playerIdx, _playersScores[i].killsAmount);
                //Checks if this player has the highest score or not
                CheckHighestKillAmount();
                //updates the players status if they are winning aka highest kill amount
                PlayerManager.Instance.UpdateWinningStatus();
                HUDManager.Instance.ChangeWinningPlayer();
                //Checks if the car has enough kills to finish game.
                if (_playersScores[i].hasMostKills && _playersScores[i].killsAmount >= _winningScore && !HasKillScoresStalemate())
                {
                    _endAchieved = true;
                }
                return;
            }
        }
    }



    //Updates your velocity if the incomming velocity is the highest
    public void UpdateVelocity(int ID, float yeetSpeed)
    {
        for (int i = 0; i < _playersScores.Length; i++)
        {
            if (_playersScores[i].playerID == ID)
            {
                if (_playersScores[i].velocity < yeetSpeed)
                {
                    _playersScores[i].velocity = yeetSpeed;
                }
                return;
            }
        }
    }

    //Getting the highest score of the playerScore
    public playerScore GetHighestRammingScore()
    {
        //look if there is only 1 player in the arena if so it just returns that
        if (_playersScores.Length == 1)
        {
            return _playersScores[0];
        }

        int highestScoreidx = 0;
        for (int i = 1; i < _playersScores.Length; i++)
        {
            if (_playersScores[i - 1].rammingScore < _playersScores[i].rammingScore)
            {
                highestScoreidx = i;
            }
        }

        return _playersScores[highestScoreidx];
    }

    //Check if the 2 hihgest scores are not the same amount
    public bool HasRammingScoresStalemate()
    {
        playerScore tmp = GetHighestRammingScore();
        int highestScoreCounter = 0;
        foreach (var VARIABLE in _playersScores)
        {
            if (VARIABLE.rammingScore == tmp.rammingScore)
            {
                highestScoreCounter++;
            }
        }

        return highestScoreCounter > 1;
    }

    //get highest skillscore
    public playerScore GetHighestKillScore()
    {
        //look if there is only 1 player in the arena if so it just returns that
        if (_playersScores.Length == 1)
        {
            return _playersScores[0];
        }

        int highestScoreidx = 0;
        for (int i = 1; i < _playersScores.Length; i++)
        {
            if (_playersScores[i].hasMostKills)
            {
                return _playersScores[i];
            }
        }

        return _playersScores[0];
    }

    public bool HasKillScoresStalemate()
    {
        playerScore tmp = GetHighestKillScore();
        int highestScoreCounter = 0;
        foreach (var VARIABLE in _playersScores)
        {
            if (VARIABLE.killsAmount == tmp.killsAmount)
            {
                highestScoreCounter++;
            }
        }

        return highestScoreCounter > 1;
    }
    //Checks if you have the highest kill amount or not
    private void CheckHighestKillAmount()
    {
        int highestScore = 0;
        foreach (var VARIABLE in _playersScores)
        {
            if (highestScore <= VARIABLE.killsAmount)
            {
                //remembers the kills amount
                highestScore = VARIABLE.killsAmount;
            }
        }

        for (int i = 0; i < _playersScores.Length; i++)
        {
            //check if have the highest kill amount
            _playersScores[i].hasMostKills = (highestScore == _playersScores[i].killsAmount);
        }  
    }

    //Return the hasmost kills via the player idx
    public bool HasPlayerMostKills(int playerIdx)
    {
        foreach (var VARIABLE in _playersScores)
        {
            if (playerIdx == VARIABLE.playerIdx)
            {

                return VARIABLE.hasMostKills;
            }
        }
        //If it doesnt find anything, it will always be false
        return false;
    }


    //Getting the highest score of the playerScore
    public playerScore GetLowestRammingScore()
    {
        //look if there is only 1 player in the arena if so it just returns that
        if (_playersScores.Length == 1)
        {
            return _playersScores[0];
        }

        int lowesttScoreidx = 0;
        for (int i = 1; i < _playersScores.Length; ++i)
        {
            if (_playersScores[i - 1].rammingScore > _playersScores[i].rammingScore)
            {
                lowesttScoreidx = i;
            }
        }

        return _playersScores[lowesttScoreidx];
    }
}
