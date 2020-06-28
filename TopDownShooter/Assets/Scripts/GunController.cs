using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [System.Serializable]
    public class GunDetails {
        public Gun gun;
        public Transform gunIcon;
    }

    public Transform weponHold;
    public GunDetails[] allGuns;
    
    public Gun equippedGun { get; private set; }

    public event System.Action OnGunChange;

    List<Gun> inventoryGuns;
    int currentGunIndex = 0;


    public void AddGun(Gun gunToAdd)
    {
        if (inventoryGuns == null)
            inventoryGuns = new List<Gun>();


        bool exist = false;
        foreach (Gun gun in inventoryGuns)
        {
            if (gun.itemID == gunToAdd.itemID)
            {
                AddMagazine(gun.startingMagAmount / 2, gun);
                exist = true;
            }
        }
        if (!exist)
        {
            equippedGun = Instantiate(gunToAdd, weponHold.position, weponHold.rotation, weponHold) as Gun;
            equippedGun.isInInventory = true;
            EquipGun(equippedGun);
            inventoryGuns.Add(equippedGun);
        }
        if (OnGunChange != null) OnGunChange();
    }

    public void AddGun(ItemID itemID){
        AddGun(GetGunByItemId(itemID));
    }

    public void EquipGun(Gun gunToEquip) {
        foreach (Gun gun in inventoryGuns)
        {
            if (gun.itemID == gunToEquip.itemID){
                equippedGun = gun;
                gun.gameObject.SetActive(true);
                if (OnGunChange != null) OnGunChange();
            }
            else {
                gun.gameObject.SetActive(false);
            }
        }
    }

    public void EquipGun(ItemID itemID) {
        EquipGun(GetGunByItemId(itemID));
    }

    public void EquipGun(int weaponIndex){
        EquipGun(inventoryGuns[weaponIndex]);
    }

    public void EquipNextGun() {
        currentGunIndex++;
        if (currentGunIndex >= inventoryGuns.Count) currentGunIndex = 0;
        EquipGun(currentGunIndex);
    }

    public void EquipPreviousGun() {
        currentGunIndex--;
        if (currentGunIndex < 0) currentGunIndex = inventoryGuns.Count-1;
        EquipGun(currentGunIndex);
    }

    public Gun GetEquippedGun() {
        return equippedGun;
    }

    public bool ContainsGunInInventory(ItemID gunItemID) {
        foreach (Gun gun in inventoryGuns)
        {
            if (gun.itemID == gunItemID) {
                return true;
            }
        }
        return false;
    }

    public Transform GetEquippedGunIcon() {

        for (int i = 0; i < allGuns.Length; i++) {
            if (equippedGun.itemID == allGuns[i].gun.itemID) {
                return allGuns[i].gunIcon.transform;
            }
        }
        return null;
    }

    public Gun GetGunByItemId(ItemID itemId)
    {
        for (int i = 0; i < allGuns.Length; i++){
            if (allGuns[i].gun.itemID == itemId) return allGuns[i].gun;
        }
        return null;
    }

    public void AddMagazine(int amount) {
        if (equippedGun != null){
            equippedGun.AddMagazine(amount);
        }
    }

    public void AddMagazine(int amount, Gun gun) {
        if (gun != null) {
            gun.AddMagazine(amount);
        }
    }

    public void AddMagazine(int amount, ItemID itemID) {
        foreach (Gun gun in inventoryGuns){
            if (gun.itemID == itemID) {
                AddMagazine(amount,gun);
            }
        }
    }

    public void OnTriggerHold() {
        if (equippedGun != null) {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease() {
        if (equippedGun != null){
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight {
        get{
            return weponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint) {
        if (equippedGun != null){
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload() {
        if (equippedGun != null){
            equippedGun.Reload();
        }
    }
}
