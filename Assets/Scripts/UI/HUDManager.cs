using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    private GameObject pistolWeaponSprite;
    private GameObject rifleWeaponSprite;
    private GameObject pistolAmmoSprite;
    private GameObject rifleAmmoSprite;
    private GameObject dynamiteSprite;
    private GameObject spoiledDynamiteSprite;

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;
    public Sprite greySlot;




    private void Awake()
    {
        Instance = this;
        pistolWeaponSprite = Resources.Load<GameObject>("Desert_Eagle_Weapon");
        rifleWeaponSprite = Resources.Load<GameObject>("Deadeye_Weapon");
        pistolAmmoSprite = Resources.Load<GameObject>("Pistol_Ammo");
        rifleAmmoSprite = Resources.Load<GameObject>("Rifle_Ammo");
        dynamiteSprite = Resources.Load<GameObject>("Dynamite");
        spoiledDynamiteSprite = Resources.Load<GameObject>("Spoiled_Dynamite");
    }

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }

        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }

        if(WeaponManager.Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = greySlot;
        }
        if (WeaponManager.Instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = greySlot;
        }


    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        return model switch
        {
            Weapon.WeaponModel.Desert_Eagle => pistolWeaponSprite.GetComponent<SpriteRenderer>().sprite,
            Weapon.WeaponModel.Deadeye => rifleWeaponSprite.GetComponent<SpriteRenderer>().sprite,
            _ => null,
        };
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        return model switch
        {
            Weapon.WeaponModel.Desert_Eagle => pistolAmmoSprite.GetComponent<SpriteRenderer>().sprite,
            Weapon.WeaponModel.Deadeye => rifleAmmoSprite.GetComponent<SpriteRenderer>().sprite,
            _ => null,
        };
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if(weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }

    public void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.Instance.tacticalsCount}";

        switch (WeaponManager.Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Dynamite:
                lethalUI.sprite = dynamiteSprite.GetComponent<SpriteRenderer>().sprite;
                break;
        }

        switch (WeaponManager.Instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Spoiled_Dynamite:
                tacticalUI.sprite = spoiledDynamiteSprite.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
}
