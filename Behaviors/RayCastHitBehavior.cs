using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHitBehavior : MonoBehaviour
{
    [SerializeField]
    private float _rayDistance = 4f;

    private int GROUND_LAYER =0;
    private RaycastHit _raycastHit;
    private bool _isHit = false;
    private float _minDistanceOnGround = 0.92f, _maxDistanceOnGround = 1.3f;
    
    // Start is called before the first frame update
    void Start()
    {
        GROUND_LAYER = LayerMask.NameToLayer("Ground");
    }
    void FixedUpdate()
    {
        //Cast a raycast!
        Physics.Raycast(transform.position, -transform.up, out _raycastHit, _rayDistance);
        if (_raycastHit.rigidbody == null)
        {
            _raycastHit.distance = float.MaxValue;
            _isHit = false;
            return;
        }
        _isHit = _raycastHit.rigidbody.gameObject.layer == GROUND_LAYER;
  
    }

    public bool IsHit
    {
        get { return _isHit; }
    }


    public float Distance
    {
        get { return _raycastHit.distance; }
    }

    public float MaxDistanceOnGround
    {
        get { return _maxDistanceOnGround; }
    }

    public float MinDistanceOnGround
    {
        get { return _minDistanceOnGround; }
    }
}
