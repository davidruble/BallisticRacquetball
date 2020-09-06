using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    public float maxVelocity = 10.0f;
    public float slowdownAmount = 0.5f; // smaller means faster slowdown
    public float explosionThresholdVelocity = 0.5f;

    private bool m_isExploding = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // clamp the max velocity to prevent the grenade from going crazy
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        
        // If we are in the process of exploding, start slowing down
        if (m_isExploding)
        {
            rb.velocity *= slowdownAmount;
            rb.angularVelocity *= slowdownAmount;

            // If we've slowed down enough to be considered "stopped" then trigger the explosion animation
            if (Vector3.Distance(rb.velocity, Vector3.zero) <= explosionThresholdVelocity)
            {
                // TODO: Play animation/particle effect
                Debug.Log("GAME OVER MAN! GAME OVER!");
                Destroy(gameObject);
            }
        }
    }

    // Triggers the rapid slowing of the grenade to a stop and then an explosion
    public void StartExplosion()
    {
        Debug.Log("Explosion starting");
        m_isExploding = true;
    }

    // Toggles grenade frozen in place
    public void ToggleFrozen(bool isFrozen)
    {
        if (isFrozen)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.useGravity = !isFrozen;
        rb.detectCollisions = !isFrozen;
    }
}
