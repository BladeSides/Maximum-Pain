using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class EnemyRagdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    // Start is called before the first frame update

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_rigidBody.isKinematic == false)
        {
            //Destroy when out of camera maybe
        }
    }
}
