using System;
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

    [SerializeField] private Transform _rightHostler;
    [SerializeField] private Transform _leftHostler;
    [SerializeField] private Gun _rightGun;
    [SerializeField] private Gun _leftGun;

    [SerializeField] private List<Gun> _guns;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _health = 100f;
    [SerializeField] private float _maxHealth = 100f;

    [SerializeField] private Gun.TypeOfGun _currentGunType;
    [SerializeField] private bool _dualWielding;

    [SerializeField] private Gun[] _gunPrefabs;

    [SerializeField] private Gun.TypeOfGun[] _gunOrder = { Gun.TypeOfGun.Pistol, Gun.TypeOfGun.SMG, Gun.TypeOfGun.Shotgun };

    [SerializeField] private bool _isSlowMotion = false;

    private Vector2 _Input = Vector2.zero;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        _gravitationalAcceleration = Physics.gravity.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _health = _maxHealth;

        Gun gun = Instantiate(_gunPrefabs[0]);
        _guns = new List<Gun>();
        _currentGunType = gun._typeOfGun;
        _guns.Add(gun);
        _rightGun = gun;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            int _nextGunIndex = 0;
            bool changedGun = false;
            foreach (Gun.TypeOfGun gunType in _gunOrder)
            {
                if (gunType == _currentGunType)
                {
                    _nextGunIndex = (int)gunType + 1;
                    if (_nextGunIndex > _gunOrder.Length - 1)
                    {
                        _nextGunIndex = 0;
                    }
                    while (_nextGunIndex != (int)_currentGunType)
                    {
                        foreach (Gun gun in _guns)
                        {
                            if (_nextGunIndex == (int)gun._typeOfGun)
                            {
                                _nextGunIndex = (int)gun._typeOfGun;
                                changedGun = true;
                            }
                        }
                        if (changedGun == false)
                        {
                            _nextGunIndex++;
                            if (_nextGunIndex > _gunOrder.Length - 1)
                            {
                                _nextGunIndex = 0;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            foreach (Gun gun in _guns)
            {
                print("Testing guns");
                if ((int)gun._typeOfGun == _nextGunIndex)
                {
                    print(_nextGunIndex);
                    _rightGun = gun;
                    _dualWielding = false;
                    _leftGun = null;
                    _currentGunType = gun._typeOfGun;
                    changedGun = true;
                    print("Changed Gun");
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isSlowMotion = !_isSlowMotion;
        }

        if (_isSlowMotion)
        {
            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = 0.02f * 0.25f;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
        SetInput();

        SetPlanarVelocity(); //As in, not jumping on the Y-Axis

        SetVerticalVelocity();

        SetGuns();
        DeActivateGuns();
    }

    private void DeActivateGuns()
    {
        foreach (Gun gun in _guns)
        {
            if (!_dualWielding)
            {
                if (gun != _rightGun)
                {
                    gun.gameObject.SetActive(false);
                }
            }
            else
            {
                if (gun != _rightGun && gun != _leftGun)
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetGuns()
    {
        _rightGun.transform.localPosition = Vector3.zero;
        _rightGun.transform.SetParent(_rightHostler);
        _rightGun.transform.localRotation = Quaternion.Euler(_mainCamera.transform.rotation.eulerAngles.x, 0, 0);
        _rightGun.gameObject.SetActive(true);
        if (_dualWielding)
        {
            _leftGun.transform.SetParent(_leftHostler);
            _leftGun.transform.localPosition = Vector3.zero;
            _leftGun.transform.localRotation = Quaternion.Euler(_mainCamera.transform.rotation.eulerAngles.x, 0, 0);
            _leftGun.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            _rightGun.Shoot(_rightGun.transform.position, "Player");
            if (_dualWielding)
            {
                _leftGun.Shoot(_leftGun.transform.position, "Player");
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int gunAmount = 0;
            Gun.TypeOfGun _guntype = _currentGunType;
            bool setRightGun = false;
            Gun leftGun = null, rightGun = null;
            foreach (Gun gun in _guns)
            {
                if (gun._typeOfGun == _guntype)
                {
                    if (rightGun == null && setRightGun == false)
                    {
                        rightGun = gun;
                        setRightGun = true;
                        Debug.Log("Set Right Gun"); 
                    }
                    else if (setRightGun == true)
                    {
                        leftGun = gun;
                        Debug.LogWarning("Set Left Gun");
                    }
                    gunAmount++;
                }

                _rightGun = rightGun;
                _leftGun = leftGun;
            }
            if (gunAmount >= 2)
            {
                _dualWielding = true;
            }
        }
    }

    private void SetVerticalVelocity()
    {
        if (CanJump())
        {
            _playerVelocity += Vector3.up * _jumpVelocity;
        }
        else if (IsGrounded() == false)
        {
            _playerVelocity += Vector3.down * Time.deltaTime * _gravitationalAcceleration;
        }
        else
        {
            _playerVelocity = new Vector3(_playerVelocity.x, -_groundedDownwardVelocity, _playerVelocity.z);
        }
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
    }

    private bool CanJump()
    {
        return Input.GetKey(KeyCode.Space) && _characterController.isGrounded && _playerVelocity.y <= 0;
    }

    private void SetPlanarVelocity()
    {
        _playerVelocity += new Vector3(_Input.x, 0, _Input.y) * _moveAcceleration * Time.deltaTime;
        if (_playerVelocity.sqrMagnitude > _maxVelocity * _maxVelocity)
        {
            _playerVelocity = _playerVelocity.normalized * _maxVelocity;
        }

        if (_Input.x == 0 && _Input.y == 0)
        {
            Vector3 velocityChange = new Vector3(_playerVelocity.x, 0, _playerVelocity.z) * _moveDeceleration * Time.deltaTime;
            if (_playerVelocity.sqrMagnitude - velocityChange.sqrMagnitude > 0) //If deceleration won't make it go in opposite direction
            {
                _playerVelocity -= velocityChange;
            }
            else
            {
                _playerVelocity = new Vector3(0, _playerVelocity.y, 0);
            }
        }
    }

    private void SetInput()
    {
        _Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rightGun.isReloading = true;
            _rightGun.timer = 0;
            if (_dualWielding)
            {
                _leftGun.isReloading = true;
                _leftGun.timer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 _movement = Vector3.zero;
        _movement += transform.right * _playerVelocity.x + transform.up * _playerVelocity.y + transform.forward * _playerVelocity.z;
        _characterController.Move(_movement * Time.deltaTime);
    }

    public void Damage(float damageAmount)
    {
        _health -= damageAmount;
    }

    public void AddGun(Gun gunToAdd)
    {
        int maxGunCount = gunToAdd.dualWieldable ? 1 : 2;
        int gunCount = 0;
        Gun gunToAddAmmoTo = null;
        foreach (Gun gun in _guns)
        {
            if (gun._typeOfGun == gunToAdd._typeOfGun)
            {
                gunCount++;
            }
        }
        if (gunCount > maxGunCount)
        {
            if (gunToAddAmmoTo != null)
            {
                gunToAddAmmoTo.ammoInReserve += gunToAdd.clipSize;
                Destroy(gunToAdd.gameObject);
            }
        }
        else
        {
            _guns.Add(gunToAdd);
            gunToAdd.isHeld = true;
        }
    }
}
