using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }


    public AudioSource ShootingChannel;

    public AudioClip Desert_EagleShot;
    public AudioClip DeadeyeShot;

    public AudioSource reloadingSoundDeadeye;
    public AudioSource reloadingSoundDesert_Eagle;


    public AudioSource emptyMagazineSoundDesert_Eagle;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {
            case WeaponModel.Desert_Eagle:
                ShootingChannel.PlayOneShot(Desert_EagleShot);
                break;
            case WeaponModel.Deadeye:
                ShootingChannel.PlayOneShot(DeadeyeShot);
                break;
        }

    }
    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Desert_Eagle:
                reloadingSoundDesert_Eagle.Play();
                break;
            case WeaponModel.Deadeye:
                reloadingSoundDeadeye.Play();
                break;
        }
    }
}
