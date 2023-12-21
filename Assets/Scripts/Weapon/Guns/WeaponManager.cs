using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
        public static WeaponManager Instance { get; set; }

        public List<GameObject> weaponSlots;

        public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]
    public float throwForce = 10f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject dynamitePrefab;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject spoiledDynamitePrefab;

    private void Awake()
        {
        Instance = this;
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

    public void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots) 
        { 
          if(weaponSlot == activeWeaponSlot) 
            { 
              weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if(Input.GetKey(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;

            if(forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if(Input.GetKeyUp(KeyCode.G))
        {
            if(lethalsCount > 0)
            {
                ThrowLethal();
            }
            forceMultiplier = 0;
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }
            forceMultiplier = 0;
        }

    }

   

    public void PickUpWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }
   

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x,weapon.spawnPosition.y,weapon.spawnPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if(activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;


            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;


        }
    }
    #region || ---- Weapon ---- ||
    public void SwitchActiveSlot(int slotNumber)
    {
        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }
    #endregion

    #region || ---- Ammo ---- ||
    internal void PickUpAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
            break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
            break;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
       switch(thisWeaponModel)
        {
            case Weapon.WeaponModel.Desert_Eagle:
                totalPistolAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Deadeye:
                totalRifleAmmo -= bulletsToDecrease;
                break;


        }
    }

    
    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.Desert_Eagle:
                return totalPistolAmmo;

            case Weapon.WeaponModel.Deadeye:
                return totalRifleAmmo;

            default: return 0;
        }
    }
    #endregion

    #region || ---- Throwables ---- ||
    public void PickUpThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
           case Throwable.ThrowableType.Dynamite:
                PickupThrowableAsLethal(Throwable.ThrowableType.Dynamite);
                break;
            case Throwable.ThrowableType.Spoiled_Dynamite:
                PickupThrowableAsTactical(Throwable.ThrowableType.Spoiled_Dynamite);
                break;
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalsCount < maxTacticals)
            {
                tacticalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Tactical limit reached");
            }
        }
        else
        {

        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if(equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if(lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals limit reached");
            }
        }
        else
        {

        }
    }

 

    private void ThrowLethal()
    {
       GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount -= 1;

        if(lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount -= 1;

        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }
    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Dynamite:
                return dynamitePrefab;
            case Throwable.ThrowableType.Spoiled_Dynamite:
                return spoiledDynamitePrefab;
        }
        return new();

    }
    #endregion
}
