using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]

public class HealthText : MonoBehaviour
{
    private PlayerManager _playerManager;
    private TextMeshProUGUI _text;
    // Start is called before the first frame update
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = "Health: " + (int)_playerManager.health + "\nBullet Time: " + (int)_playerManager.bulletTime;
    }
}
