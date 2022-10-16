using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

	[SerializeField]
	private TextMeshProUGUI _playerText = null;

	[SerializeField]
	private TextMeshProUGUI _impactMultiplier = null;

	[SerializeField]
	private TextMeshProUGUI _killAmount = null;

	[SerializeField]
	private Image _playerSprite = null;

	[SerializeField]
	private Animator _damageAnimator = null;

	[SerializeField]
	private GameObject _winningCrown = null;

	[SerializeField]
	private Image _backgroundImageCar = null;

	private const float _maxColorValue = 255f, _maxMultiplierValue = 200f;

	public void SetPlayerSprite(Sprite sprite)
	{
		_playerSprite.sprite = sprite;
	}

	public void SetPlayerId(int playerId)
	{
		_playerText.SetText("PLAYER " + (playerId + 1));
	}

	public void SetImpactMultiplier(float val)
	{
		_impactMultiplier.SetText("{0:#000}%", val);
		Color currColor = _impactMultiplier.color;
		if (val != 0)
		{
			_impactMultiplier.color = new Color(currColor.r, currColor.g - (val * 0.5f) / _maxColorValue, currColor.b - (val * 0.5f) / _maxColorValue);
		}
		else
		{
			_impactMultiplier.color = new Color(1f, 1f, 1f);
		}
		_damageAnimator.SetTrigger("TrActivate");
		UpdateBackgroundColor(val);	
	}
	public void setKillIncrease(int val)
	{
		_killAmount.text = val.ToString();
	}

	public void SetWinning(bool newVal)
	{
		_winningCrown.SetActive(newVal);
	}


	private void UpdateBackgroundColor(float val)
    {
		//To avoid calculating with 0
		if (val < 1f)
			val = 1f;
		Color tmpColor = _backgroundImageCar.color;
		//calculate from 255 to 125 
		tmpColor.r = 1f - ((val / _maxMultiplierValue) * 1f / 2f);
		//calculate from 255 to 0
		tmpColor.g = 1f - ((val / _maxMultiplierValue) * 1f);
		tmpColor.b = 1f - ((val / _maxMultiplierValue) * 1f);
		_backgroundImageCar.color = tmpColor;
	}
}
