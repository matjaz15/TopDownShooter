using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    public Transform meleeHold;
    public Animator handAnimator;
    public float delayBetweenAttacks = 0.3f;

    MeleeWeapon equippedWeapon;

    bool allowAttack;


    private void Start()
    {
        allowAttack = true;
    }

    public void EquippedMeleeWeapon(MeleeWeapon meleeWeapon) {
        if (meleeWeapon != null) {
            equippedWeapon = Instantiate(meleeWeapon, meleeHold.position, meleeHold.rotation, meleeHold) as MeleeWeapon;
            equippedWeapon.gameObject.SetActive(false);
        }
    }

    public void Attack() {

        if (allowAttack) {
            equippedWeapon.gameObject.SetActive(true);
            StartCoroutine(WeaponSwingAnimation());
            equippedWeapon.Attack();
        }    
    }

    IEnumerator WeaponSwingAnimation() {
        allowAttack = false;        
        handAnimator.SetBool("isSwinging", true);
        yield return new WaitForSeconds(delayBetweenAttacks);
        handAnimator.SetBool("isSwinging", false);
        equippedWeapon.gameObject.SetActive(false);
        allowAttack = true;
    }


}
