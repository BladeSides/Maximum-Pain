using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painkiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().AddPainKiller();
            Destroy(this.gameObject);
        }
    }
}
