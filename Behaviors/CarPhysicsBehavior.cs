using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class CarPhysicsBehavior : MonoBehaviour
{
    //This is for looking where the car hits
    //the sides of the car
    public enum CarSide
    {
        front,
        rear,
        leftOrRight,
        none
    }
    [System.Serializable]
    public struct CarSidePoints
    {
        public CarSide carSide;
        public Transform transform;


    }
    //car collision vars
    [SerializeField]
    private float _ignoreSlowVelocitySqrMagn = 2f, _carKnockBackForce = 5f, _velocityLimitMultiplier = 1.2f;

    //car collision place var
    //These are the modiefier if you get hit by the other car with the side:
    [SerializeField]
    private float _frontImpactMod = 1f, _sideImpactMod = 0.8f, _rearImpactMod = 0.8f, _slideImpactMod = 0.1f;

    //Wall collision vars
    [SerializeField]
    private float _minImpactMultiplier = 1.5f, _minForwardSpeed = 0.5f, _wallKnockBackForce = 2.5f, _wallLowestVelocity = 6f;


    //car sides points
    [SerializeField]
    private CarSidePoints[] _carSidePointsArray;

    [SerializeField]
    private ParticleSystem[] _carHitParticleSystems;


    //private float[] _
    private const float _forceMultiplier = 200000f, _lowestVelocityMultiplier = 100f, _defaultImpactMod = 0.5f;
    private float _impactPlaceModifier = 1f, _velocityDifferenceModifier = 1f; 
    private const float  _maxMagSqrVelocity = 1000f, _maxAxisVelocity = 50f , _maxVelocityDifference = 0.2f, _maxModifier = 1.75f;
    private Rigidbody _rigidbodySphere = null;
    private Vector3 _prevVelocity = Vector3.zero, _defaultVelocity = new Vector3 (8f,1f,8f);
    private bool _isCollidingOnHit = false, _hasBeenRammed = false;
    private float _maxYeetSpeed = 0f;


    public CarSidePoints[] CarSidePointsArray
    {
        get
        {
            return _carSidePointsArray;
        }
    }

    public Vector3 PreviousVelocity
    {
        get
        {
            return _prevVelocity;
        }
    }

    public bool IsRammed
    {
        get
        {
            return _hasBeenRammed;
        }
    }   // Start is called before the first frame update
    void Start()
    {
        //clamping SerializeField variables to prevent breaking calculations and physics 
        _rigidbodySphere = GetComponent<Rigidbody>();
        _minForwardSpeed = Mathf.Clamp(_minForwardSpeed, 0f, 1f);
        _velocityLimitMultiplier = Mathf.Clamp(_velocityLimitMultiplier, 0, 100f);
        _frontImpactMod = Mathf.Clamp(_frontImpactMod, 0.1f, 10f);
        _rearImpactMod = Mathf.Clamp(_rearImpactMod, 0.1f, 10f);
        _sideImpactMod = Mathf.Clamp(_sideImpactMod, 0.1f, 10f);
        _slideImpactMod = Mathf.Clamp(_slideImpactMod, 0.1f, 10f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isCollidingOnHit)
        {
            _prevVelocity = _rigidbodySphere.velocity;
        }
    }

    void LateUpdate()
    {
        if (_maxYeetSpeed < _rigidbodySphere.velocity.sqrMagnitude)
        {
            _maxYeetSpeed = _rigidbodySphere.velocity.sqrMagnitude;
            PlayersScoreManager.Instance.UpdateVelocity(gameObject.GetInstanceID(),_maxYeetSpeed);
        }
    }

    //Checks wich side has the closest distamce and returns the closest side from the contactPoint
    private CarSide GetNearestSide(Vector3 contactPos, CarSidePoints[] carSidePointsArray)
    {
        int indexClosest = 0;
        float tmpDistance = 0f;
        float closestDistance = 100000f;
        for (int i = 0; i < carSidePointsArray.Length; i++)
        {
            //This checks if the current distance is closer or not
            tmpDistance = Vector3.Distance(carSidePointsArray[i].transform.position, contactPos);
            if (tmpDistance < closestDistance)
            {
                // if the other distance is close, then it will get the index and distance ofit
                closestDistance = tmpDistance;
                indexClosest = i;
            }
        }
        return carSidePointsArray[indexClosest].carSide;
    }

    //checking who rammed who (this car or the other car)
    private bool CheckIfIAmRammed(Collision collision, CarPhysicsBehavior otherCarPhysicsBehavior)
    {
        _impactPlaceModifier = _defaultImpactMod;

        //looks where the car hits
        CarSide otherCarSide = GetNearestSide(collision.GetContact(0).point, otherCarPhysicsBehavior.CarSidePointsArray);
        CarSide thisCarSide = GetNearestSide(collision.GetContact(0).point, _carSidePointsArray);

        //If the other car hits with te front or rear, this car has been rammed
        //We already calculated the min velocity before that, if the other car is too slow or standing still

        if (otherCarSide == CarSide.front && thisCarSide == CarSide.front)
        {
            //check if its a head on collision
            _impactPlaceModifier = _frontImpactMod + _defaultImpactMod/2;
            return true;
        }
        else if (otherCarSide == CarSide.front)
        {
            //This car has been rammed at the front
            _impactPlaceModifier = _frontImpactMod;
            return true;
        }
        else if (otherCarSide == CarSide.rear)
        {
            //This car has been rammed at the rear
            _impactPlaceModifier = _rearImpactMod;
            return true;
        }

        //checking of they are not just sliding against each other
        //Now we need to check if if the z and x velocity speed, so we can now if its a sliding or bouncing against each other
        //If the other z or x velocity is bigger with x amount, then we can see it as the other ramming this car
        if (Mathf.Abs(otherCarPhysicsBehavior.PreviousVelocity.z) > Mathf.Abs(_prevVelocity.z) * _velocityLimitMultiplier)
        {
            //this car is rammed because my z velocity is lower then the other car
            _impactPlaceModifier = _sideImpactMod;
            return true;
        }//vice verca with x
        else if (Mathf.Abs(otherCarPhysicsBehavior.PreviousVelocity.x) > Mathf.Abs(_prevVelocity.x) * _velocityLimitMultiplier)
        {
            //this car is rammed because my x velocity is lower then the other car
            _impactPlaceModifier = _sideImpactMod;
            return true;
        }



        //If both cars are around about the same speed and just slide against slid to each sides, the impact lessens 
        if (otherCarSide == CarSide.leftOrRight && thisCarSide == CarSide.leftOrRight)
        {
            _impactPlaceModifier = _slideImpactMod;
        }
        return false;
    }
    //Here we calculate the velocity modifier
    private void CalculateVelocityModifier(Vector3 otherPrevVelocity)
    {
        //resets the velocity modifier
        _velocityDifferenceModifier = 0.5f;

        //checks of the velocity is not too low, if it is then it sets it to a default value
        Vector3 otherVelocity = CheckSizeVelocity(otherPrevVelocity);
        Vector3 thisVelocity = CheckSizeVelocity(_prevVelocity);

        //turning it into percentage aka between 0 and 1
        //calculates the velocity by seeing how fast one is campareted to the other
        otherVelocity.x /= _maxAxisVelocity;
        otherVelocity.z /= _maxAxisVelocity;
        thisVelocity.x /= _maxAxisVelocity;
        thisVelocity.z /= _maxAxisVelocity;

        if (_hasBeenRammed)
        {
            _velocityDifferenceModifier += (otherVelocity.x + thisVelocity.x)/2;
            _velocityDifferenceModifier += (otherVelocity.z + thisVelocity.z)/2;
        }
        else
        {
            _velocityDifferenceModifier += otherVelocity.x - thisVelocity.x;
            _velocityDifferenceModifier += otherVelocity.z - thisVelocity.z;
        }
        
        _velocityDifferenceModifier = Mathf.Clamp(_velocityDifferenceModifier, 0.1f, _maxModifier);
    }

    //This checks if the vector is big enough for the calculation, if not it gets a default value
   private Vector3 CheckSizeVelocity(Vector3 velocity)
   {
       velocity.z = Mathf.Clamp(Mathf.Abs(velocity.z), _defaultVelocity.z, _maxAxisVelocity);
       velocity.x = Mathf.Clamp(Mathf.Abs(velocity.x), _defaultVelocity.x, _maxAxisVelocity);

       velocity.y = _defaultVelocity.y;
        return velocity;
   }

   private void ShowParticleOnHit(Collision collision)
   {
       foreach (var VARIABLE in _carHitParticleSystems)
       {
           if (!VARIABLE.isPlaying)
           {
               VARIABLE.transform.position = collision.GetContact(0).point;
               VARIABLE.Play();
               return;
           }
        }
      
    }
   


   //Does the impact of this car and calculates how far it will be knockedback
    public void CarCollisionOnHit(Collision collision, float impactMultiplier)
    {
        //Say to this class that the colliding has begun
        _isCollidingOnHit = true;
        //checking if the colliding car is driving very slow/standing still

        CarPhysicsBehavior otherCarPhysicsBehavior = collision.body.gameObject.GetComponent<CarPhysicsBehavior>();
        if (otherCarPhysicsBehavior == null)
            return;
     
        float otherCarSqrVelocity = otherCarPhysicsBehavior.PreviousVelocity.sqrMagnitude;

        //checking if the other one is standing still or being very slow
        if (otherCarSqrVelocity < (_ignoreSlowVelocitySqrMagn * _lowestVelocityMultiplier) || otherCarSqrVelocity < _prevVelocity.sqrMagnitude / 2f)
        {
            //The other  car is so slow or standing still, that you dont get negative impacted in any way
            //Colliding ends
            _hasBeenRammed = false;
            _isCollidingOnHit = false;
            return;
        }
        //Now lets see who is going faster and that will influence the direction
        //The direction of a person will be stronger if that person has a higher velocity
        Vector3 directionModifier = _prevVelocity - otherCarPhysicsBehavior.PreviousVelocity;

        //float relativeVelocity = collision.relativeVelocity.magnitude;
        //making the direction where this car goes to
        Vector3 direction = (transform.position - collision.body.transform.position);
        direction += directionModifier.normalized;
        direction = direction.normalized;
        //to prevent launching upwards
        direction.y = 0.1f; ;


        //Checking if this car has been rammed
        //Note: the way i made it, can be that both cars can be counted as rammed, this is intentional
        //Check where the car is impacted for both cars
        _hasBeenRammed = CheckIfIAmRammed(collision, otherCarPhysicsBehavior);
        //calculating the velocity modifier
        CalculateVelocityModifier(otherCarPhysicsBehavior.PreviousVelocity);

        //Show ParticleHit!
        ShowParticleOnHit(collision);

        //knock-back of car
        _rigidbodySphere.AddForce(direction * ((_carKnockBackForce * _forceMultiplier) *
            (impactMultiplier * _impactPlaceModifier * _velocityDifferenceModifier)));


        //Colliding ends
        _isCollidingOnHit = false;
    }
    //Any collission with the car on wall will be calculated and excecuted here 
    public void WallCollisionOnHit(Collision collision, float impactMultiplier, float forwardAcceleration)
    {
        //Say to this class that the colliding has begun
        _isCollidingOnHit = true;
        //When the car is moving slow and has its impact multiplier low, it will only slowdown
        if (impactMultiplier >= _minImpactMultiplier && (forwardAcceleration <= _minForwardSpeed
            || PreviousVelocity.sqrMagnitude < (_wallLowestVelocity * _lowestVelocityMultiplier)))
        {
            //Colliding ends
            _isCollidingOnHit = false;
            return;
        }
        //If the car is moving too fast then, we will have it bounce it from the wall in relative to its speed, velocity and impactMult
        //making the direction where this car goes to
        Vector3 toWallDirection = (collision.GetContact(0).point - transform.position);
        //calculates the reflect, so it bounces off in an angle
        Vector3 reflectDirection = Vector3.Reflect(toWallDirection, collision.GetContact(0).normal);
        reflectDirection = reflectDirection.normalized;
        reflectDirection.y = 0.1f;
        //Show ParticleHit!
        ShowParticleOnHit(collision);

        //The knockback force aplied to the car
        _rigidbodySphere.AddForce(reflectDirection * ((_wallKnockBackForce * (_forceMultiplier * 0.5f)) * impactMultiplier));
        _isCollidingOnHit = false;
    }
    //Collision with general things, like car mods and others
    public void GeneralCollisionOnHit(Transform otherTransform, float impactMultiplier)
    {
        //making the direction where this car goes to
        Vector3 direction = (transform.position - otherTransform.position);
        direction = direction.normalized;

        // knock-back of car
        _rigidbodySphere.AddForce(direction * ((_carKnockBackForce * _forceMultiplier) * impactMultiplier));
    }
    //Any collission with the car on carmod will be calculated and excecuted here 
    public void CarModCollisionOnHit(Collision collision, float impactMultiplier )
    {
        //init modpart
        BaseItem carmod = collision.body.gameObject.GetComponent<BaseItem>();
        if (carmod == null)
            return;
      
        //making the direction where this car goes to
        Vector3 direction = (transform.position - collision.body.transform.position);
        direction = direction.normalized;
        //to prevent launching upwards
        direction.y = 0.1f; ;
        //Calculating velocity modifer with the default velocity
        CalculateVelocityModifier(_defaultVelocity);
        _hasBeenRammed = carmod.CountAsRamming;
        carmod.ActivatePartciles(collision.contacts[0].point);
        
        //knock-back of car
        _rigidbodySphere.AddForce(direction * ((_carKnockBackForce * _forceMultiplier) * (impactMultiplier * carmod.ModPartModifier * _velocityDifferenceModifier)));
    }


}
