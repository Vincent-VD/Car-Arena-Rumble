using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
	[SerializeField]
	private String _levelToLoad = null;

	public void Reset()
	{
		EndScreenPlayerLineup[] ls = FindObjectsOfType<EndScreenPlayerLineup>();
		foreach (var lineupMenu in ls)
		{
			lineupMenu.ClearControlBindings();
		}

		Destroy(PlayersScoreManager.Instance.gameObject);
		Destroy(PlayerManager.Instance.gameObject);
		Destroy(SoundManager.Instance.gameObject);

		foreach (var setupMenu in FindObjectsOfType<SpawnPlayerSetupMenu>())
		{
			Destroy(setupMenu.gameObject);
		}

		SceneManager.LoadScene(_levelToLoad);
	}

}
