using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchAttack : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody = null;

    [SerializeField]
    private Transform _forward = null;

    [SerializeField]
    private float _attackDuration = 0.5f;

    [SerializeField]
    private float _attackForce = 10f;

    [SerializeField]
    private float _brakeForce = 1f;

    [SerializeField]
    private float _brakeForceStarts = 0.8f;

    [SerializeField]
    private ParticleSystem[] _launchParticles = null;

    private CarMovementBehavior _movementBehavior = null;


    private float _forceMultiplier = 1000f, _attackCounter = 0f;
    private bool _isAttacking = false;
    private float _startWaitDuration = 5f, _attackCooldown = 1f;
    private bool _canAttack = false;



    // Start is called before the first frame update
    void Start()
    {
        _attackCounter = _attackDuration*2f;
        _movementBehavior = GetComponent<CarMovementBehavior>();
    }

    void FixedUpdate()
    {
        _startWaitDuration -= Time.deltaTime;
        if (_startWaitDuration > 0) 
            return;

        if (!_canAttack)
            return;
        //Checks if the attack counter is lower then the attack duration with in mind when the brake force should start
        if (_attackCounter < _attackDuration * _brakeForceStarts)
        {      
            //aplies launch 
            LaunchForwards();
            _attackCounter += Time.deltaTime;
        }
        else if (_attackCounter < _attackDuration)
        {
            //applies brake
            BrakeLaunch();
            _attackCounter += Time.deltaTime;
    
        }
    }

    //Launches the car forward
    private void LaunchForwards()
    {
        _rigidbody.AddForce(
          _forward.forward * (_attackForce * _forceMultiplier * Mathf.Abs(_movementBehavior.ForwardAcceleration)));
    }
    //At the end of the launch, i will give it a force back to make it stop faster
    private void BrakeLaunch()
    {
        _rigidbody.AddForce( (-_forward.forward) * ((_brakeForce * Mathf.Abs(_movementBehavior.ForwardAcceleration)) * _forceMultiplier));
    }
    //when activated, it checks if it is ready (counter bigger attackduration)
    public void AttackLaunch(InputAction.CallbackContext context)
    {
        _isAttacking = context.performed;
        if (!_canAttack && _isAttacking && _startWaitDuration <= 0)
        {
            _attackCounter = 0f;
            _canAttack = true;
            PLayParticles();
            Invoke("ReadyToAttack", _attackCooldown);
        }
    }

    private void ReadyToAttack()
    {
        PauseParticles();
        _canAttack = false;
    }

    private void PLayParticles()
    {
        for (int i = 0; i < _launchParticles.Length; i++)
        {
            if (!_launchParticles[i].isPlaying)
            {
                _launchParticles[i].Play();
            }
        }
    }

    private void PauseParticles()
    {
        for (int i = 0; i < _launchParticles.Length; i++)
        {
            if (_launchParticles[i].isPlaying)
            {
                _launchParticles[i].Stop();
            }
        }
    }

}

