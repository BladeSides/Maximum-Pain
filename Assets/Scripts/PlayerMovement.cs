using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _gravitationalAcceleration;

    // Start is called before the first frame update
    void Start()
    {
        _gravitationalAcceleration = Physics.gravity.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
