using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour, Shootable
{
    // Velocity properties
    public float maxVelocity = 10.0f;
    public float slowdownAmount = 0.5f; // smaller means faster slowdown

    // Explosion properties
    public float explosionThresholdVelocity = 0.5f;
    public float explosionForce = 60.0f;
    public float explosionUpwardsModifier = 0.001f;

    private bool m_isExploding = false;
    private Rigidbody m_rb;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // clamp the max velocity to prevent the grenade from going crazy
        m_rb.velocity = Vector3.ClampMagnitude(m_rb.velocity, maxVelocity);
        
        // If we are in the process of exploding, start slowing down
        if (m_isExploding)
        {
            m_rb.velocity *= slowdownAmount;
            m_rb.angularVelocity *= slowdownAmount;

            // If we've slowed down enough to be considered "stopped" then trigger the explosion animation
            if (Vector3.Distance(m_rb.velocity, Vector3.zero) <= explosionThresholdVelocity)
            {
                // TODO: Play animation/particle effect
                Debug.Log("GAME OVER MAN! GAME OVER!");
                Destroy(gameObject);
            }
        }
    }
    
    // Implements Shootable OnShot function
    public void OnShot(Vector3 position, Vector3 direction)
    {
        m_rb.AddExplosionForce(explosionForce, position, 0.0f, explosionUpwardsModifier, ForceMode.VelocityChange);
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
            m_rb.velocity = Vector3.zero;
            m_rb.angularVelocity = Vector3.zero;
        }
        m_rb.useGravity = !isFrozen;
        m_rb.detectCollisions = !isFrozen;
    }
}
