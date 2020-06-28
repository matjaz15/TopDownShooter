using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    public Color trailColor;

    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = 0.1f;


    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position,.3f);
        if (initialCollisions.Length > 0) {
            foreach (Collider col in initialCollisions){
                if (col.GetComponent<BoxCollider>() && col.gameObject.layer != 11){
                    if (!col.GetComponent<BoxCollider>().isTrigger) {
                        Destroy(gameObject);
                    }                                            
                }
            }            
        }
        LayerHandler(initialCollisions[0], transform.position);

        GetComponent<TrailRenderer>().material.SetColor("_TintColor",trailColor);
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

	// Update is called once per frame
	void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    void CheckCollisions(float moveDistance) {
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
        //    OnHitObject(hit.collider, hit.point);
        //}

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth))
        {
            LayerHandler(hit.collider,hit.point);
        }
    }

    void LayerHandler(Collider c, Vector3 pos) {
        if (c.gameObject.layer == 8) //enemy
        {
            OnHitObject(c, pos);
        }
        else if (c.gameObject.layer == 10) //obstace
        {
            GameObject.Destroy(gameObject);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint) {
        IDamagable damagableObject = c.GetComponent<IDamagable>();
        if (damagableObject != null){
            damagableObject.TakeHit(damage, hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
