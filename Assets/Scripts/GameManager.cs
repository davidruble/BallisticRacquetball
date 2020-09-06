using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Grenade grenade;
    public Timer timer;
    public Transform center;

    public int startCountdownSeconds = 3;
    public int grenadeCountdownSeconds = 10;

    private bool m_gameStarted = false;

    public static GameManager Instance
    {
        get
        {
            return mInstance as GameManager;
        }
        set
        {
            mInstance = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (grenade == null || timer == null || center == null)
        {
            Debug.LogError("Grenade, Timer, or Center unassigned on GameManager");
            return;
        }

        // Grenade should start off frozen in place
        grenade.ToggleFrozen(true);

        // Subscribe to timer events
        timer.timerCollision += OnTimerCollision;
        timer.timeOut += OnTimeout;
    }

    private void OnTimerCollision(object info)
    {
        // The first timer collision starts the initial countdown timer
        if (!m_gameStarted)
        {
            timer.ToggleCollisions(false);
            timer.StartCountdown(startCountdownSeconds);
        }
    }

    private void OnTimeout(object info)
    {
        if (!m_gameStarted)
        {
            // Timer will countdown to 0 before starting the game
            Debug.Log("Starting game with a timer of " + grenadeCountdownSeconds);
            m_gameStarted = true;
            grenade.ToggleFrozen(false);
            timer.StartCountdown(grenadeCountdownSeconds);
        } 
        else
        {
            // Game over condition
            grenade.StartExplosion();

            // We can determine the winner by which side of center the grenade is.
            // Center points AWAY from the player so if the dot product is negative it's on the player's side.
            Vector3 heading = grenade.transform.position - center.position;
            float dot = Vector3.Dot(heading, center.forward);
            if (dot < 0.0f)
            {
                Debug.Log("I'm sorry Dave");
                timer.SetText("YOU LOSE");
            }
            else
            {
                Debug.Log("No, I'M sorry, HAL");
                timer.SetText("YOU WIN");
            }

            // TODO: Other game over events
        }

    }
}
