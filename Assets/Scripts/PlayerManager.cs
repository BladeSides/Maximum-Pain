using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    //Health and bullet time
    public float health = 100f;
    [SerializeField] private float _maxHealth = 100f;
    public float bulletTime = 100f;
    [SerializeField] private float _maxBulletTime = 100f;

    public float painkillerCount = 0f;
    [SerializeField] private float _pkHealthRegenRate = 25f;
    [SerializeField] private float _timerPerPk = 1.5f;
    [SerializeField] private float _pkTimer = 0f;

    public bool isAlive = true;

    //Game Over Stuff
    [SerializeField] private float _deathTimer;
    [SerializeField] private float _timeToEnd = 3f;

    //Camera
    [SerializeField] private CameraController _cameraController;

    [SerializeField] private float _bulletTimeDegenRate = 10f;
    [SerializeField] private float _bulletTimePerHit = 10f;

    //Slow Motion
    [SerializeField] public bool isSlowMotion = false;
    [SerializeField] private float _defaultDeltaTime;

    //Blood Particles
    public ParticleSystem bloodParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        //Set Default Values
        health = _maxHealth;
        bulletTime = _maxBulletTime;
        _defaultDeltaTime = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f && isAlive)
        {
            KillPlayer();
            Time.timeScale = 0.25f;
            isAlive = false;
        }
        if (!isAlive)
        {
            EndGame();
            return;
        }
        PainKillers();
        BulletTime();
    }

    private void LateUpdate() //Make sure health is clamped after all updates have executed, so player doesn't
        // have negative health
    {
        health = Mathf.Clamp(health, 0, _maxHealth);
    }

    private void EndGame()
    {
        _deathTimer += Time.unscaledDeltaTime;
        if (_deathTimer > _timeToEnd)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void PainKillers()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (painkillerCount > 0 && health < _maxHealth)
            {
                painkillerCount -= 1;
                _pkTimer += _timerPerPk;
            }
        }

        if (_pkTimer > 0)
        {
            health += _pkHealthRegenRate * Time.deltaTime;
        }

        _pkTimer -= Time.deltaTime;

        //LimitValues
        health = Mathf.Clamp(health, 0, _maxHealth);
        _pkTimer = Mathf.Max(_pkTimer, 0);
    }
    private void KillPlayer()
    {
        _cameraController.isPlayerDead = true;
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = _defaultDeltaTime * 0.25f;
        GameManager.gameWon = false;
        _deathTimer = 0;
    }

    private void BulletTime()
    {
        if (bulletTime <= 0)
        {
            isSlowMotion = false;
        }
        if (isSlowMotion)
        {
            bulletTime -= _bulletTimeDegenRate * Time.unscaledDeltaTime;
            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = _defaultDeltaTime * 0.25f;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultDeltaTime;
        }
        bulletTime = Mathf.Clamp(bulletTime, 0, _maxBulletTime);
    }

    public void Damage(float damageAmount)
    {
        health -= damageAmount;
    }

    public void AddBulletTime()
    {
        bulletTime += _bulletTimePerHit;
        bulletTime = Mathf.Clamp(bulletTime, 0, _maxBulletTime);
    }

    public void AddPainKiller()
    {
        painkillerCount += 1;
    }
}
