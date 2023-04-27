using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float moveSpeed = 5f;
    public float jumpVelocity = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime);
        characterController.Move(Physics.gravity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterController.Move(Vector3.up * jumpVelocity);
        }
    }
}
