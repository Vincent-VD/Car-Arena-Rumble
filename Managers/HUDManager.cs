using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _HUD = null;

	[SerializeField]
	private PlayerHUD[] _playerHUDs = null;

	private static HUDManager _instance = null;

	public static HUDManager Instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		//Creates HUDs for available players, assigns correct IDs
		List<PlayerConfig> ids = PlayerManager.Instance.PlayerConfigs;
		for (int i = 0; i < _playerHUDs.Length; i++)
		{
			if (i >= ids.Count)
			{
				Destroy(_playerHUDs[i].gameObject);
				continue;
			}
			_playerHUDs[i].SetPlayerSprite(ids[i]._playerSprite);
			_playerHUDs[i].SetPlayerId(i);
		}
		Debug.Log("player IDs assigned");
	}

	public void HandleDamageIncrease(int playerId, float normalizedAmount)
	{
		_playerHUDs[playerId].SetImpactMultiplier(normalizedAmount);
	}

	public void HandleKillsIncrease(int playerId, int normalizedAmount)
	{
		_playerHUDs[playerId].setKillIncrease(normalizedAmount);
	}

	public void ChangeWinningPlayer()
	{
		PlayersScoreManager.playerScore[] scores = PlayersScoreManager.Instance.PlayerScores;
		for (int iter = 0; iter < _playerHUDs.Length; ++iter)
		{
			if (_playerHUDs[iter] != null)
			{
				_playerHUDs[iter].SetWinning(scores[iter].hasMostKills);

			}
		}
	}

}
