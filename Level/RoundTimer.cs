using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _UITimer = null;

	[SerializeField]
	private float _maxRoundTime = 180.0f;
	private float _currentTime = 0;

	private bool _isActive = false;

	public bool IsActive
	{
		get => _isActive;
		set => _isActive = value;
	}
	private bool _isInOverTime = false;

	private void Start()
	{
		_currentTime = _maxRoundTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (!_isActive) return;
		
		if (_currentTime > 0)
		{
			_currentTime -= Time.deltaTime;
			DisplayTime(_currentTime);
			//added this return to improve performance
			//Because the if statement underneath here would never be true if it this statement is true
			return;

		}

		//When this goes under 0 it also checks each frame if it has still a scoreStaleMate
		//Which is 2+ highest score aere the same or not
		if (_currentTime <= 0 && !PlayersScoreManager.Instance.HasKillScoresStalemate())
		{
			//Debug.Log("Round End or Overtime check");
			PlayersScoreManager.Instance.EndAchieved = true;
			_isActive = false;
		}
	}

	private void DisplayTime(float timeToDisplay)
	{
		timeToDisplay += 1;

		float sec = Mathf.FloorToInt(timeToDisplay % 60);
		float min = Mathf.FloorToInt(timeToDisplay / 60);

		_UITimer.text = $"{min:00}:{sec:00}";


	}
}
