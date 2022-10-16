using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayAnimation : MonoBehaviour
{
	[SerializeField]
	private Animator _animation = null;

	[SerializeField]
	private String _command = null;

	[SerializeField]
	private float _delay = 0f;

	private void Start()
	{
		this.gameObject.SetActive(false);
		Invoke("Activate", _delay);
	}

	public void Activate()
	{
		this.gameObject.SetActive(true);
		_animation.SetTrigger(_command);
	}
}
