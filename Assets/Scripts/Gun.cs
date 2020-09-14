using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

[RequireComponent(typeof(Animator))]
public class Gun : DistanceGrabbable
{
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject impactPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    
    // Aiming parameters
    public float aimTolerance = 0.5f;
    public float maxShootDistance = 25.0f;
    
    // Bullet casing parameters
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
        // Perform a hit scan to see if we shot anything. If anything was hit, perform "hit" actions.
        RaycastHit hit;
        if (Physics.SphereCast(barrelLocation.position, aimTolerance, barrelLocation.forward, out hit, maxShootDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            // Instantiate the impact prefab at the point of contact
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            InstantiateParticleEffect(impactPrefab, hit.point, rotation);
            
            // If the object has any additional actions to perform when shot, trigger those.
            Shootable shootableObj = hit.transform.GetComponent<Shootable>();
            shootableObj?.OnShot(hit.point, barrelLocation.forward);
        }

        // Create the muzzle flash effect
        InstantiateParticleEffect(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
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

    // Helper function to create and then destroy a particle system (bullet impact, muzzle flash, etc)
    void InstantiateParticleEffect(GameObject prefab, Vector3 pos, Quaternion rot, float maxDuration = 5.0f)
    {
        if (prefab)
        {
            GameObject instance = Instantiate(prefab, pos, rot);
            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps) Destroy(instance, ps.main.duration);
            else Destroy(instance, maxDuration);
        }
        else
        {
            Debug.LogError("Attempting to instantiate explosion without a set prefab");
        }
    }

}
