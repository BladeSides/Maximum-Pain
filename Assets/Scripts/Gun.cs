using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject Owner;
    public enum typeOfGun 
    {
        Pistol,
        SMG
    }
    public bool dualWieldable = false;
    public bool isEquipped = false;
    public bool isReloading = false;
    public int clipSize;
    public int ammoInReserve;
    public int ammoInGun;
    public float fireRate;
    public float reloadTime;
    public float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
    }

    public void Shoot(Vector3 Origin, string Owner)
    {
        if (Owner == "Player")
        {
            if (timer > fireRate)
            {
                timer = 0;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f)) - Origin, out RaycastHit hit, Camera.main.farClipPlane))
                {
                    Debug.DrawLine(Origin, hit.point, Color.red, 0.01f);
                }
            }
        }
    }
}
