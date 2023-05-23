using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GunText : MonoBehaviour
{
    private PlayerGunManager _playerGunManager;
    private TextMeshProUGUI _text;
    [SerializeField] private Sprite _pistolTexture;
    [SerializeField] private Sprite _shotgunTexture;
    [SerializeField] private Sprite _smgTexture;
    [SerializeField] private Image _gunImage;
    [SerializeField] private Image _gunDualImage;

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
        if (_playerGunManager.RightGun._typeOfGun == Gun.TypeOfGun.Pistol)
        {
            _gunImage.sprite = _pistolTexture;
            _gunDualImage.sprite = _pistolTexture;
        }
        else if (_playerGunManager.RightGun._typeOfGun == Gun.TypeOfGun.SMG)
        {
            _gunImage.sprite = _smgTexture;
            _gunDualImage.sprite = _smgTexture;
        }
        else
        {
            _gunImage.sprite = _shotgunTexture;
            _gunDualImage.sprite = null;
        }

        if (_playerGunManager.IsDualWielding)
        {
            _gunDualImage.enabled = true;

            _text.SetText(_playerGunManager.RightGun._typeOfGun.ToString()
            + "\n" + _playerGunManager.RightGun.ammoInGun + "/" +
            _playerGunManager.RightGun.ammoInReserve + "\n" +
            + _playerGunManager.LeftGun.ammoInGun + "/" +
            _playerGunManager.LeftGun.ammoInReserve);

        }
        else
        {
            _gunDualImage.enabled = false;

            _text.SetText(_playerGunManager.RightGun._typeOfGun.ToString() 
                + "\n" + _playerGunManager.RightGun.ammoInGun + "/" +
                _playerGunManager.RightGun.ammoInReserve);
        }
    }
}
