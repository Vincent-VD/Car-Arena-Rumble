using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackground : MonoBehaviour
{
	[SerializeField]
	private Sprite[] _backgroundImages = null;

	[SerializeField]
	private Image _targetImage = null;

	private void Start()
	{
		_targetImage.sprite = _backgroundImages[0];
	}

	public void UpdateBackground()
	{
		List<PlayerConfig> configs = PlayerManager.Instance.PlayerConfigs;
		_targetImage.sprite = _backgroundImages[configs.Count];
	}
}
