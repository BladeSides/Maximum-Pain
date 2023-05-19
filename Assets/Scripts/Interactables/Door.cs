using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class Door : MonoBehaviour
{
    [SerializeField] private bool isInteractable;
    private HingeJoint joint;

    private void Awake()
    {
        if (joint == null)
        {
            joint = GetComponent<HingeJoint>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            joint.useMotor = true;
            joint.useSpring = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            joint.useMotor = false;
            joint.useSpring = true;
        }
    }
}