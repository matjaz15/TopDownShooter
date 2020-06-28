using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPlayerInLevel : MonoBehaviour {

    public float timeBeforeHurt = 5f;
    public int hurtAmount = 1;
    public float hurtDelay = 1f;

    public float timer { get; private set; }

    public event System.Action OnLeaveLevel;
    public event System.Action OnEnterLevel;

    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OnEnterLevel != null) OnEnterLevel();
            StopCoroutine("DamageCounter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (OnLeaveLevel != null) OnLeaveLevel();
            StartCoroutine("DamageCounter");
        }
    }

    //Hurt the player
    IEnumerator DamageCounter()
    {
        IDamagable damagable = player.GetComponent<IDamagable>();

        float time = 0;
        timer = timeBeforeHurt;

        while (true)
        {
            if (time >= timeBeforeHurt)
            {
                damagable.TakeDamage(hurtAmount);
                timer = 0;
                yield return new WaitForSeconds(hurtDelay);
            }
            else {
                time += Time.deltaTime;
                timer = timeBeforeHurt - time;

            }
            yield return null;

        }
    }
}
