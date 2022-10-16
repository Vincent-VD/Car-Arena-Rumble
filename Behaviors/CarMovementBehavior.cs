using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovementBehavior : MonoBehaviour
{


    //Serialized fields for the movement of the car and the bodies
    [SerializeField]
    private Transform _forwardBody = null;

    [SerializeField]
    private Transform _collisionBox = null;

    [SerializeField]
    private Vector3 _forwardBodyOffset = Vector3.zero;
    //Forward movement 
    [SerializeField]
    private float _maxMovementSpeed = 50f, _secToMaxSpeed = 1f, _maxAccelerateBackward = -0.5f, _maxAccelerateForward = 1f, _minForwardAccForSteer = 0.1f;
    //Steering
    [SerializeField]
    private float _rollOutCarSpeed = 10f, _brakeStrength = 2f, _turnStrength = 50f, _turnReductionBySpeed = 0.7f, _airSteeringPenalty = 0.5f;
    //On wall
    [SerializeField]
    private float _slowOnWallAmount = 1f, _maxSlowOnWallAmount = 0.3f;

    [SerializeField]
    private float _FallRotationSpeed = 5f;

    [SerializeField]
    private RayCastInfo[] _rayCastInfoArray = null;

    private float _totalAccelerateForward = 0f, _steeringAmount = 0f, _acceleration = 0f, _airSteeringModfier = 1f;
    private bool _isSteering = false, _isDriving = false, _onGround = true, _isActive = true;
    private const float _movementMultiplier = 1000f, _steerMultiplier = 20f, _slowOnWallMultiplier = 10, _midAirRotationMod = 3f, _halfCircle = 180f;

    private Rigidbody _rigidbodySphere = null;

    [System.Serializable]
    public struct RayCastInfo
    {
        public RayCastHitBehavior rayCast;
        public int xAxis;
        public int zAxis;
    }

    public RayCastInfo[] RayCastInfoArray => _rayCastInfoArray;

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }
    public float ForwardAcceleration => _totalAccelerateForward;

    public float SteeringValue => _steeringAmount;

    public Transform ForwardBody => _forwardBody;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody tmpRigid = GetComponent<Rigidbody>();
        if (tmpRigid)
        {
            _rigidbodySphere = tmpRigid;
        }

        if (_forwardBody)
            _forwardBody.parent = null;

        if (_secToMaxSpeed <= 0f)
        {
            _secToMaxSpeed = 1f;
        }

        if (_brakeStrength <= 0f)
        {
            _brakeStrength = 1f;
        }
        if (_slowOnWallAmount <= 0f)
        {
            _slowOnWallAmount = 1f;
        }

        _slowOnWallAmount = _slowOnWallMultiplier / _slowOnWallAmount;
        //making it inverse to increase performance! (avoiding devision)
        _slowOnWallAmount = 1 / _slowOnWallAmount;

        _turnReductionBySpeed = Mathf.Clamp(_turnReductionBySpeed, 0.1f, 1f);
        _airSteeringPenalty = Mathf.Clamp(_airSteeringPenalty, 0.1f, 1f);
        UpdatePositions();
    }

    //Sets the car driving status, from is driving forward or not at all
    public void DriveForward(InputAction.CallbackContext context)
    {
        _isDriving = context.performed;
        _acceleration = _maxAccelerateForward / _secToMaxSpeed;
    }

    //Sets the car driving status, from is driving backward or not at all
    public void DriveBackward(InputAction.CallbackContext context)
    {
        _isDriving = context.performed;
        _acceleration = (_maxAccelerateBackward / _secToMaxSpeed);
    }

    //Sets the car driving status, from is steering or not and gets the steering value
    public void Steer(InputAction.CallbackContext context)
    {
        _isSteering = context.performed;
        _steeringAmount = context.ReadValue<Vector2>().x / 2f;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isActive)
            return;
        UpdateAirSteerMod();
        FixUpdateCarMovement();
        UpdatePositions();
        UpdateRotation();
    }

    //Updates the car movement
    private void FixUpdateCarMovement()
    {
        //check if the car is moving forward, but the player is not on the gas paddle
        if (!_isDriving && Mathf.Abs(_totalAccelerateForward) > 0.01f || !_onGround)
        {
            //Now the car will slow down at a slow pace
            _totalAccelerateForward -= _totalAccelerateForward / _rollOutCarSpeed;
        }
        //settng the acceleration to zero to prevent constant driving
        if (!_isDriving && Mathf.Abs(_totalAccelerateForward) < 0.01f)
        {
            _totalAccelerateForward = 0f;
            return;
        }

        //when driving the car moves forward or backwar at an accelerated speed which is also clamped
        if (_isDriving)
        {
            //When driving forward and pressing the brake/backward button, the car will slow down faster, this is also vise versa
            if (_totalAccelerateForward > 0f && _acceleration < 0f || _totalAccelerateForward < 0f && _acceleration > 0f)
            {
                _totalAccelerateForward += (_acceleration * _brakeStrength) * Time.deltaTime;
            }
            else
            {
                _totalAccelerateForward += _acceleration * Time.deltaTime;
            }
            _totalAccelerateForward = Mathf.Clamp(_totalAccelerateForward, _maxAccelerateBackward, _maxAccelerateForward);
        }

        //moving forward or backward with the sphere, while using the forward of the forwardBody
        float movementForward = _totalAccelerateForward * (_maxMovementSpeed * _movementMultiplier);
        _rigidbodySphere.AddForce(_forwardBody.forward * movementForward);

        //Calulating and steering the forward body
        if (_isSteering && Mathf.Abs(_totalAccelerateForward) > _minForwardAccForSteer)
        {
            //Making it so that the steering power is relative to the speed of the car (lower speed == faster turning)
            float steerForwardPower = Mathf.Clamp(1 / Mathf.Abs(movementForward), _turnReductionBySpeed, 1f);
            float steeringTotal = (_steeringAmount * (_turnStrength * Time.deltaTime) * (steerForwardPower * _steerMultiplier)) * _airSteeringModfier;
            _forwardBody.rotation = Quaternion.Euler(_forwardBody.rotation.eulerAngles + new Vector3(0f, steeringTotal, 0f));
        }

    }

    //Updates the positions of the forwadbody and the collision
    private void UpdatePositions()
    {
        //place the forward body on the sphere + offset to let it stay on the car
        _forwardBody.position = transform.localPosition + _forwardBodyOffset;
        //Sets position and rotation of the collisionBox to the _forwardBody to prevent it rotating 
        _collisionBox.rotation = _forwardBody.rotation;
        _collisionBox.position = _forwardBody.position;
    }

    public void AlignForwardBody(Quaternion quat)
    {
        _forwardBody.rotation = quat;
        UpdatePositions();
    }
    //This is where the car will be slowed when it gets called
    public void SlowOnWall()
    {
        //Slowing amount for the car
        _rigidbodySphere.velocity -= _rigidbodySphere.velocity * (_slowOnWallAmount);
        _totalAccelerateForward -= _totalAccelerateForward * _slowOnWallAmount;
        //Clamping the car, so that it doesnt stand still
        if (_acceleration > 0)
        {
            _totalAccelerateForward = Mathf.Clamp(_totalAccelerateForward, _maxSlowOnWallAmount, 1f);
        }
        else
        {
            _totalAccelerateForward = Mathf.Clamp(_totalAccelerateForward, _maxAccelerateBackward, -_maxSlowOnWallAmount * 0.5f);
        }
    }

    //Updates the x and z rotation of the forward body!
    private void UpdateRotation()
    {
        //values to see which direction and how much rotation speed to that diretion should be aplied
        float xAngleValue = 0f;
        float zAngleValue = 0f;
        _onGround = false;

        foreach (var rayCastInfo in _rayCastInfoArray)
        {
            if (rayCastInfo.rayCast.IsHit)
            {

                _onGround = true;
            }
            //if its need to go up
            if (rayCastInfo.rayCast.Distance < rayCastInfo.rayCast.MinDistanceOnGround)
            {
                xAngleValue -= rayCastInfo.zAxis;
                zAngleValue -= rayCastInfo.xAxis;
            }//if it needs to go down
            else if (!rayCastInfo.rayCast.IsHit || rayCastInfo.rayCast.Distance > rayCastInfo.rayCast.MaxDistanceOnGround)
            {
                xAngleValue += rayCastInfo.zAxis;
                zAngleValue += rayCastInfo.xAxis;               
            }
        }
        ////now it will nose dive in the direction its going when its off the ground
        if (!_onGround)
        {
            if (_forwardBody.eulerAngles.x > _halfCircle)
            {
                xAngleValue = _midAirRotationMod;
            }
            else
            {
                xAngleValue = -_midAirRotationMod;
            }
           
        }
        //Rotates
        _forwardBody.Rotate(Vector3.right, xAngleValue * (_FallRotationSpeed * Time.deltaTime));
        _forwardBody.Rotate(Vector3.back, zAngleValue * (_FallRotationSpeed * Time.deltaTime));
    }

   //Changed the angluar drag of the car when its off ground or not
   private void UpdateAirSteerMod()
    {
        if (_onGround)
        {
           _airSteeringModfier = 1f;
        }
        else
        {
            _airSteeringModfier = _airSteeringPenalty;
        }
    }

}
