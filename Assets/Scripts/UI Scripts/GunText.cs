using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GunText : MonoBehaviour
{
    private PlayerController _playerController;
    private TextMeshProUGUI _text;
    // Start is called before the first frame update
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        if (_playerController == null)
        {
            _playerController = FindAnyObjectByType<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.IsDualWielding)
        {
            _text.SetText(_playerController.RightGun._typeOfGun.ToString()
            + "\nAmmo: " + _playerController.RightGun.ammoInGun + "/" +
            _playerController.RightGun.ammoInReserve + "\n" + 
            _playerController.LeftGun._typeOfGun.ToString()
            + "\nAmmo: " + _playerController.LeftGun.ammoInGun + "/" +
            _playerController.LeftGun.ammoInReserve);

        }
        else
        {
            _text.SetText(_playerController.RightGun._typeOfGun.ToString() 
                + "\nAmmo: " + _playerController.RightGun.ammoInGun + "/" + 
                _playerController.RightGun.ammoInReserve);
        }
    }
}
