using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleBehavior : MonoBehaviour
{
	private Gamepad _targetGamepad = null;
	public Gamepad TargetGamePad
	{
		set => _targetGamepad = value;
	}

    private float _time = 0;

    private bool _isActive = false;

    private void Awake()
    {
	    PlayerInput input = GetComponent<PlayerInput>();
	    _targetGamepad = Gamepad.all.FirstOrDefault(p => input.devices.Any(d => d.deviceId == p.deviceId));
    }

	private void OnDestroy()
	{
		ResetHaptics();
	}
	public void RumblePulse()
    {
	    if (_isActive) return;
	    _targetGamepad.SetMotorSpeeds(0.3f, 0.8f);
	    Invoke("ResetHaptics", 0.2f);
	    _isActive = true;
    }

    public void RumbleIntense()
    {
	    if (_isActive) return;
	    _targetGamepad.SetMotorSpeeds(0.8f, 0.1f);
	    Invoke("ResetHaptics", 0.6f);
	    _isActive = true;
	}

    public void RumbleSoft()
    {
	    if (_isActive) return;
	    _targetGamepad.SetMotorSpeeds(0.2f, 0.8f);
	    Invoke("ResetHaptics", 0.4f);
	    _isActive = true;
	}

    private void ResetHaptics()
    {
	    _targetGamepad.ResetHaptics();
	    _isActive = false;
	}
}
