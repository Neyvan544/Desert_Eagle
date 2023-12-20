using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        Dynamite
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if(hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0f && !hasExploded)
            {
                Exploded();
                hasExploded = true;
            }
        }
    }

    private void Exploded()
    {
        GetThrowableEffect();

        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
       switch (throwableType)
        {
            case ThrowableType.Dynamite:
                DynamiteEffect();
                break;
        }
    }

    private void DynamiteEffect()
    {
        GameObject explosionEffect = GlobalReferences.Instance.dynamiteExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);


        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null )
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
        }
    }
}
