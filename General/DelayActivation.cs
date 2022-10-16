using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DelayActivation : MonoBehaviour
{
	[SerializeField]
	private PlayerInputManager _toDelay = null;

	[SerializeField]
	private float _delay = 9f;
    void Start()
	{
		Invoke("Activate", _delay);
	}

    private void Activate()
    {
	    _toDelay.enabled = true;
    }
   
}
