using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour {

    GunController gunController;
    Player player;

    bool pickupSuccessfull;

    private void Start(){
        gunController = FindObjectOfType<GunController>();
        player = FindObjectOfType<Player>();

        Item.OnItemPickup += PickupItem;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        Item.OnItemPickup -= PickupItem;
    }


    void SetAmounition(Item toItem, ItemID id) {
        if (gunController.ContainsGunInInventory(id))
            gunController.AddMagazine(toItem.giveAmount, id);
        else pickupSuccessfull = false;
    }


    public void PickupItem(Item itm)
    {
        pickupSuccessfull = true;

        switch (itm.itemID)
        {
            case ItemID.Rifle:
                gunController.AddGun(itm.itemID);
                break;
            case ItemID.Pistol:
                gunController.AddGun(itm.itemID);
                break;
            case ItemID.Shotgun:
                gunController.AddGun(itm.itemID);
                break;
            case ItemID.RifleAmounition:
                SetAmounition(itm, ItemID.Rifle);
                break;
            case ItemID.PistolAmounition:
                SetAmounition(itm, ItemID.Pistol);
                break;
            case ItemID.ShotgunAmounition:
                SetAmounition(itm, ItemID.Shotgun);
                break;
            case ItemID.HealthPickup:
                player.AddHealth(itm.giveAmount);
                break;
            default:
                break;
        }

        if (pickupSuccessfull)
            itm.PickupSuccesfull();
    }
}
