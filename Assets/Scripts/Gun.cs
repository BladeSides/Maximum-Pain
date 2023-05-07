using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour
{
    public GameObject Owner;
    public enum TypeOfGun 
    {
        Pistol,
        SMG,
        Shotgun
    }

    public TypeOfGun _typeOfGun;
    public bool dualWieldable = false;
    public bool isHeld = false;
    public bool isReloading = false;
    public int clipSize;
    public int ammoInReserve;
    public int ammoInGun;
    public float fireRate;
    public float reloadTime;
    public float timer;
    public float damage;
    public GameObject _player;
    private Rigidbody _rigidBody;
    public ParticleSystem _muzzleFlash;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _muzzleFlash.Pause(); 
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        ReloadGun();
    }

    private void FixedUpdate()
    {
        if (isHeld)
        {
            _rigidBody.useGravity = false;
        }
        else
        {
            _rigidBody.useGravity = true;
        }
    }

    public void Shoot(Vector3 Origin, string Owner)
    {
        if (CanShoot())
        {
            if (Owner == "Player")
            {
                if (timer > fireRate)
                {
                    timer = 0;
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f)) - Origin, out RaycastHit hit, Camera.main.farClipPlane))
                    {
                        Debug.DrawLine(Origin, hit.point, Color.red, 0.5f);
                        if (hit.transform.gameObject.tag == "Enemy")
                        {
                            EnemyController ec = hit.transform.gameObject.GetComponent<EnemyController>();
                            ec.Damage(damage);
                            Instantiate(ec._bloodParticles, hit.point, Quaternion.identity);
                        }
                    }
                    ammoInGun--;
                    _muzzleFlash.Play();
                }
            }
            if (Owner == "Enemy")
            {
                if (timer > fireRate)
                {
                    timer = 0;
                    if (Physics.Raycast(Origin, (_player.transform.position - Origin).normalized, out RaycastHit hit))
                    {
                        Debug.DrawLine(Origin, hit.point, Color.green, 0.5f);
                        if (hit.transform.root.tag == "Player")
                        {
                            if (hit.transform.gameObject.TryGetComponent<PlayerController>(out PlayerController pc))
                            {
                                pc.Damage(damage);
                                Instantiate(pc._bloodParticleSystem, hit.point, Quaternion.identity);
                            }
                        }
                    }
                    ammoInGun--;
                    _muzzleFlash.Play();
                }
            }

            if (ammoInGun <= 0)
            {
                if (Owner == "Enemy")
                {
                    ammoInReserve += clipSize; //Give enemy infinite ammo
                }
                isReloading = true;
                timer = 0;
            }
        }
    }

    public void ReloadGun()
    {
        if (!isReloading)
            return;

        if (ammoInGun >= clipSize || ammoInReserve == 0)
        {
            return;
        }
        if (timer > reloadTime)
        {
            int ammoToAdd = clipSize - ammoInGun;
            if (ammoInReserve > clipSize)
            {
                ammoInReserve -= ammoToAdd;
                ammoInGun += ammoToAdd;
            }
            else
            {
                ammoInGun = ammoInReserve;
                ammoInReserve = 0;
            }
            isReloading = false;
        }
    }

    private bool CanShoot()
    {
        return (ammoInGun > 0 && !isReloading);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHeld)
        {
            if (other.transform.root.tag == "Player")
            {
                _player.gameObject.GetComponent<PlayerController>().AddGun(this);
            }
        }
    }
}
