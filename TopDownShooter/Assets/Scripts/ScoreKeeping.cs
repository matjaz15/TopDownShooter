using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeping : MonoBehaviour {

    public static int kills { get; private set; }
    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1;


    private void Start() {
        Enemy.OnDeathStatic += OnEnemyDeath;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        kills = 0;
    }

    private void OnEnemyDeath() {
        kills++;
    }

    public static int GetMostKills()
    {
        return PlayerPrefs.GetInt("MostKills");
    }

    public static bool IsHighestScore() {
        int mostKills = PlayerPrefs.GetInt("MostKills");
        if (kills >= mostKills)
        {
            PlayerPrefs.SetInt("MostKills", kills);
            return true;            
        }
        return false;
    }

    void OnPlayerDeath() {
        Enemy.OnDeathStatic -= OnEnemyDeath;
        IsHighestScore();



    }



}
