using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour {

    public float maxRange = 3f;
    public float damage = 2;

    public AudioClip swingAudio;


    Player player;


    void CheckCollisions(){
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRange)){
            Collider[] hitObjectsInRadius = Physics.OverlapSphere(hit.point,3);

            float minDistance = 1000;
            Collider closestEnemy = hit.collider;
            foreach (Collider hitObject in hitObjectsInRadius){
                if (hitObject.CompareTag("Enemy")){

                    float distance = Vector3.Distance(player.transform.position, hitObject.transform.position);
                    if (distance < minDistance){
                        closestEnemy = hitObject;
                        minDistance = distance;
                    }
                }
            }
            OnHitObject(closestEnemy, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint){
        IDamagable damagableObject = c.GetComponent<IDamagable>();
        if (damagableObject != null){
            damagableObject.TakeHit(damage, hitPoint, player.transform.forward);
        }
    }

    public void Attack() {
        player = FindObjectOfType<Player>();

        CheckCollisions();
        AudioManager.instance.PlaySound(swingAudio,transform.position);
    }
}
