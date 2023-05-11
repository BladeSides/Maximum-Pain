using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GunText : MonoBehaviour
{
    private PlayerGunManager _playerGunManager;
    private TextMeshProUGUI _text;
    // Start is called before the first frame update
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        if (_playerGunManager == null)
        {
            _playerGunManager = FindAnyObjectByType<PlayerGunManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerGunManager.IsDualWielding)
        {
            _text.SetText(_playerGunManager.RightGun._typeOfGun.ToString()
            + "\nAmmo: " + _playerGunManager.RightGun.ammoInGun + "/" +
            _playerGunManager.RightGun.ammoInReserve + "\n" +
            _playerGunManager.LeftGun._typeOfGun.ToString()
            + "\nAmmo: " + _playerGunManager.LeftGun.ammoInGun + "/" +
            _playerGunManager.LeftGun.ammoInReserve);

        }
        else
        {
            _text.SetText(_playerGunManager.RightGun._typeOfGun.ToString() 
                + "\nAmmo: " + _playerGunManager.RightGun.ammoInGun + "/" +
                _playerGunManager.RightGun.ammoInReserve);
        }
    }
}
