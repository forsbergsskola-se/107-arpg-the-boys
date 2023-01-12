using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public DifficultyTracker difficultyTracker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            difficultyTracker = FindObjectOfType<DifficultyTracker>();
            difficultyTracker.difficulty += difficultyTracker.difficulty;
            difficultyTracker.LoadNewDungeon();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            difficultyTracker = FindObjectOfType<DifficultyTracker>();
            difficultyTracker.difficulty += difficultyTracker.difficulty;
            difficultyTracker.LoadNewDungeon();
        }
    }
}
