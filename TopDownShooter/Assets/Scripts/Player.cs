using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : Entity {

    public float moveSPeed = 5;
    public Croshairs croshairs;
    public Gun startingGun;
    public MeleeWeapon startingMeleeWeapon;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;
    MeleeController meleeController;
    PlayerInventoryController inventoryController;


    protected override void Start () {
        base.Start();        
    }

    private void Awake(){
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        meleeController = GetComponent<MeleeController>();
        inventoryController = GetComponent<PlayerInventoryController>();

        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;

        if (startingGun != null)
            gunController.AddGun(startingGun);
        else
            gunController.AddGun(0);

        meleeController.EquippedMeleeWeapon(startingMeleeWeapon);
    }

    void OnNewWave(int waveNumber) {
        health = startingHealth;       
    }

    public void AddHealth(int amount) {
        health += amount;
        if (health >= startingHealth) health = startingHealth;
    }

	void Update () {
        //Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSPeed;
        controller.Move(moveVelocity);

        //Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up,Vector3.up * gunController.GunHeight);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            croshairs.transform.position = point;
            croshairs.DetectTargets(ray);
            
            //Amiming fix
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1.5) {
                gunController.Aim(point);
            }
            
        }

        //Weapon input
        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0)){
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            gunController.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            meleeController.Attack();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            gunController.EquipNextGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            gunController.EquipPreviousGun();
        }

        //Fall death
        if (transform.position.y < -10) {
            TakeDamage(health);
        }

    }

    public override void Die()
    {
        //AudioManager.instance.PlaySound("PlayerDeath", transform.position);
        base.Die();
    }

}
