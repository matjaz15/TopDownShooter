using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID
{
    Rifle,
    Pistol,
    Shotgun,
    RifleAmounition,
    PistolAmounition,
    ShotgunAmounition,
    HealthPickup
}

public class Item : MonoBehaviour
{
    

    public ItemID itemID;
    public AudioClip pickupSound;
    public int giveAmount;

    public static event System.Action<Item> OnItemPickup;


    [Header("Glowing Light")]
    public Light glowingLight;
    public float glowSpeed=1;
    public float glowFactor=1;
    public float glowOffset=0;

    private void Start()
    {
        if (glowingLight != null)
            StartCoroutine(GlowAnimation());
    }

    IEnumerator GlowAnimation() {

        float percent = 0;
        float initialPercent = 0;
        bool direction = true;
        float initialIntensity = glowingLight.intensity;
        float randomOffsetTime = Random.Range(.3f,3);
        glowingLight.intensity = 0;

        while (initialPercent < 1)
        {
            initialPercent += Time.deltaTime * randomOffsetTime;
            Mathf.Lerp(glowingLight.intensity, initialIntensity, initialPercent);
            yield return null;
        }

        while (true){
            if (direction){
                percent += Time.deltaTime * glowSpeed;
                if (percent >= 1){
                    direction = false;
                    percent = 1;
                }
            }
            else{
                percent -= Time.deltaTime * glowSpeed;                
                if (percent <= 0){
                    direction = true;
                    percent = 0;
                }
            }
            glowingLight.intensity = (((2 * percent) * initialIntensity) / glowFactor) + glowOffset;
            yield return null;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (OnItemPickup != null) OnItemPickup(this);
        }
    }

    public void PickupSuccesfull() {
        AudioManager.instance.PlaySound(pickupSound, transform.position);
        Destroy(gameObject);
    }


}
