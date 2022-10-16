using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactMultiplier : MonoBehaviour
{
	[SerializeField] 
	private float _impactMultiplier = 1f, _impactIncrease = 0.1f, _maxImpactAmount = 3f;

	private float _startAmount = 0f;
	private const float _toPercent = 100f, _randomRange = 0.1f;
	private int _playerId;


	public float ImpactMultiplierVal
	{
		get
		{
			return _impactMultiplier;
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		_playerId = GetComponent<BasicCarCharachter>().PlayerID;
		_startAmount = _impactMultiplier;
	}

	//When used, the impact multiplier gets increased
	public void IncreaseMultplier()
	{
		_impactMultiplier += _impactIncrease + Random.Range(-_randomRange, _randomRange);
		if (_impactMultiplier > _maxImpactAmount)
			_impactMultiplier = _maxImpactAmount;
		//This will give the normalized aka to percent 
		HUDManager.Instance.HandleDamageIncrease(_playerId,(int)(Mathf.Round((_impactMultiplier - _startAmount) * _toPercent)));

	}
	public void IncreaseMultplier(float extraIncrease)
	{
		_impactMultiplier += _impactIncrease + extraIncrease + Random.Range(-_randomRange, _randomRange);
		if (_impactMultiplier > _maxImpactAmount)
			_impactMultiplier = _maxImpactAmount;
		//This will give the normalized aka to percent 
		HUDManager.Instance.HandleDamageIncrease(_playerId, (int)(Mathf.Round((_impactMultiplier - _startAmount) * _toPercent)));
	}
	//Resets the impact multiplier
	public void ResetMultiplier()
	{
		_impactMultiplier = _startAmount;
		//This will give the normalized aka to percent 
		HUDManager.Instance.HandleDamageIncrease(_playerId, 0);

	}


}
