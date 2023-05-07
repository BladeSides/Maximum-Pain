using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private GameObject _mesh;

    [SerializeField] public bool _isShootDodging;
    [SerializeField] public bool _isProne;

    [SerializeField] private float _standupTimer;

    [SerializeField] private float _maxVelocity = 1f;
    [SerializeField] private float _moveAcceleration = 1f;
    [SerializeField] private float _moveDeceleration = 5f;

    [SerializeField] private float _groundedDownwardVelocity = 0.1f;

    [SerializeField] private Vector3 _playerPlanarVelocity;
    [SerializeField] private Vector3 _playerVerticalVelocity;

    [SerializeField] private float _gravitationalAcceleration;
    [SerializeField] private float _jumpVelocity = 10f;

    [SerializeField] private float _characterControllerDefaultHeight;

    [SerializeField] private Transform _rightHostler;
    [SerializeField] private Transform _leftHostler;
    [SerializeField] public Gun RightGun;
    [SerializeField] public Gun LeftGun;

    [SerializeField] private List<Gun> _guns;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _health = 100f;
    [SerializeField] private float _maxHealth = 100f;

    [SerializeField] private Gun.TypeOfGun _currentGunType;
    [SerializeField] public bool IsDualWielding;

    [SerializeField] private Gun[] _gunPrefabs;

    [SerializeField] private Gun.TypeOfGun[] _gunOrder = { Gun.TypeOfGun.Pistol, Gun.TypeOfGun.SMG, Gun.TypeOfGun.Shotgun };

    [SerializeField] private bool _isSlowMotion = false;
    public ParticleSystem _bloodParticleSystem;

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
        RightGun = gun;
        _characterControllerDefaultHeight = _characterController.height;
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
                    RightGun = gun;
                    IsDualWielding = false;
                    LeftGun = null;
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


        if (Input.GetKeyDown(KeyCode.C) && CanJump())
        {
            _playerPlanarVelocity = new Vector3(_Input.x, 0, _Input.y);
            _playerVerticalVelocity = new Vector3(0, _jumpVelocity, 0);
            _characterController.Move(_jumpVelocity * Vector3.up * Time.deltaTime);
            Time.timeScale = 0.1f;
            _characterController.height = _characterControllerDefaultHeight / 2;
            _isShootDodging = true;
            _playerPlanarVelocity = _playerPlanarVelocity.normalized * _maxVelocity;
        }
        if (_isShootDodging)
        {
            if (_playerPlanarVelocity.sqrMagnitude >= 0.25) //Ofset
            {
                _mesh.transform.localRotation = Quaternion.Euler(
                    new Vector3(_playerPlanarVelocity.z, 0, -_playerPlanarVelocity.x).normalized * 90);
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

        if (!_isShootDodging && !_isProne)
        {
            _mesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    private void DeActivateGuns()
    {
        foreach (Gun gun in _guns)
        {
            if (!IsDualWielding)
            {
                if (gun != RightGun)
                {
                    gun.gameObject.SetActive(false);
                }
            }
            else
            {
                if (gun != RightGun && gun != LeftGun)
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetGuns()
    {
        DistributeAmmo();
        RightGun.transform.localPosition = Vector3.zero;
        RightGun.transform.SetParent(_rightHostler);
        RightGun.transform.LookAt(this.transform.position + _mainCamera.transform.forward * 5 + _mainCamera.transform.right);
        RightGun.gameObject.SetActive(true);
        if (IsDualWielding)
        {
            LeftGun.transform.SetParent(_leftHostler);
            LeftGun.transform.localPosition = Vector3.zero;
            LeftGun.transform.LookAt(this.transform.position + _mainCamera.transform.forward * 5 + _mainCamera.transform.right);
            LeftGun.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DualWield();
        }
    }

    private void DualWield()
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

            RightGun = rightGun;
            LeftGun = leftGun;
        }
        if (gunAmount >= 2)
        {
            IsDualWielding = true;
        }
    }

    private void Shoot()
    {
        RightGun.Shoot(this.transform.position, "Player");
        if (IsDualWielding)
        {
            LeftGun.Shoot(this.transform.position, "Player");
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

    private void SetInput()
    {
        _Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (IsDualWielding)
            {
                DistributeAmmo();
                LeftGun.isReloading = true;
                LeftGun.timer = 0;
            }
            RightGun.isReloading = true;
            RightGun.timer = 0;
        }
    }

    private void DistributeAmmo()
    {
        int totalAmmo = 0;
        int totalGuns = 0;
        foreach (Gun gun in _guns)
        {
            if (gun._typeOfGun == _currentGunType)
            {
                totalAmmo += gun.ammoInReserve;
                totalGuns++;
            }
        }
        foreach (Gun gun in _guns)
        {
            if (gun._typeOfGun == _currentGunType)
            {
                gun.ammoInReserve = totalAmmo / totalGuns;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 _movement = Vector3.zero;
        _movement += transform.right * _playerPlanarVelocity.x + transform.up * _playerVerticalVelocity.y + transform.forward * _playerPlanarVelocity.z;
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
