using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class Timer : MonoBehaviour, Shootable
{
    private TextMeshPro m_timerText;
    private int m_secondsRemaining;
    private IEnumerator m_countdownCoroutine;

    public delegate void TimerEvent(object info);
    public event TimerEvent timeOut;
    public event TimerEvent timerCollision;

    void Awake()
    {
        m_timerText = GetComponent<TextMeshPro>();
    }

    // Start the timer (stopping any previously started ones)
    public void StartCountdown(int numSeconds)
    {
        SetText(numSeconds.ToString());
        if (m_countdownCoroutine != null) StopCoroutine(m_countdownCoroutine);
        m_countdownCoroutine = CountdownCoroutine(numSeconds);
        StartCoroutine(m_countdownCoroutine);
    }

    // Toggles collisions on/off
    public void ToggleCollisions(bool collisionsEnabled)
    {
        GetComponent<Collider>().enabled = collisionsEnabled;
    }

    // Sets the text of the timer text field
    public void SetText(string msg)
    {
        m_timerText.SetText(msg);
    }

    // Performs the timer countdown and updates the timer text
    private IEnumerator CountdownCoroutine(int countdownStart)
    {
        m_secondsRemaining = countdownStart;
        while (m_secondsRemaining > 0)
        {
            Debug.Log("Countdown: " + m_secondsRemaining);
            yield return new WaitForSeconds(1.0f);
            m_secondsRemaining--;
            SetText(m_secondsRemaining.ToString());
        }
        timeOut?.Invoke(m_secondsRemaining);
        yield return null;
    }

    // Implements Shootable OnShot function
    public void OnShot(Vector3 position, Vector3 direction)
    {
        timerCollision?.Invoke(position);
    }
}
