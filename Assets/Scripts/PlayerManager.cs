using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float _health = 100f;
    [SerializeField] private float _maxHealth = 100f;

    //Slow Motion
    [SerializeField] private bool _isSlowMotion = false;
    [SerializeField] private float _defaultDeltaTime;

    //Blood Particles
    public ParticleSystem bloodParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _defaultDeltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _isSlowMotion = !_isSlowMotion;
        }
        if (_isSlowMotion)
        {
            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = _defaultDeltaTime * 0.25f;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultDeltaTime;
        }
    }
    public void Damage(float damageAmount)
    {
        _health -= damageAmount;
    }
}
