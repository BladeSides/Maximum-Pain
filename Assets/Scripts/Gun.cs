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
    public GameObject _bulletDecal;
    public LineRenderer _bulletTrail;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _muzzleFlash.Pause();
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
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
        if (isReloading && ammoInGun >= clipSize)
        {
            isReloading = false; //safety check
        }
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
                            if (hit.transform.gameObject.TryGetComponent<EnemyController>(out EnemyController ec))
                            {
                                ec.Damage(damage);
                                Instantiate(ec._bloodParticles, hit.point, Quaternion.identity);
                            }
                        }
                        else if (hit.collider.isTrigger == false) //Prevents decals from spawning at the the trigger boxes for doors
                        {
                            Vector3 direction = (hit.point - Origin).normalized;
                            Instantiate(_bulletDecal, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                        }
                        LineRenderer lr = Instantiate(_bulletTrail, Vector3.zero, Quaternion.identity);
                        lr.SetPosition(0, this.transform.position);
                        lr.SetPosition(1, hit.point);
                    }
                    else
                    {
                        LineRenderer lr = Instantiate(_bulletTrail, Vector3.zero, Quaternion.identity);
                        lr.SetPosition(0, this.transform.position);
                        lr.SetPosition(1, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f)));
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
                    //Make less likely to hit player when moving
                    if (Physics.Raycast(Origin, (_player.transform.position - Origin).normalized - UnityEngine.Random.insideUnitSphere * 
                        (_player.GetComponent<PlayerMovement>().movement.magnitude/5 + 0.25f), out RaycastHit hit))
                    {
                        Debug.DrawLine(Origin, hit.point, Color.green, 0.5f);
                        if (hit.transform.root.tag == "Player")
                        {
                            if (hit.transform.gameObject.TryGetComponent<PlayerManager>(out PlayerManager pm))
                            {
                                pm.Damage(damage);
                                Instantiate(pm.bloodParticleSystem, hit.point, Quaternion.identity);
                            }
                        }
                        else if (hit.collider.isTrigger == false && hit.transform.root.tag != "Enemy")
                        {
                            Vector3 direction = (hit.point - Origin).normalized;
                            Instantiate(_bulletDecal, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                        }
                        LineRenderer lr = Instantiate(_bulletTrail, Vector3.zero, Quaternion.identity);
                        lr.SetPosition(0, this.transform.position);
                        lr.SetPosition(1, hit.point);
                    }
                    else
                    {
                        LineRenderer lr = Instantiate(_bulletTrail, Vector3.zero, Quaternion.identity);
                        lr.SetPosition(0, this.transform.position);
                        lr.SetPosition(1, (_player.transform.position - Origin).normalized - UnityEngine.Random.onUnitSphere * 5);
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
                _player.gameObject.GetComponent<PlayerGunManager>().AddGun(this);
            }
        }
    }
}
