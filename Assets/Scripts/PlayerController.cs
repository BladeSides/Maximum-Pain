using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    
    [SerializeField] private float _maxVelocity = 1f;
    [SerializeField] private float _moveAcceleration = 1f;
    [SerializeField] private float _moveDeceleration = 5f;

    [SerializeField] private float _groundedDownwardVelocity = 0.01f;

    [SerializeField] private Vector3 _playerVelocity;

    [SerializeField] private float _gravitationalAcceleration;
    [SerializeField] private float _jumpVelocity = 10f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        _gravitationalAcceleration = Physics.gravity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        _playerVelocity += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _moveAcceleration * Time.deltaTime;
        if (_playerVelocity.sqrMagnitude > _maxVelocity*_maxVelocity)
        {
            _playerVelocity = _playerVelocity.normalized * _maxVelocity;
        }

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            Vector3 velocityChange = new Vector3(_playerVelocity.x,0,_playerVelocity.z) * _moveDeceleration * Time.deltaTime;
            if (_playerVelocity.sqrMagnitude - velocityChange.sqrMagnitude > 0) //If deceleration won't make it go in opposite direction
            {
                _playerVelocity -= velocityChange;
            }
            else
            {
                _playerVelocity = new Vector3(0, _playerVelocity.y, 0);
            }
        }

        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded && _playerVelocity.y <= 0)
        {
            _playerVelocity += Vector3.up * _jumpVelocity;
        }
        else if (_characterController.isGrounded == false)
        {
            _playerVelocity += Vector3.down * Time.deltaTime * _gravitationalAcceleration;
        }
        else
        {
            _playerVelocity = new Vector3(_playerVelocity.x, -_groundedDownwardVelocity, _playerVelocity.z);
        }

    }

    private void FixedUpdate()
    {
        _characterController.Move(_playerVelocity*Time.deltaTime);
    }
}
