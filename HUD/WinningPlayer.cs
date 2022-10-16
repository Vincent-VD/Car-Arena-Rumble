using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinningPlayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _winningPlayer = null;

    [SerializeField]
    private string _extraText = " has won!";

    private const string _player = "Player ";

    private bool _hasShowedPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (_winningPlayer && PlayersScoreManager.Instance.EndAchieved && !_hasShowedPlayer)
        {
            _hasShowedPlayer = true;
            _winningPlayer.text = _player + (PlayersScoreManager.Instance.GetHighestKillScore().playerIdx + 1).ToString() + _extraText;
        }
    }
}
