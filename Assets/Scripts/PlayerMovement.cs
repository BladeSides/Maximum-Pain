using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Input;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    //Character Controller
    private CharacterController _characterController;

    //Velocities
    [SerializeField] private Vector3 _playerPlanarVelocity;
    [SerializeField] private Vector3 _playerVerticalVelocity;
    
    public Vector3 movement;

    //ShootDodge Values
    [SerializeField] private float _standupTimer;
    [SerializeField] private Vector3 _shootDodgeDirection;
    [SerializeField] public bool _isShootDodging;
    [SerializeField] public bool _isProne;

    //Character Controller Values
    [SerializeField] private float _gravitationalAcceleration;
    [SerializeField] private float _maxVelocity = 1f;
    [SerializeField] private float _moveAcceleration = 1f;
    [SerializeField] private float _moveDeceleration = 5f;
    [SerializeField] private float _groundedDownwardVelocity = 0.1f;
    [SerializeField] private float _jumpVelocity = 10f;
    [SerializeField] private float _characterControllerDefaultHeight;

    //Animating Values
    [SerializeField] private float _angleRotateSpeed = 3f;

    //Character Mesh
    [SerializeField] private GameObject _mesh;

    //Input
    private Vector2 _Input = Vector2.zero;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _gravitationalAcceleration = Physics.gravity.magnitude;
        _characterControllerDefaultHeight = _characterController.height;
    }
    private void FixedUpdate()
    {
        movement = Vector3.zero;
        movement += transform.right * _playerPlanarVelocity.x + transform.up * _playerVerticalVelocity.y + transform.forward * _playerPlanarVelocity.z;
        _characterController.Move(movement * Time.deltaTime);
    }


    // Update is called once per frame
    void Update()
    {
        SetInput();

        SetPlanarVelocity(); //As in, not jumping on the Y-Axis

        SetVerticalVelocity();

        ShootDodge();
    }

    private void SetInput()
    {
        _Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void ShootDodge()
    {
        if (Input.GetKeyDown(KeyCode.C) && CanJump())
        {
            _playerPlanarVelocity = new Vector3(_Input.x, 0, _Input.y);
            _playerVerticalVelocity = new Vector3(0, _jumpVelocity, 0);
            _characterController.Move(_jumpVelocity * Vector3.up * Time.deltaTime);
            Time.timeScale = 0.1f;
            _characterController.height = _characterControllerDefaultHeight / 2;
            _isShootDodging = true;
            _shootDodgeDirection = this.transform.forward;
            _playerPlanarVelocity = _playerPlanarVelocity.normalized * _maxVelocity;
        }
        if (_isShootDodging)
        {
            if (_playerPlanarVelocity.sqrMagnitude >= 0.25) //Ofset
            {
                Quaternion targetAngle = Quaternion.Euler(
                    new Vector3(_playerPlanarVelocity.z, 0, -_playerPlanarVelocity.x).normalized * 90);
                _mesh.transform.localRotation = RotateAngle(_mesh.transform.localRotation, targetAngle, _angleRotateSpeed * Time.deltaTime);
            }
        }
        if (_isShootDodging && IsGrounded())
        {
            _isProne = true;
        }

        if (_isProne)
        {
            _standupTimer += Time.deltaTime;
        }

        if (_isShootDodging && _isProne && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) // Moves when prone
        {
            if (_standupTimer > 0.5f)
            {
                _characterController.Move(_characterControllerDefaultHeight / 2 * Vector3.up);
                _characterController.height = _characterControllerDefaultHeight;
                _isShootDodging = false;
                _isProne = false;
                _standupTimer = 0;
            }
        }

        if (!_isShootDodging && !_isProne) //Make player stand up
        {
            _mesh.transform.localRotation = RotateAngle(_mesh.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), _angleRotateSpeed * Time.deltaTime);
        }
    }

    private void SetPlanarVelocity()
    {
        if (_isShootDodging && !_isProne)
        {
            return;
        }
        if (!_isProne) //Prevents sliding after shoot dodging while laying on the ground
        {
            _playerPlanarVelocity += new Vector3(_Input.x, 0, _Input.y) * _moveAcceleration * Time.deltaTime;
        }
        if (_playerPlanarVelocity.sqrMagnitude > _maxVelocity * _maxVelocity)
        {
            _playerPlanarVelocity = _playerPlanarVelocity.normalized * _maxVelocity;
        }

        if (_Input.x == 0 && _Input.y == 0 && (!_isShootDodging || _isProne))
        {
            Vector3 velocityChange = new Vector3(_playerPlanarVelocity.x, 0, _playerPlanarVelocity.z) * _moveDeceleration * Time.deltaTime;
            if (_playerPlanarVelocity.sqrMagnitude - velocityChange.sqrMagnitude > 0) //If deceleration won't make it go in opposite direction
            {
                _playerPlanarVelocity -= velocityChange;
            }
            else
            {
                _playerPlanarVelocity = Vector3.zero;
            }
        }
    }

    private void SetVerticalVelocity()
    {
        if (CanJump() && IsJumping()) //on jump
        {
            _playerVerticalVelocity += Vector3.up * _jumpVelocity;
        }
        else if (IsGrounded() == false) // if falling
        {
            _playerVerticalVelocity += Vector3.down * Time.deltaTime * _gravitationalAcceleration;
        }
        else // if grounded, apply consistent force downwards
        {
            _playerVerticalVelocity = _groundedDownwardVelocity * Vector3.down;
        }
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
    }

    private bool CanJump()
    {
        return _characterController.isGrounded && _playerVerticalVelocity.y <= 0 && !_isProne && !_isShootDodging;
    }

    private bool IsJumping()
    {
        return Input.GetKey(KeyCode.Space);
    }

    //Slerp between two angles
    Quaternion RotateAngle(Quaternion initialAngle, Quaternion targetAngle, float rotationSpeed)
    {
        return Quaternion.Slerp(initialAngle, targetAngle, rotationSpeed);
    }
}
