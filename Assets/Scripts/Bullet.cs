using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject explosionPrefab;

    public float bulletLifetimeSec = 2.0f;
    public float explosionForce = 60.0f;
    public float explosionUpwardsModifier = 0.001f;

    void Start()
    {
        // Destroy object automatically if it doesn't collide with anything
        Destroy(gameObject, bulletLifetimeSec);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        // Instantiate the explosion prefab at the point of contact
        if (explosionPrefab != null)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Instantiate(explosionPrefab, contact.point, rotation);
        }

        // If we hit a shootable object, add an explosive force to it
        if (collision.gameObject.CompareTag("Shootable"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, contact.point, 0.0f, explosionUpwardsModifier, ForceMode.VelocityChange);
            }
        }

        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}
