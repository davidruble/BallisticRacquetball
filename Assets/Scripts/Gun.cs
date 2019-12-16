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

    private float bulletForce = 2000.0f;
    public float casingExitForce = 550.0f;
    public float casingLifetimeSec = 5.0f;

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
        // Instantiate the bullet and give it a little push. We instantiate it a little bit forward
        // to avoid colliding with the gun itself.
        Vector3 bulletPos = barrelLocation.position + barrelLocation.forward * 0.1f;
        GameObject bullet = Instantiate(bulletPrefab, bulletPos, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * bulletForce);

        // Create the muzzle flash effect and destroy it once it's effect is done
        GameObject flash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
        ParticleSystem ps = flash.GetComponent<ParticleSystem>();
        Destroy(flash, ps.main.duration);
    }

    // This will be called by the Animator during the shoot animation
    void CasingRelease()
    {
        // Explosively expel the casing up and to the right
        GameObject casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        casing.GetComponent<Rigidbody>().AddExplosionForce(casingExitForce, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 2f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);

        // Clean up the casings after some time
        Destroy(casing, casingLifetimeSec);
    }
}
