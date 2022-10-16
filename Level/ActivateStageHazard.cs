using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateStageHazard : MonoBehaviour
{

	[SerializeField]
	private Animator _hazardAnimator = null;

	[SerializeField] private float _cooldown = 5.0f;

	[SerializeField]
	private float _knockbackForce = 1000000f;

	private bool _isOpen = false;

	private void OnCollisionEnter(Collision other)
	{
		if (other.body.CompareTag("Player") && !_isOpen)
		{
			SoundManager.Instance.PlayBouncySpringSound();
			Rigidbody rigidBody = other.body.GetComponentInParent<Rigidbody>();
			Debug.Log("Playing animation");
			_hazardAnimator.SetTrigger("TrOpen");
			Vector3 forward = this.transform.forward;
			rigidBody.AddForce(forward * _knockbackForce);
			_isOpen = true;
			Invoke("ResetIsActive", _cooldown);
		}
	}

	private void ResetIsActive()
	{
		_hazardAnimator.SetTrigger("TrClose");
		_isOpen = false;
	}
}
