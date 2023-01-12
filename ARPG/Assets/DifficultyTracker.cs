using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyTracker : MonoBehaviour
{
    public int difficulty = 1;
    public GameObject player;

    public void LoadNewDungeon()
    {
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("NewDungeon");
    }
}
