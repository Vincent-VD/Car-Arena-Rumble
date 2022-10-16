using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimationBehavior : MonoBehaviour
{
    [SerializeField] 
    private Transform _frontLeftWheel = null;

    [SerializeField]
    private Transform _frontRightWheel = null;

    [SerializeField]
    private Transform _particleLefTransform =null;

    [SerializeField]
    private Transform _particleRightTransform = null;

    [SerializeField]
    private Renderer _bodyRenderer = null;

    [SerializeField]
    private Material _transparantMaterial = null;

    private Material[] _defaultMaterials = null;
    private Material[] _transparantMaterials = null;

    [SerializeField]
    private ParticleSystem[] _tireTrackParticles = null;

    private CarMovementBehavior _carMovementBehavior = null;
    private Health _health = null;
    private const float _rotationSpeed = 80f, _angleSpace = 30f, _halfCircle = 180f, _minMoveSpeed = 0.05f;
    private float _scale = 0f, _leftSmokeScaleModifier = 1f, _rightSmokeScaleModifier = 1f;
 



    // Start is called before the first frame update
    void Start()
    {
        _scale = _particleRightTransform.localScale.z;
        _defaultMaterials = _bodyRenderer.materials;
        _transparantMaterials = new Material[_bodyRenderer.materials.Length];
        _transparantMaterial.color =Color.white;
        for (int i = 0; i < _bodyRenderer.materials.Length; i++)
        {
            _transparantMaterials[i] = _transparantMaterial;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_carMovementBehavior)
            return;
       RotateWheels();
    }

    private void Update()
    {
        UpdateTireTracksParticles();
        UpdateDustParticleSize();
        UpdateInvincibleShader();
    }
    //This updates the partciles, when the raycasts is off the ground, that tire track particle on that tire will be paused 
    //untill its back on the ground
    private void UpdateTireTracksParticles()
    {
        for (int i = 0; i < _tireTrackParticles.Length; i++)
        {   //If the raycast is on the ground and if the tire particle is paused
            if (_carMovementBehavior.RayCastInfoArray[i].rayCast.IsHit && _tireTrackParticles[i].isPaused)
            {//If that is true then play the tire particle
                _tireTrackParticles[i].Play();
            }//If the raycast is not on the ground and if the tire particle is playing
            else if (!_carMovementBehavior.RayCastInfoArray[i].rayCast.IsHit && _tireTrackParticles[i].isPlaying)
            {//If that is true then pause the tire particle
                _tireTrackParticles[i].Pause();
            }
        }
    }
    //Sets the car movement behavior 
    public void SetCarMovementBehavior(CarMovementBehavior carMovementBehavior)
    {
        if (carMovementBehavior)
        {
            _carMovementBehavior = carMovementBehavior;        
        }
    }

    public void SetHealth(Health health)
    {
        _health = health;
    }


    //Rotates the wheels of a car, the same way they would be if you are steering
    private void RotateWheels()
    {
        //Calculate front wheel
        //left from wheel
        Vector3 tmpVector = _frontLeftWheel.eulerAngles;
        tmpVector.y = Mathf.Clamp(_frontLeftWheel.parent.eulerAngles.y + (_carMovementBehavior.SteeringValue * _rotationSpeed), 
            _frontLeftWheel.parent.eulerAngles.y - _angleSpace, _frontLeftWheel.parent.eulerAngles.y + _angleSpace);
        _frontLeftWheel.eulerAngles = tmpVector;
        //right front wheel
        tmpVector = _frontRightWheel.eulerAngles;
        tmpVector.y = Mathf.Clamp(_frontRightWheel.parent.eulerAngles.y + _halfCircle + (_carMovementBehavior.SteeringValue * _rotationSpeed), 
            _frontRightWheel.parent.eulerAngles.y + _halfCircle - _angleSpace, _frontRightWheel.parent.eulerAngles.y + _halfCircle + _angleSpace);
        _frontRightWheel.eulerAngles = tmpVector;
    }

    //changed de z scaling of the particlesystem, to show how fast you are going
    private void UpdateDustParticleSize()
    {
        //When standing still or almost standing still, the smoke particle will not be shown or rather your wont see it due to the small scale
        if (Mathf.Abs(_carMovementBehavior.ForwardAcceleration)  < _minMoveSpeed)
        {
            _leftSmokeScaleModifier = 0f;
            _rightSmokeScaleModifier = 0f;
            //Stops the particles if its not driving (if its playing)
            for (int i = 0; i < _tireTrackParticles.Length; i++)
            {
                if (_tireTrackParticles[i].isPlaying)
                {
                    _tireTrackParticles[i].Pause();
                }
            }
        }
        else 
        {//This checks if a tire is above ground or if it is above grouynd then it will not show smoke and vise versa
            if (!_carMovementBehavior.RayCastInfoArray[2].rayCast.IsHit)//Right tire
            {
                _rightSmokeScaleModifier = 0f;
            }
            else
            {
                _rightSmokeScaleModifier = 1f;
            }
            if (!_carMovementBehavior.RayCastInfoArray[3].rayCast.IsHit)//Left Tire
            {
                _leftSmokeScaleModifier = 0f;
            }
            else
            {
                _leftSmokeScaleModifier = 1f;
            }
          
        }
        _particleLefTransform.localScale = new Vector3(_scale * _leftSmokeScaleModifier, _scale * _leftSmokeScaleModifier, (_scale * _carMovementBehavior.ForwardAcceleration) * _leftSmokeScaleModifier);
        _particleRightTransform.localScale = new Vector3(_scale * _rightSmokeScaleModifier, _scale * _rightSmokeScaleModifier, (_scale * _carMovementBehavior.ForwardAcceleration) * _rightSmokeScaleModifier);
    }

    //This updates the shaders in the body renderer, if it is invincible then it becomes trasnparrant and default if its not
   private void UpdateInvincibleShader()
    {
        if (!_health)
            return;
 
        if (_health.IsInvincible && _bodyRenderer.material != _transparantMaterial)
        {
            _bodyRenderer.materials = _transparantMaterials;      
        }
        else if(!_health.IsInvincible && _bodyRenderer.material != _defaultMaterials[0])
        {
            _bodyRenderer.materials = _defaultMaterials;
        } 
    }

}
