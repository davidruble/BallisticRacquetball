using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

[RequireComponent(typeof(Animator))]
public class Gun : DistanceGrabbable
{
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;

    public float bulletForce = 100.0f;
    public float casingExitForce = 550.0f;

    public void FireWeapon(object handThatFired)
    {
        // This will trigger the fire animation AS WELL AS the Shoot() and CasingRelease() functions
        GetComponent<Animator>().SetTrigger("Fire");
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);

        // Fire weapon when index trigger on the hand that grabbed it is pressed
        HandInteractions interactions = grabbedBy.gameObject.GetComponent<HandInteractions>();
        if (interactions != null)
        {
            interactions.indexTriggerPressed += FireWeapon;
        }
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        // Remove the fire weapon trigger when dropped
        HandInteractions interactions = grabbedBy.gameObject.GetComponent<HandInteractions>();
        if (interactions != null)
        {
            interactions.indexTriggerPressed -= FireWeapon;
        }

        base.GrabEnd(linearVelocity, angularVelocity);
    }

    // This will be called by the Animator during the shoot animation
    void Shoot()
    {
        // Instantiate the bullet and give it a little push
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * bulletForce);

        // Create the muzzle flash effect
        Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
    }

    // This will be called by the Animator during the shoot animation
    void CasingRelease()
    {
        // Explosively expel the casing up and to the right
        GameObject casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        casing.GetComponent<Rigidbody>().AddExplosionForce(casingExitForce, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 2f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
    }
}
