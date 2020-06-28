 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidBody;

	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
	}


    public void Move(Vector3 _velotity)
    {
        velocity = _velotity;
    }

    public void LookAt(Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x,transform.position.y,lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }


	void Update () {
        transform.position = Vector3.Lerp(myRigidBody.position, myRigidBody.position + velocity,Time.deltaTime);
	}
}
