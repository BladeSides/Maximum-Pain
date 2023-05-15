using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerGunManager : MonoBehaviour
{
    [SerializeField] private Transform _rightHostler;
    [SerializeField] private Transform _leftHostler;
    [SerializeField] public Gun RightGun;
    [SerializeField] public Gun LeftGun;

    [SerializeField] private PlayerManager _playerManager;

    [SerializeField] private List<Gun> _guns;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private Gun.TypeOfGun _currentGunType;

    [SerializeField] public bool IsDualWielding;

    [SerializeField] private Gun[] _gunPrefabs;

    [SerializeField] private Gun.TypeOfGun[] _gunOrder = { Gun.TypeOfGun.Pistol, Gun.TypeOfGun.SMG, Gun.TypeOfGun.Shotgun };

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        Gun gun = Instantiate(_gunPrefabs[0]);
        _guns = new List<Gun>();
        _currentGunType = gun._typeOfGun;
        _guns.Add(gun);
        RightGun = gun;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerManager.isAlive)
        {
            return;
        }
        SetInput();

        SetGuns();
        DeActivateGuns();
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

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                GetNextWeapon();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                GetPreviousWeapon();
            }
        }

    }

    private void GetPreviousWeapon()
    {
        if (IsDualWielding & LeftGun != null)
        {
            LeftGun = null;
            IsDualWielding = false;
            return;
        }

        int _nextGunIndex = 0;
        bool changedGun = false;
        foreach (Gun.TypeOfGun gunType in _gunOrder)
        {
            if (gunType == _currentGunType)
            {
                _nextGunIndex = (int)gunType - 1;
                if (_nextGunIndex < 0)
                {
                    _nextGunIndex = _gunOrder.Length - 1;
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
                        _nextGunIndex--;
                        if (_nextGunIndex < 0)
                        {
                            _nextGunIndex = _gunOrder.Length - 1;
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

    private void GetNextWeapon()
    {
        bool dualwieldSuccesful = false;
        if (!IsDualWielding & LeftGun == null && RightGun.dualWieldable)
        {
            dualwieldSuccesful = DualWield();
            if (dualwieldSuccesful)
            {
                return;
            }
        }

        if (dualwieldSuccesful == false)
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
    }

    private bool DualWield() //Tries to dual wield, returns true if succesful
    {
        if (RightGun.dualWieldable == false)
        {
            return false;
        }
        int gunAmount = 0;

        bool setRightGun = false;
        Gun leftGun = null, rightGun = null;

        foreach (Gun gun in _guns)
        {
            if (gun._typeOfGun == _currentGunType)
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

            if (rightGun != null)
            {
                RightGun = rightGun;
            }
            print(rightGun);
            LeftGun = leftGun;
            if (leftGun != null)
            {
                leftGun.timer = 0;
            }

        }
        if (gunAmount >= 2)
        {
            IsDualWielding = true;
            return true;
        }
        else
        {
            return false;
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


    private void SetInput()
    {
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

        if (IsDualWielding)
        {
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

        else if (RightGun.dualWieldable)
        {
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
                    gun.ammoInReserve = 0;
                }
            }
            RightGun.ammoInReserve = totalAmmo;
        }
    }

    public void AddGun(Gun gunToAdd)
    {
        gunToAdd.timer = 0;
        int maxGunCount = gunToAdd.dualWieldable ? 1 : 2;
        int gunCount = 0;
        Gun gunToAddAmmoTo = null;
        foreach (Gun gun in _guns)
        {
            if (gun._typeOfGun == gunToAdd._typeOfGun)
            {
                gunCount++;
                gunToAddAmmoTo = gun;
            }
        }
        if (gunCount > maxGunCount)
        {
            if (gunToAddAmmoTo != null)
            {
                gunToAddAmmoTo.ammoInReserve += gunToAdd.clipSize;
                Destroy(gunToAdd.gameObject);
                Debug.Log("Destroyed");
            }
        }
        else
        {
            _guns.Add(gunToAdd);
            gunToAdd.isHeld = true;
        }
    }
}
