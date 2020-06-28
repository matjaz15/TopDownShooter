using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float followSpeed = 10;

    Transform player;
    float offsetZ;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offsetZ = transform.position.z;

    }
	
	// Update is called once per frame
	void Update () {
        if (player != null) {

            Vector3 newPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z + offsetZ);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followSpeed);

        }
    }
}
