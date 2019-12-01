using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandInteractions : DistanceGrabber
{
    public float vibrationFrequency = 0.2f;
    public float vibrationAmplitude = 0.2f;

    public delegate void HandAction(object sender);

    public event HandAction indexTriggerPressed;
    public event HandAction indexTriggerUnpressed;

    private bool m_isVibrating = false;
    private float m_vibrationDuration = 0.0f;

    private bool m_isIndexTriggerPressed = false;

    // Expose which type of controller this is
    public OVRInput.Controller Controller
    {
        get { return m_controller; }
    }

    public void StartVibrationEvent(float duration)
    {
        // Set the duration. If already vibrating, restart the vibration "timer".
        m_vibrationDuration = duration;

        if (!m_isVibrating)
        {
            // Start vibrating if not already vibrating. Update will take care of stopping vibration.
            m_isVibrating = true;
            OVRInput.SetControllerVibration(vibrationFrequency, vibrationAmplitude, m_controller);
        }
    }

    protected override void Update()
    {
        base.Update();

        // Stop vibration if the "timer" has run down
        if (m_vibrationDuration > 0.0f)
        {
            m_vibrationDuration -= Time.deltaTime;
            if (m_vibrationDuration <= 0.0f)
            {
                m_isVibrating = false;
                OVRInput.SetControllerVibration(0.0f, 0.0f, m_controller);
            }
        }
    }

    #region Callbacks

    public void OnIndexTriggerPressed()
    {
        indexTriggerPressed?.Invoke(this);
    }

    public void OnIndexTriggerUnpressed()
    {
        indexTriggerUnpressed?.Invoke(this);
    }

    #endregion

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Index trigger
        float indexPress = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
        if (indexPress > 0.0f && !m_isIndexTriggerPressed)
        {
            m_isIndexTriggerPressed = true;
            OnIndexTriggerPressed();
        }
        else if (indexPress == 0.0f && m_isIndexTriggerPressed)
        {
            m_isIndexTriggerPressed = false;
            OnIndexTriggerUnpressed();
        }
    }
}
